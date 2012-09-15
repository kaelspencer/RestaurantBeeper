from __future__ import absolute_import
from django.conf.urls.defaults import *
from . import views

urlpatterns = patterns('',
    url(r'^(?P<slug>[a-zA-Z0-9]{20})/$', views.get_visitor_json),
)
