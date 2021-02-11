from django.contrib.auth.mixins import LoginRequiredMixin
from django.urls import reverse_lazy
from django.views import generic
from django.views.generic import edit

from guardian.mixins import PermissionRequiredMixin
from guardian.shortcuts import get_objects_for_user

from .models import Function

class IndexView(LoginRequiredMixin, generic.ListView):
    context_object_name = 'functions'

    def get_queryset(self):
        """Return all environment variables."""
        return get_objects_for_user(self.request.user, 'view_function', Function)

class CreateView(PermissionRequiredMixin, edit.CreateView):
    permission_required = 'add_function'
    model = Function
    fields = ['owner', 'name', 'args']

class UpdateView(PermissionRequiredMixin, edit.UpdateView):
    permission_required = 'change_function'
    model = Function
    fields = ['name', 'args']

class DeleteView(PermissionRequiredMixin, edit.DeleteView):
    permission_required = 'delete_function'
    model = Function
    success_url = reverse_lazy('functions:index')
