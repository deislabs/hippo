from django.urls import reverse_lazy
from django.views import generic
from django.views.generic import edit

from guardian.mixins import LoginRequiredMixin, PermissionListMixin, PermissionRequiredMixin
from guardian.shortcuts import assign_perm, get_objects_for_user

from apps.models import App
from .models import Domain

class ListView(LoginRequiredMixin, generic.ListView):
    model = Domain
    context_object_name = 'domains'
    permission_required = 'view_domain'

class DetailView(PermissionRequiredMixin, generic.DetailView):
    model = Domain
    permission_required = 'view_domain'

class CreateView(PermissionRequiredMixin, edit.CreateView):
    model = Domain
    permission_required = 'domains.add_domain'
    permission_object = None
    fields = ['owner', 'domain']

    def get_context_data(self, **kwargs):
        context = super(CreateView, self).get_context_data(**kwargs)
        context['form'].fields['owner'].queryset = get_objects_for_user(self.request.user, 'view_app', App)
        return context

    def form_valid(self, form):
        resp = super().form_valid(form)
        assign_perm('view_domain', self.request.user, self.object)
        assign_perm('change_domain', self.request.user, self.object)
        assign_perm('delete_domain', self.request.user, self.object)
        return resp

class UpdateView(PermissionRequiredMixin, edit.UpdateView):
    model = Domain
    permission_required = 'change_domain'
    fields = ['owner', 'domain']

    def get_context_data(self, **kwargs):
        context = super(UpdateView, self).get_context_data(**kwargs)
        context['form'].fields['owner'].queryset = get_objects_for_user(self.request.user, 'view_app', App)
        return context

class DeleteView(PermissionRequiredMixin, edit.DeleteView):
    model = Domain
    permission_required = 'delete_domain'
    success_url = reverse_lazy('domains:list')
