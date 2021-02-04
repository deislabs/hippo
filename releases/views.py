from django.shortcuts import render
from django.contrib.auth.decorators import login_required
from django.http import HttpResponse, Http404, JsonResponse
from django.shortcuts import get_object_or_404
import subprocess

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
