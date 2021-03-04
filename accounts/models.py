from django.contrib.auth.models import AbstractUser
from django.db import models
from django.urls import reverse

from hippo.models import UuidTimestampedModel

class CustomUser(AbstractUser, UuidTimestampedModel):
    pass

    def get_absolute_url(self):
        return reverse('apps:list')
