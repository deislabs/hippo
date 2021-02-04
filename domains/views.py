from django.core import serializers
from django.contrib.auth.decorators import login_required
from django.forms.models import model_to_dict
from django.http import HttpResponse, JsonResponse
from django.shortcuts import get_object_or_404, get_list_or_404

from .models import Domain
from pegasus.serialization import to_dict

@login_required
def index(request):
    domains = [to_dict(d) for d in get_list_or_404(Domain, owner=request.user.pk)]
    return JsonResponse(domains, safe=False)
