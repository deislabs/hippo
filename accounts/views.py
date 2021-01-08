from django.contrib.auth import login
from django.contrib.auth.decorators import login_required
from django.contrib.auth.forms import UserCreationForm
from django.http import HttpResponse
from django.shortcuts import redirect, render
from django.urls import reverse

@login_required
def profile(request):
    return HttpResponse('welcome to your profile page!')

def register(request):
    if request.method == 'GET':
        return render(
            request, 'registration/register.html',
            {"form": UserCreationForm}
        )
    elif request.method == 'POST':
        form = UserCreationForm(request.POST)
        if form.is_valid():
            user = form.save()
            login(request, user)
            return redirect(reverse('profile'))
