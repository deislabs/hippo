from OpenSSL import crypto

from django.core.exceptions import ValidationError
from django.db import models
from django.urls import reverse

from domains.models import Domain
from pegasus.models import UuidTimestampedModel

def validate_certificate(value):
    try:
        return crypto.load_certificate(crypto.FILETYPE_PEM, value)
    except crypto.Error as e:
        raise ValidationError('Could not load certificate: {}'.format(e))


def validate_private_key(value):
    try:
        return crypto.load_privatekey(crypto.FILETYPE_PEM, value)
    except crypto.Error as e:
        raise ValidationError('Could not load private key: {}'.format(e))

class Certificate(UuidTimestampedModel):
    """
    Public and private key pair used to secure application traffic at the router.
    """
    owner = models.ForeignKey(Domain, on_delete=models.CASCADE)
    certificate = models.FileField(validators=[validate_certificate])
    key = models.FileField(validators=[validate_private_key])

    def get_absolute_url(self):
        return reverse('certificates:detail', kwargs={'pk': self.pk})
