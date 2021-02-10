from django.contrib.auth.mixins import LoginRequiredMixin
from django.urls import reverse_lazy
from django.views import generic
from django.views.generic import edit

from guardian.mixins import PermissionRequiredMixin
from guardian.shortcuts import get_objects_for_user

from .models import Release

class IndexView(LoginRequiredMixin, generic.ListView):
    context_object_name = 'releases'

    def get_queryset(self):
        """Return all applications that the logged-in user has view permission."""
        return get_objects_for_user(self.request.user, 'view_release', Release)

class DetailView(PermissionRequiredMixin, generic.DetailView):
    permission_required = 'view_release'
    model = Release

class CreateView(PermissionRequiredMixin, edit.CreateView):
    permission_required = 'add_release'
    model = Release
    fields = ['owner', 'build', 'description']

class UpdateView(PermissionRequiredMixin, edit.UpdateView):
    permission_required = 'change_release'
    model = Release
    fields = ['build', 'description']

class DeleteView(PermissionRequiredMixin, edit.DeleteView):
    permission_required = 'delete_release'
    model = Release
    success_url = reverse_lazy('releases:index')
