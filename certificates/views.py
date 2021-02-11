from django.urls import reverse_lazy
from django.views import generic
from django.views.generic import edit

from guardian.mixins import LoginRequiredMixin, PermissionListMixin, PermissionRequiredMixin
from guardian.shortcuts import assign_perm, get_objects_for_user

from domains.models import Domain
from .models import Certificate

class ListView(PermissionListMixin, LoginRequiredMixin, generic.ListView):
    model = Certificate
    permission_required = 'view_certificate'
    context_object_name = 'certificates'

    def get_queryset(self):
        queryset = super().get_queryset()
        domain = self.request.GET.get('domain')
        if domain is not None:
            queryset = queryset.filter(owner=domain)
        return queryset

class DetailView(PermissionRequiredMixin, generic.DetailView):
    permission_required = 'view_certificate'
    model = Certificate

class CreateView(PermissionRequiredMixin, edit.CreateView):
    model = Certificate
    permission_required = 'certificates.add_certificate'
    permission_object = None
    fields = ['owner', 'certificate', 'key']

    def get_context_data(self, **kwargs):
        context = super(CreateView, self).get_context_data(**kwargs)
        context['form'].fields['owner'].queryset = get_objects_for_user(self.request.user, 'view_domain', Domain)
        return context

    def form_valid(self, form):
        resp = super().form_valid(form)
        assign_perm('view_certificate', self.request.user, self.object)
        assign_perm('change_certificate', self.request.user, self.object)
        assign_perm('delete_certificate', self.request.user, self.object)
        return resp

class UpdateView(PermissionRequiredMixin, edit.UpdateView):
    permission_required = 'change_certificate'
    model = Certificate
    fields = ['certificate', 'key']

class DeleteView(PermissionRequiredMixin, edit.DeleteView):
    permission_required = 'delete_certificate'
    model = Certificate
    success_url = reverse_lazy('certificates:list')
