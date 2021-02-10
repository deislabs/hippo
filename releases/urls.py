from django.urls import path

from . import views

app_name = 'releases'
urlpatterns = [
    path('<str:release_pk>/invoke/', views.invoke, name='invoke'),
    path('add/', views.add, name='add')
]
