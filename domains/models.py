from django.conf import settings
from django.db import models

from apps.models import App

class Domain(models.Model):
    owner = models.ForeignKey(settings.AUTH_USER_MODEL, on_delete=models.CASCADE)
    domain = models.CharField(max_length=255, blank=False, null=False, unique=True, error_messages={
        'unique': 'domain has already been claimed by another user',
    })

    def __str__(self):
        return self.domain

class DomainBinding(models.Model):
    app = models.ForeignKey(App, on_delete=models.CASCADE)
    domain = models.ForeignKey(Domain, on_delete=models.CASCADE)
    cname = models.CharField(max_length=200, blank=True)

    class Meta:
        constraints = [
            models.UniqueConstraint(fields=['app', 'cname', 'domain'], name='domain_is_unique')
        ]

    def __str__(self):
        if self.cname == '':
            return '{app}.{domain}'.format(app=self.app.name, domain=str(self.domain))
        return '{cname}.{domain}'.format(app=self.app.name, domain=str(self.domain))
