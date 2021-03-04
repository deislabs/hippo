import random
import string

from .production import *

DEBUG = True

# If set to True, Django's normal exception handling of view functions
# will be suppressed, and exceptions will propagate upwards
# https://docs.djangoproject.com/en/3.1/ref/settings/#debug-propagate-exceptions
DEBUG_PROPAGATE_EXCEPTIONS = True

DATABASES['default']['NAME'] = "unittest-{}".format(''.join(
    random.choice(string.ascii_letters + string.digits) for _ in range(8)))

SECRET_KEY = 'CHANGEME_sapm$s%upvsw5l_zuy_&29rkywd^78ff(qi*#@&*^'
