from django.db import models
import uuid

class TimestampedModel(models.Model):
    """Adds created and updated fields to a model."""

    created = models.DateTimeField('date created', auto_now_add=True)
    updated = models.DateTimeField('date updated', auto_now=True)

    class Meta:
        abstract = True


class UuidTimestampedModel(TimestampedModel):
    """Add a UUID primary key to an :class:`TimestampedModel`."""

    uuid = models.UUIDField('UUID',
                            default=uuid.uuid4,
                            primary_key=True,
                            editable=False,
                            auto_created=True,
                            unique=True)

    class Meta:
        """Mark :class:`UuidTimestampedModel` as abstract."""
        abstract = True
