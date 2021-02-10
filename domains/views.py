from django.core import serializers
from django.contrib.auth.decorators import login_required
from django.forms.models import model_to_dict
from django.http import HttpResponse, JsonResponse
from django.shortcuts import get_object_or_404, get_list_or_404, render, redirect
from django.urls import reverse

from pegasus.serialization import to_dict

from .forms import DomainForm
from .models import Domain

@login_required
def index(request):
    domains = [to_dict(d) for d in get_list_or_404(Domain, owner=request.user.pk)]
    return JsonResponse(domains, safe=False)

@login_required
def add(request):
    if request.method == 'GET':
        return render(
            request, 'domains/create.html',
            {"form": DomainForm}
        )
    elif request.method == 'POST':
        form = DomainForm(request.POST)
        if form.is_valid():
            domain = form.save(commit=False)
            domain.owner = request.user
            domain.save()
            form.save_m2m()
            return redirect(reverse('accounts:profile'))
