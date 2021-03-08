from django.urls import reverse_lazy
from django.views import generic
from django.views.generic import edit

from guardian.mixins import LoginRequiredMixin, PermissionListMixin, PermissionRequiredMixin
from guardian.shortcuts import assign_perm, get_objects_for_user

from apps.models import App
from .models import Build

class DetailView(PermissionRequiredMixin, generic.DetailView):
    model = Build
    permission_required = 'view_build'

class CreateView(PermissionRequiredMixin, edit.CreateView):
    model = Build
    permission_required = 'builds.add_build'
    permission_object = None
    fields = ['owner', 'artifact', 'description']

    def get_context_data(self, **kwargs):
        context = super(CreateView, self).get_context_data(**kwargs)
        context['form'].fields['owner'].queryset = get_objects_for_user(self.request.user, 'view_app', App)
        return context

    def form_valid(self, form):
        resp = super().form_valid(form)
        assign_perm('view_build', self.request.user, self.object)
        assign_perm('change_build', self.request.user, self.object)
        assign_perm('delete_build', self.request.user, self.object)
        return resp

class UpdateView(PermissionRequiredMixin, edit.UpdateView):
    model = Build
    permission_required = 'change_build'
    # cannot allow user to change the build artifact after it's been uploaded
    fields = ['owner', 'description']

    def get_context_data(self, **kwargs):
        context = super(UpdateView, self).get_context_data(**kwargs)
        context['form'].fields['owner'].queryset = get_objects_for_user(self.request.user, 'view_app', App)
        return context

class DeleteView(PermissionRequiredMixin, edit.DeleteView):
    model = Build
    permission_required = 'delete_build'
    success_url = reverse_lazy('builds:list')
