from django.urls import path

from . import views

urlpatterns = [
    path('<str:release_pk>/invoke/', views.invoke, name='invoke'),
]
