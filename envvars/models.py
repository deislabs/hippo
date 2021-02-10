from django.db import models
from django.urls import reverse

from pegasus.models import UuidTimestampedModel
from apps.models import App

class EnvironmentVariable(UuidTimestampedModel):
    owner = models.ForeignKey(App, on_delete=models.CASCADE)
    key = models.CharField(max_length=100)
    value = models.CharField(max_length=1000)

    class Meta:
        constraints = [
            models.UniqueConstraint(fields=['owner', 'key'], name='envvar_is_unique')
        ]

    def __str__(self):
        return '{key}={value}'.format(key=self.key, value=self.value)

    def get_absolute_url(self):
        return reverse('apps:detail', kwargs={'pk': self.owner.pk})
