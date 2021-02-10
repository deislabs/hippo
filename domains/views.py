from django.contrib.auth.mixins import LoginRequiredMixin
from django.urls import reverse_lazy
from django.views import generic
from django.views.generic import edit

from guardian.mixins import PermissionRequiredMixin
from guardian.shortcuts import get_objects_for_user

from .models import Domain

class IndexView(generic.ListView, LoginRequiredMixin):
    context_object_name = 'domains'

    def get_queryset(self):
        """Return all environment variables."""
        return get_objects_for_user(self.request.user, 'view_domain', Domain)

class DetailView(generic.DetailView, PermissionRequiredMixin):
    permission_required = 'view_domain'
    model = Domain

class CreateView(edit.CreateView, PermissionRequiredMixin):
    permission_required = 'add_domain'
    model = Domain
    fields = ['domain', 'app']

    def form_valid(self, form):
        form.instance.owner = self.request.user
        return super().form_valid(form)

class UpdateView(edit.UpdateView, PermissionRequiredMixin):
    permission_required = 'change_domain'
    model = Domain
    fields = ['domain', 'app']

class DeleteView(edit.DeleteView, PermissionRequiredMixin):
    permission_required = 'delete_domain'
    model = Domain
    success_url = reverse_lazy('domains:index')
