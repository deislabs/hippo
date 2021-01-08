from django.conf import settings
from django.db import models

from pegasus.models import AuditedModel

class Certificate(TimestampedModel):
    """
    Public and private key pair used to secure application traffic at the router.
    """
    owner = models.ForeignKey(settings.AUTH_USER_MODEL, on_delete=models.PROTECT)
    name = models.CharField(max_length=253, unique=True, validators=[validate_label])
    # there is no upper limit on the size of an x.509 certificate
    certificate = models.TextField(validators=[validate_certificate])
    key = models.TextField(validators=[validate_private_key])
    # X.509 certificates allow any string of information as the common name.
    common_name = models.TextField(editable=False, unique=False, null=True)
    # A list of DNS records if certificate has SubjectAltName
    san = ArrayField(models.CharField(max_length=253), null=True)
    # SHA256 fingerprint
    fingerprint = models.CharField(max_length=96, editable=False)
    # Expires and Start time of cert
    expires = models.DateTimeField(editable=False)
    starts = models.DateTimeField(editable=False)
    issuer = models.TextField(editable=False)
    subject = models.TextField(editable=False)
