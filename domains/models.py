from django.conf import settings
from django.db import models
from django.urls import reverse

from hippo.models import UuidTimestampedModel
from apps.models import App

class Domain(UuidTimestampedModel):
    owner = models.ForeignKey(App, on_delete=models.CASCADE)
    domain = models.CharField(max_length=255, blank=False, null=False, unique=True, error_messages={
        'unique': 'domain has already been claimed',
    })

    def __str__(self):
        return self.domain

    def get_absolute_url(self):
        return reverse('apps:detail', kwargs={'pk': self.owner.pk})
