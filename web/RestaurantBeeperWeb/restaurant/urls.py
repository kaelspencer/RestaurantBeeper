from __future__ import absolute_import
from django.conf.urls.defaults import *
from django.views.generic.base import TemplateView
from . import views

urlpatterns = patterns('',
    url(r'^register/$', TemplateView.as_view(template_name='restaurant/register.html')),
    url(r'^register/(?P<slug>[a-zA-Z0-9]{20})/$', views.register_visitor),
    url(r'^retrieve/(?P<slug>[a-zA-Z0-9]{20})/$', views.retrieve_visitor),
)
