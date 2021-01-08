from django.contrib import admin

from .models import Domain, DomainBinding

admin.site.register(Domain)
admin.site.register(DomainBinding)
