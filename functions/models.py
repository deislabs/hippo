from django.db import models
from django.urls import reverse

from hippo.models import UuidTimestampedModel
from apps.models import App

class Function(UuidTimestampedModel):
    owner = models.ForeignKey(App, on_delete=models.CASCADE)
    name = models.CharField(max_length=100)
    args = models.CharField(max_length=1000)

    class Meta:
        constraints = [
            models.UniqueConstraint(fields=['owner', 'name'], name='function_is_unique')
        ]

    def __str__(self):
        return '"{name}": "{args}"'.format(name=self.name, args=self.args)

    def get_absolute_url(self):
        return "{}?app={}".format(reverse('functions:list'), self.owner.pk)
