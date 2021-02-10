from django.contrib.auth.mixins import LoginRequiredMixin
from django.urls import reverse_lazy
from django.views import generic
from django.views.generic import edit

from .models import App

class IndexView(generic.ListView, LoginRequiredMixin):
    context_object_name = 'apps'

    def get_queryset(self):
        """Return all applications owned by the logged-in user."""
        return App.objects.filter(owner=self.request.user)

class DetailView(generic.DetailView, LoginRequiredMixin):
    model = App

class AppCreate(edit.CreateView, LoginRequiredMixin):
    model = App
    fields = ['name']

    def form_valid(self, form):
        form.instance.owner = self.request.user
        return super().form_valid(form)

class AppUpdate(edit.UpdateView, LoginRequiredMixin):
    model = App
    fields = ['name']

class AppDelete(edit.DeleteView, LoginRequiredMixin):
    model = App
    success_url = reverse_lazy('apps:index')
