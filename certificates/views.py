from django.contrib.auth.mixins import LoginRequiredMixin
from django.urls import reverse_lazy
from django.views import generic
from django.views.generic import edit

from guardian.mixins import PermissionRequiredMixin
from guardian.shortcuts import get_objects_for_user

from .models import Certificate

class IndexView(generic.ListView, LoginRequiredMixin):
    context_object_name = 'certificates'

    def get_queryset(self):
        """Return all environment variables."""
        return get_objects_for_user(self.request.user, 'view_certificate', Certificate)

class DetailView(generic.DetailView, PermissionRequiredMixin):
    permission_required = 'view_certificate'
    model = Certificate

class CreateView(edit.CreateView, PermissionRequiredMixin):
    permission_required = 'add_certificate'
    model = Certificate
    fields = ['owner', 'certificate', 'key']

class UpdateView(edit.UpdateView, PermissionRequiredMixin):
    permission_required = 'change_certificate'
    model = Certificate
    fields = ['owner', 'certificate', 'key']

class DeleteView(edit.DeleteView, PermissionRequiredMixin):
    permission_required = 'delete_certificate'
    model = Certificate
    success_url = reverse_lazy('certificates:index')
