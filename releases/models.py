from django.db import models
from django.urls import reverse

from pegasus.models import UuidTimestampedModel
from apps.models import App

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

    def get_absolute_url(self):
        return reverse('releases:detail', kwargs={'pk': self.pk})
