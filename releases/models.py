import os
import psutil
import socket
import subprocess
import toml

from django.conf import settings
from django.db import models
from django.db.models.signals import post_save, post_delete
from django.dispatch import receiver
from django.urls import reverse

from hippo.models import UuidTimestampedModel
from apps.models import App
from builds.models import Build
from domains.models import Domain
from envvars.models import EnvironmentVariable

class Release(UuidTimestampedModel):
    owner = models.ForeignKey(App, on_delete=models.CASCADE)
    build = models.ForeignKey(Build, on_delete=models.CASCADE)
    version = models.PositiveIntegerField()

    def save(self, *args, **kwargs):
        try:
            self.version = self.owner.release_set.latest('created').version + 1
        except Release.DoesNotExist:
            self.version = 1
        super().save(*args, **kwargs)
        with open(self.wagi_config_path(), 'w') as f:
            toml.dump(self.wagi_config(), f)
        with open(self.systemd_service_path(), 'w') as f:
            f.write(self.systemd_service())
        # if we're the first release to be deployed, start the systemd unit
        if self.owner.release_set.count() == 1:
            subprocess.call(['systemctl', 'start', 'hippo-{}'.format(self.owner.name)])
            subprocess.call(['systemctl', 'enable', 'hippo-{}'.format(self.owner.name)])
        else:
            subprocess.call(['systemctl', 'daemon-reload'])
            subprocess.call(['systemctl', 'reload', 'hippo-{}'.format(self.owner.name)])
        # we need to wait until the app has started. We need the PID file to determine which port we need to wire up.
        with open(self.traefik_config_path(), 'w') as f:
            toml.dump(self.traefik_config(), f)

    def delete(self, *args, **kwargs):
        # check if we're the last release to be removed; if so we need to remove the systemd unit
        if self.owner.release_set.count() == 1:
            subprocess.call(['systemctl', 'stop', 'hippo-{}'.format(self.owner.name)])
            subprocess.call(['systemctl', 'disable', 'hippo-{}'.format(self.owner.name)])
            os.remove(self.systemd_service_path())
            os.remove('/usr/lib/systemd/system/hippo-{}.service'.format(self.owner.name))
            subprocess.call(['systemctl', 'daemon-reload'])
            subprocess.call(['systemctl', 'reset-failed'])
        super().delete(*args, **kwargs)

    def get_absolute_url(self):
        return reverse('apps:detail', kwargs={'pk': self.owner.pk})

    def wagi_config_path(self):
        return os.path.join(settings.MEDIA_ROOT, self.owner.name, 'modules.toml')

    def wagi_config(self):
        module_path = self.build.artifact.path
        envvars = EnvironmentVariable.objects.filter(owner=self.owner)
        domains = Domain.objects.filter(owner=self.owner)
        wagi_config = {
            'module': [],
        }
        for domain in domains:
            module_config = {
                'module': self.build.artifact.path,
                'route': '/',
                'host': domain.domain,
            }
            for envvar in envvars:
                module_config['environment'][envvar.key] = envvar.value
            wagi_config['module'].append(module_config)
        return wagi_config

    def systemd_service_path(self):
        return '/etc/systemd/system/hippo-{}.service'.format(self.owner.name)

    def systemd_service(self):
        svc = '[Unit]\n'
        svc += 'Description=Hippo runtime for app {}\n\n'.format(self.owner.name)
        svc += '[Service]\n'
        svc += 'Type=simple\n'
        svc += "ExecStart=/usr/local/bin/wagi --config {} --listen 0.0.0.0:0\n\n".format(self.wagi_config_path())
        svc += '[Install]\nWantedBy=multi-user.target\n'
        return svc

    def traefik_config_path(self):
        return '/etc/traefik/conf.d/{}.toml'.format(self.owner.name)

    def traefik_config(self):
        traefik_config = {}
        pid = 0
        try:
            # output will be something like 'ExecMainPID=27197'
            output = subprocess.check_output(['systemctl', 'show', '-p', 'ExecMainPID', 'hippo-{}'.format(self.owner.name)])
            pid = int(output.decode('utf-8').split('=')[1])
        except Exception as e:
            print(e)
            return traefik_config
        port = 0
        for connection in psutil.net_connections(kind='tcp'):
            if connection.pid == pid:
                port = connection.laddr.port
        if port == 0:
            print('could not find port')
            return traefik_config
        domains = Domain.objects.filter(owner=self.owner)
        for domain in domains:
            traefik_config.update({
                'http': {
                    'routers': {
                        'to-{}'.format(self.owner.name): {
                            'rule': 'Host(`{}`) && PathPrefix(`/`)'.format(domain.domain),
                            'service': self.owner.name,
                        }
                    },
                    'services': {
                        self.owner.name: {
                            'LoadBalancer': {
                                'servers': [
                                    {
                                        'url': 'http://localhost:{}'.format(port)
                                    }
                                ]
                            }
                        }
                    }
                }
            })
        return traefik_config

@receiver(post_save, sender=Build)
def create_release_on_build_save(sender,**kwargs):
    Release.objects.create(owner=sender.owner, build=sender)

@receiver(post_save, sender=EnvironmentVariable)
def create_release_on_envvar_save(sender,**kwargs):
    if sender.owner.build_set.count() != 0:
        Release.objects.create(
            owner=sender.owner,
            # since there's at least one build, we can assume here that
            # there has also been a release. it is important that we use the
            # latest release to find the correct build; it is possible that
            # the latest build is not currently in use (e.g. a rollback)
            build=sender.owner.release_set.latest('created').build
        )

@receiver(post_delete, sender=EnvironmentVariable)
def create_release_on_envvar_delete(sender,**kwargs):
    if sender.owner.build_set.count() != 0:
        Release.objects.create(
            owner=sender.owner,
            # since there's at least one build, we can assume here that there has also been a release.
            build=sender.owner.release_set.latest('created').build
        )
