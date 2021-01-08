from django.contrib.auth.decorators import login_required
from django.http import HttpResponse
from django.shortcuts import get_object_or_404, get_list_or_404

from .models import App

@login_required
def index(request):
    apps = App.objects.all()
    return HttpResponse(', '.join([str(a) for a in apps if request.user.has_perm('apps.view_app')]))

@login_required
def detail(request, app_id):
    return HttpResponse(get_object_or_404(App, pk=app_id))
