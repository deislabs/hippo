from django.contrib.auth.mixins import LoginRequiredMixin
from django.urls import reverse_lazy
from django.views import generic
from django.views.generic import edit

from guardian.mixins import PermissionRequiredMixin
from guardian.shortcuts import get_objects_for_user

from .models import Certificate

class ListView(LoginRequiredMixin, generic.ListView):
    context_object_name = 'certificates'

    def get_queryset(self):
        """Return all environment variables."""
        return get_objects_for_user(self.request.user, 'view_certificate', Certificate)

class DetailView(PermissionRequiredMixin, generic.DetailView):
    permission_required = 'view_certificate'
    model = Certificate

class CreateView(PermissionRequiredMixin, edit.CreateView):
    permission_required = 'add_certificate'
    model = Certificate
    fields = ['owner', 'certificate', 'key']

class UpdateView(PermissionRequiredMixin, edit.UpdateView):
    permission_required = 'change_certificate'
    model = Certificate
    fields = ['owner', 'certificate', 'key']

class DeleteView(PermissionRequiredMixin, edit.DeleteView):
    permission_required = 'delete_certificate'
    model = Certificate
    success_url = reverse_lazy('certificates:list')
