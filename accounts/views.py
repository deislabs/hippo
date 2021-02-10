from django.conf import settings
from django.contrib.auth import get_user_model, login
from django.contrib.auth.mixins import LoginRequiredMixin
from django.http import HttpResponseForbidden
from django.urls import reverse_lazy
from django.views import generic
from django.views.generic import edit

from guardian.shortcuts import assign_perm

from .forms import CustomUserCreationForm

class DetailView(LoginRequiredMixin, generic.DetailView):
    context_object_name = 'user'
    model = get_user_model()
    template_name = 'accounts/profile.html'

    def get_object(self):
        return self.request.user

class RegistrationView(edit.CreateView):
    model = get_user_model()
    form_class = CustomUserCreationForm
    template_name = 'registration/register.html'

    def form_valid(self, form):
        """Register the first user as a superuser."""
        if settings.REGISTRATION_MODE == 'disabled':
            return HttpResponseForbidden('Registration has been disabled. Please contact the administrator.')
        resp = super().form_valid(form)
        # the first user is the "AnonymousUser".
        if get_user_model().objects.count() == 2:
            self.object.is_superuser = self.object.is_staff = True
        self.object.save()
        assign_perm('apps.add_app', self.object)
        assign_perm('certificates.add_certificate', self.object)
        assign_perm('domains.add_domain', self.object)
        assign_perm('envvars.add_environmentvariable', self.object)
        assign_perm('functions.add_function', self.object)
        assign_perm('releases.add_release', self.object)
        return resp
