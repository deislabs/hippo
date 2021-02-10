from django.contrib.auth.mixins import LoginRequiredMixin
from django.urls import reverse_lazy
from django.views import generic
from django.views.generic import edit

from guardian.mixins import PermissionRequiredMixin
from guardian.shortcuts import get_objects_for_user

from .models import Function

class IndexView(generic.ListView, LoginRequiredMixin):
    context_object_name = 'functions'

    def get_queryset(self):
        """Return all envioronment variables."""
        return get_objects_for_user(self.request.user, 'view_function', Function)

class DetailView(generic.DetailView, PermissionRequiredMixin):
    permission_required = 'view_function'
    model = Function

class CreateView(edit.CreateView, PermissionRequiredMixin):
    permission_required = 'add_function'
    model = Function
    fields = ['name', 'args']

    def form_valid(self, form):
        form.instance.owner = self.request.user
        return super().form_valid(form)

class UpdateView(edit.UpdateView, PermissionRequiredMixin):
    permission_required = 'change_function'
    model = Function
    fields = ['name', 'args']

class DeleteView(edit.DeleteView, PermissionRequiredMixin):
    permission_required = 'delete_function'
    model = Function
    success_url = reverse_lazy('functions:index')
