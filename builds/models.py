from django.db import models
from hippo.models import UuidTimestampedModel

from apps.models import App

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
