from django.urls import reverse_lazy
from django.views import generic
from django.views.generic import edit

from guardian.mixins import LoginRequiredMixin, PermissionListMixin, PermissionRequiredMixin

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

class DeleteView(PermissionRequiredMixin, edit.DeleteView):
    permission_required = 'delete_release'
    model = Release
    success_url = reverse_lazy('releases:list')
