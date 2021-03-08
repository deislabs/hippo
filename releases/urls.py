from django.urls import path

from . import views

app_name = 'releases'
urlpatterns = [
    path('', views.ListView.as_view(), name='list'),
    path('<uuid:pk>/', views.DetailView.as_view(), name='detail'),
    path('<uuid:pk>/delete/', views.DeleteView.as_view(), name='delete'),
]
