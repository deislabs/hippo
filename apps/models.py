from django.db import models

class App(models.Model):
    name = models.CharField(max_length=200, unique=True)
    created_date = models.DateTimeField('date created', auto_now_add=True)
    last_updated = models.DateTimeField('date updated', auto_now=True)

    def __str__(self):
        return self.name
