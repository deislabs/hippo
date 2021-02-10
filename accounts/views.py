from django.conf import settings
from django.contrib.auth import login
from django.contrib.auth.decorators import login_required
from django.http import HttpResponse, HttpResponseNotFound
from django.shortcuts import redirect, render
from django.urls import reverse

from apps.models import App
from .models import CustomUser
from .forms import CustomUserCreationForm

@login_required
def profile(request):
    context = {
        'apps': App.objects.filter(owner=request.user.pk)
    }
    return render(request, 'accounts/profile.html', context)

def register(request):
    if settings.REGISTRATION_MODE == 'disabled':
        return HttpResponseNotFound('Registration has been disabled. Please contact the administrator.')
    if request.method == 'GET':
        return render(
            request, 'registration/register.html',
            {"form": CustomUserCreationForm}
        )
    elif request.method == 'POST':
        form = CustomUserCreationForm(request.POST)
        if form.is_valid():
            user = form.save()
            # First user created is an administrator
            if not CustomUser.objects.filter(is_superuser=True).exists():
                user.is_superuser = user.is_staff = True
                user.save()
            login(request, user)
            return redirect(reverse('profile'))
