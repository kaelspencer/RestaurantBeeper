from __future__ import absolute_import
from django.conf.urls.defaults import *
from . import views

urlpatterns = patterns('',
    url(r'^cancel/(?P<slug>[a-zA-Z0-9]{20})/$', views.cancel),
    url(r'^delay/(?P<slug>[a-zA-Z0-9]{20})/$', views.delay),
    url(r'^register/(?P<slug>[a-zA-Z0-9]{20})/$', views.register_visitor),
    url(r'^reserve/$', views.reservation_new),
    url(r'^reserve/(?P<slug>[a-zA-Z0-9]{20})/$', views.reservation_view),
    url(r'^restaurant/(?P<slug>[a-zA-Z0-9]{20})/$', views.restaurant_view),
    url(r'^retrieve/(?P<slug>[a-zA-Z0-9]{20})/$', views.retrieve_visitor),
)
