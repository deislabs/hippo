from django.conf import settings
from django.db import models

from pegasus.models import UuidTimestampedModel
from apps.models import App

class Domain(UuidTimestampedModel):
    owner = models.ForeignKey(settings.AUTH_USER_MODEL, on_delete=models.CASCADE)
    domain = models.CharField(max_length=255, blank=False, null=False, unique=True, error_messages={
        'unique': 'domain has already been claimed by another user',
    })
    app = models.ForeignKey(App, on_delete=models.CASCADE)

    def __str__(self):
        return self.domain
