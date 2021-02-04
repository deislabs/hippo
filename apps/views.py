from django.core import serializers
from django.contrib.auth.decorators import login_required
from django.forms.models import model_to_dict
from django.http import JsonResponse
from django.shortcuts import get_object_or_404, get_list_or_404

from .models import App
from config.models import EnvironmentVariable
from pegasus.serialization import to_dict

@login_required
def index(request):
    apps = [to_dict(a) for a in get_list_or_404(App, owner=request.user.pk)]
    return JsonResponse(apps, safe=False)

@login_required
def detail(request, app_pk):
    return JsonResponse(to_dict(get_object_or_404(App, pk=app_pk, owner=request.user.pk)))
