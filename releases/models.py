from django.db import models

from pegasus.models import UuidTimestampedModel
from apps.models import App

class Release(UuidTimestampedModel):
    owner = models.ForeignKey(App, on_delete=models.CASCADE)
    build = models.FileField()
    description = models.TextField()
