from django.core import serializers
from django.contrib.auth.decorators import login_required
from django.forms.models import model_to_dict
from django.http import HttpResponse, JsonResponse
from django.shortcuts import get_object_or_404, get_list_or_404

from apps.models import App
from .models import Domain
from pegasus.serialization import to_dict

@login_required
def index(request):
    apps_i_own = [a.pk for a in get_list_or_404(App, owner=request.user.pk)]
    queryset = Domain.objects.filter(app__in=apps_i_own)
    app = request.GET.get('app', None)
    if app:
        print(app)
        queryset = queryset.filter(app=app)
    return JsonResponse(queryset, safe=False)
