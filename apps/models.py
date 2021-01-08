from django.conf import settings
from django.db import models

from pegasus.models import UuidTimestampedModel

class App(UuidTimestampedModel):
    name = models.CharField(max_length=200, unique=True)
    owner = models.ForeignKey(settings.AUTH_USER_MODEL, on_delete=models.CASCADE)

    def __str__(self):
        return self.name
