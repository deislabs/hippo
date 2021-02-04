from django.contrib import admin

from .models import EnvironmentVariable, Function

admin.site.register(EnvironmentVariable)
admin.site.register(Function)
