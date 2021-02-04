from django.db import models

from pegasus.models import UuidTimestampedModel
from apps.models import App

class Release(UuidTimestampedModel):
    owner = models.ForeignKey(App, on_delete=models.CASCADE)
    build = models.FileField()
    description = models.TextField(blank=True)
    version = models.PositiveIntegerField()

    def save(self, *args, **kwargs):
        try:
            self.version = self.owner.release_set.latest('created').version + 1
        except Release.DoesNotExist:
            self.version = 1
        super().save(*args, **kwargs)
