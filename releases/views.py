from django.urls import reverse_lazy
from django.views import generic
from django.views.generic import edit

from guardian.mixins import LoginRequiredMixin, PermissionListMixin, PermissionRequiredMixin
from guardian.shortcuts import assign_perm, get_objects_for_user

from apps.models import App
from .models import Release

class ListView(PermissionListMixin, LoginRequiredMixin, generic.ListView):
    model = Release
    permission_required = 'view_release'
    context_object_name = 'releases'

    def get_queryset(self):
        queryset = super().get_queryset()
        app = self.request.GET.get('app')
        if app is not None:
            queryset = queryset.filter(owner=self.request.GET.get('app'))
        return queryset

class DetailView(PermissionRequiredMixin, generic.DetailView):
    permission_required = 'view_release'
    model = Release

class CreateView(PermissionRequiredMixin, edit.CreateView):
    model = Release
    permission_required = 'releases.add_release'
    permission_object = None
    fields = ['owner', 'build']

    def get_context_data(self, **kwargs):
        context = super(CreateView, self).get_context_data(**kwargs)
        context['form'].fields['owner'].queryset = get_objects_for_user(self.request.user, 'view_app', App)
        return context

    def form_valid(self, form):
        resp = super().form_valid(form)
        assign_perm('view_release', self.request.user, self.object)
        assign_perm('delete_release', self.request.user, self.object)
        return resp

class DeleteView(PermissionRequiredMixin, edit.DeleteView):
    permission_required = 'delete_release'
    model = Release
    success_url = reverse_lazy('releases:list')
