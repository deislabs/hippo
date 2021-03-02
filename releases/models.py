import os

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


def get_upload_path(instance, filename):
    return urljoin('uploads', instance.owner.name, instance.version, 'app.wasm')

class Release(UuidTimestampedModel):
    owner = models.ForeignKey(App, on_delete=models.CASCADE)
    build = models.FileField(upload_to=get_upload_path)
    description = models.TextField(blank=True)
    version = models.PositiveIntegerField()

    def save(self, *args, **kwargs):
        try:
            self.version = self.owner.release_set.latest('created').version + 1
        except Release.DoesNotExist:
            self.version = 1
        super().save(*args, **kwargs)
        with open(os.path.join(os.path.dirname(self.build.path), 'modules.toml'), 'w') as f:
            f.write(self.get_wagi_config())

    def get_absolute_url(self):
        return reverse('releases:detail', kwargs={'pk': self.pk})

    def get_wagi_config(self):
        module_path = self.build.path
        route = '/'
        envvars = EnvironmentVariable.objects.filter(owner=self.owner)
        domains = Domain.objects.filter(owner=self.owner)
        wagi_config = ''
        for domain in domains:
            wagi_config += '[[module]]\n'
            wagi_config += 'module = "app.wasm"\n'
            wagi_config += 'route = "{}"\n'.format(route)
            for envvar in envvars:
                wagi_config += 'environment.{} = "{}"\n'.format(envvar.key, envvar.value)
            wagi_config += 'host = "{}"\n'.format(domain.domain)
            wagi_config += '\n'
        return wagi_config
