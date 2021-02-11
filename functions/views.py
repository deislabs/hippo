from django.urls import reverse_lazy
from django.views import generic
from django.views.generic import edit

from guardian.mixins import LoginRequiredMixin, PermissionListMixin, PermissionRequiredMixin
from guardian.shortcuts import assign_perm, get_objects_for_user

from apps.models import App
from .models import Function

class ListView(PermissionListMixin, LoginRequiredMixin, generic.ListView):
    model = Function
    permission_required = 'view_function'
    context_object_name = 'functions'

    def get_queryset(self):
        queryset = super().get_queryset()
        app = self.request.GET.get('app')
        if app is not None:
            queryset = queryset.objects.filter(owner=self.request.GET.get('app'))
        return queryset

class CreateView(PermissionRequiredMixin, edit.CreateView):
    model = Function
    permission_required = 'functions.add_function'
    permission_object = None
    fields = ['owner', 'name', 'args']

    def get_context_data(self, **kwargs):
        context = super(CreateView, self).get_context_data(**kwargs)
        context['form'].fields['owner'].queryset = get_objects_for_user(self.request.user, 'view_app', App)
        return context

    def form_valid(self, form):
        resp = super().form_valid(form)
        assign_perm('view_function', self.request.user, self.object)
        assign_perm('change_function', self.request.user, self.object)
        assign_perm('delete_function', self.request.user, self.object)
        return resp

class UpdateView(PermissionRequiredMixin, edit.UpdateView):
    permission_required = 'change_function'
    model = Function
    fields = ['name', 'args']

class DeleteView(PermissionRequiredMixin, edit.DeleteView):
    permission_required = 'delete_function'
    model = Function
    success_url = reverse_lazy('functions:list')
