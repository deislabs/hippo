from django.contrib.auth.decorators import login_required
from django.urls import path

from . import views

app_name = 'apps'
urlpatterns = [
    path('', views.IndexView.as_view(), name='index'),
    path('new/', views.AppCreate.as_view(), name='new'),
    path('<uuid:pk>/', views.DetailView.as_view(), name='detail'),
    path('<uuid:pk>/edit/', views.AppUpdate.as_view(), name='update'),
    path('<uuid:pk>/delete/', views.AppDelete.as_view(), name='delete'),
]
