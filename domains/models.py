from django.db import models

from apps.models import App

class Domain(models.Model):
    app = models.ForeignKey(App, on_delete=models.CASCADE)
    domain = models.TextField(blank=False, null=False, unique=True, error_messages={
        'unique': 'domain is already in use by another application',
    })
