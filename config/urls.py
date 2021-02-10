from django.urls import path

from . import views

app_name = 'config'
urlpatterns = [
    path('', views.index, name='index'),
    path('envvars/add/', views.add_envvar, name='add_envvar'),
    path('functions/add/', views.add_function, name='add_function'),
]
