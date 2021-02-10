from django.contrib import messages
from django.contrib.auth.decorators import login_required
from django.http import HttpResponse, Http404, JsonResponse
from django.shortcuts import get_object_or_404, redirect, render
from django.urls import reverse

import subprocess

from .forms import ReleaseForm
from .models import Release
from config.models import Function
from pegasus.serialization import to_dict

@login_required
def invoke(request, release_pk):
    release = get_object_or_404(Release, pk=release_pk)
    if release.owner.owner.pk != request.user.pk:
        return Http404()
    function_name = request.GET.get('fn', '_start')
    cmd = ['wasmtime', release.build.path]
    try:
        function = Function.objects.get(app=release.owner, name=function_name)
        cmd += ['--invoke', function.name, function.args.split()]
    except Function.DoesNotExist:
        pass
    completed_process = subprocess.run(cmd, capture_output=True)
    return HttpResponse(completed_process.stdout)

@login_required
def add(request):
    if request.method == 'GET':
        return render(
            request, 'releases/create.html',
            {"form": ReleaseForm}
        )
    elif request.method == 'POST':
        form = ReleaseForm(request.POST, request.FILES)
        if form.is_valid():
            form.save()
            return redirect(reverse('accounts:profile'))
