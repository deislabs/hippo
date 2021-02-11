from django.urls import reverse_lazy
from django.views import generic
from django.views.generic import edit

from guardian.mixins import PermissionListMixin, PermissionRequiredMixin
from guardian.shortcuts import assign_perm

from .models import App

class ListView(PermissionListMixin, generic.ListView):
    model = App
    permission_required = 'view_app'
    context_object_name = 'apps'


class DetailView(PermissionRequiredMixin, generic.DetailView):
    model = App
    permission_required = 'view_app'

class CreateView(PermissionRequiredMixin, edit.CreateView):
    model = App
    permission_required = 'apps.add_app'
    permission_object = None
    fields = ['name']

    def form_valid(self, form):
        resp = super().form_valid(form)
        assign_perm('view_app', self.request.user, self.object)
        assign_perm('change_app', self.request.user, self.object)
        assign_perm('delete_app', self.request.user, self.object)
        return resp

class UpdateView(PermissionRequiredMixin, edit.UpdateView):
    permission_required = 'change_app'
    model = App
    fields = ['name']

class DeleteView(PermissionRequiredMixin, edit.DeleteView):
    permission_required = 'delete_app'
    model = App
    success_url = reverse_lazy('apps:list')
