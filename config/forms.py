from django.forms import ModelForm
from .models import EnvironmentVariable, Function

class EnvironmentVariableForm(ModelForm):

    class Meta:
        model = EnvironmentVariable
        fields = ['app', 'key', 'value']

class FunctionForm(ModelForm):

    class Meta:
        model = Function
        fields = ['app', 'name', 'args']
