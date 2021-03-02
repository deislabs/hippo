import os

from django.conf import settings
from django.db import models
from django.urls import reverse

from pegasus.models import UuidTimestampedModel
from apps.models import App
from domains.models import Domain
from envvars.models import EnvironmentVariable

def urljoin(*args):
    """
    Joins given arguments into an url. Trailing but not leading slashes are
    stripped for each argument.
    """
    return "/".join(map(lambda x: str(x).rstrip('/'), args))


def upload_path(instance, filename):
    return urljoin('uploads', instance.owner.name, instance.version, 'app.wasm')

class Release(UuidTimestampedModel):
    owner = models.ForeignKey(App, on_delete=models.CASCADE)
    build = models.FileField(upload_to=upload_path)
    description = models.TextField(blank=True)
    version = models.PositiveIntegerField()

    def save(self, *args, **kwargs):
        try:
            self.version = self.owner.release_set.latest('created').version + 1
        except Release.DoesNotExist:
            self.version = 1
        super().save(*args, **kwargs)
        with open(self.wagi_config_path(), 'w') as f:
            f.write(self.wagi_config())
        with open(self.systemd_service_path(), 'w') as f:
            f.write(self.systemd_service())

    def get_absolute_url(self):
        return reverse('releases:detail', kwargs={'pk': self.pk})

    def wagi_config_path(self):
        return os.path.join(os.path.dirname(self.build.path), 'modules.toml')

    def wagi_config(self):
        module_path = self.build.path
        route = '/'
        envvars = EnvironmentVariable.objects.filter(owner=self.owner)
        domains = Domain.objects.filter(owner=self.owner)
        wagi_config = ''
        for domain in domains:
            wagi_config += '[[module]]\n'
            wagi_config += 'module = "{}"\n'.format(self.build.path)
            wagi_config += 'route = "{}"\n'.format(route)
            for envvar in envvars:
                wagi_config += 'environment.{} = "{}"\n'.format(envvar.key, envvar.value)
            wagi_config += 'host = "{}"\n'.format(domain.domain)
            wagi_config += '\n'
        return wagi_config

    def systemd_service_path(self):
        return os.path.join(settings.MEDIA_ROOT, '{}.service'.format(self.owner.name))

    def systemd_service(self):
        svc = '[Unit]\n'
        svc += 'Type=simple\n'
        svc += 'ExecStart=/usr/local/bin/wagi --config {} --listen 0.0.0.0:0\n'.format(self.wagi_config_path())
        svc += 'PIDFile={}/wagi.pid\n\n'.format(os.path.dirname(self.build.path))
        svc += '[Install]\nWantedBy=multi-user.target\n'
        return svc
