from django.db import models
from django.urls import reverse

from apps.models import App
from hippo.models import UuidTimestampedModel

def urljoin(*args):
    """
    Joins given arguments into a url. Trailing but not leading slashes are
    stripped for each argument.
    """
    return "/".join(map(lambda x: str(x).rstrip('/'), args))


def upload_path(instance, filename):
    return urljoin('builds', str(instance.pk), 'app.wasm')


class Build(UuidTimestampedModel):
    owner = models.ForeignKey(App, on_delete=models.CASCADE)
    artifact = models.FileField(upload_to=upload_path)
    description = models.TextField(blank=True)

    def get_absolute_url(self):
        return reverse('apps:detail', kwargs={'pk': self.owner.pk})
