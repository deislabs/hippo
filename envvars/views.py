from django.contrib.auth.mixins import LoginRequiredMixin
from django.urls import reverse_lazy
from django.views import generic
from django.views.generic import edit

from guardian.mixins import PermissionRequiredMixin
from guardian.shortcuts import get_objects_for_user

from .models import EnvironmentVariable

class IndexView(generic.ListView, LoginRequiredMixin):
    context_object_name = 'envvars'

    def get_queryset(self):
        """Return all envioronment variables."""
        return get_objects_for_user(self.request.user, 'view_environmentvariable', EnvironmentVariable)

class DetailView(generic.DetailView, PermissionRequiredMixin):
    permission_required = 'view_environmentvariable'
    model = EnvironmentVariable

class CreateView(edit.CreateView, PermissionRequiredMixin):
    permission_required = 'add_environmentvariable'
    model = EnvironmentVariable
    fields = ['key', 'value']

    def form_valid(self, form):
        form.instance.owner = self.request.user
        return super().form_valid(form)

class UpdateView(edit.UpdateView, PermissionRequiredMixin):
    permission_required = 'change_environmentvariable'
    model = EnvironmentVariable
    fields = ['key', 'value']

class DeleteView(edit.DeleteView, PermissionRequiredMixin):
    permission_required = 'delete_environmentvariable'
    model = EnvironmentVariable
    success_url = reverse_lazy('envvars:index')
