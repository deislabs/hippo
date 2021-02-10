from django.forms import ModelForm
from .models import Domain

class DomainForm(ModelForm):

    class Meta:
        model = Domain
        fields = ['app', 'domain']
