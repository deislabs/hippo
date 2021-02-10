from django.core import serializers
from django.contrib.auth.decorators import login_required
from django.forms.models import model_to_dict
from django.http import HttpResponse, JsonResponse
from django.shortcuts import get_object_or_404, get_list_or_404, render, redirect
from django.urls import reverse

from apps.models import App
from .forms import EnvironmentVariableForm, FunctionForm
from .models import EnvironmentVariable
from pegasus.serialization import to_dict

@login_required
def index(request):
    apps_i_own = [a.pk for a in get_list_or_404(App, owner=request.user.pk)]
    queryset = EnvironmentVariable.objects.filter(app__in=apps_i_own)
    app = request.GET.get('app', None)
    if app:
        print(app)
        queryset = queryset.filter(app=app)
    envvars = [{'app': e.app.pk, 'key': e.key, 'value': e.value} for e in queryset]
    return JsonResponse(envvars, safe=False)

@login_required
def add_envvar(request):
    if request.method == 'GET':
        return render(
            request, 'config/create_envvar.html',
            {"form": EnvironmentVariableForm}
        )
    elif request.method == 'POST':
        form = EnvironmentVariableForm(request.POST)
        if form.is_valid():
            form.save()
            return redirect(reverse('accounts:profile'))

@login_required
def add_function(request):
    if request.method == 'GET':
        return render(
            request, 'config/create_function.html',
            {"form": FunctionForm}
        )
    elif request.method == 'POST':
        form = FunctionForm(request.POST)
        if form.is_valid():
            form.save()
            return redirect(reverse('accounts:profile'))
