from django.db import models
from django.urls import reverse

from hippo.models import UuidTimestampedModel
from apps.models import App
from releases.models import Release

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
        return "{}?app={}".format(reverse('envvars:list'), self.owner.pk)

    def save(self, *args, **kwargs):
        super().save(*args, **kwargs)
        if self.owner.build_set.count() != 0:
            Release.objects.create(
                owner=self.owner,
                # since there's at least one build, we can assume here that there has also been a release.
                build=self.owner.release_set.latest('created').build
            )
