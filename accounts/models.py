from django.contrib.auth.models import AbstractUser
from django.db import models

from pegasus.models import UuidTimestampedModel

class CustomUser(AbstractUser, UuidTimestampedModel):
    pass
