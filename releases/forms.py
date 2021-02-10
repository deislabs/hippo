from django.forms import ModelForm, FileField
from .models import Release

class ReleaseForm(ModelForm):

    class Meta:
        model = Release
        fields = ['owner', 'build', 'description']
