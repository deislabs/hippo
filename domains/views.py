from django.contrib.auth.decorators import login_required
from django.http import HttpResponse

@login_required
def index(request):
    return HttpResponse('You are at the config view')
