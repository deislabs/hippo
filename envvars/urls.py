from django.urls import path

from . import views

app_name = 'envvars'
urlpatterns = [
    path('', views.ListView.as_view(), name='list'),
    path('new/', views.CreateView.as_view(), name='new'),
    path('<uuid:pk>/edit/', views.UpdateView.as_view(), name='update'),
    path('<uuid:pk>/delete/', views.DeleteView.as_view(), name='delete'),
]
