from django.conf.urls import patterns, include, url
from django.contrib import admin
from django.conf import settings
from . import views

admin.autodiscover()

urlpatterns = patterns('',
    url(r'^admin/doc/', include('django.contrib.admindocs.urls')),
    url(r'^admin/', include(admin.site.urls)),
    url(r'^login/', 'django.contrib.auth.views.login', {'template_name': 'auth/login.html'}),
    url(r'^$', views.default_view),
    url(r'^', include('restaurant.urls')),
)

if settings.DEBUG:
    from django.views.static import serve
    media_url = settings.MEDIA_URL

    if media_url.startswith('/'):
        media_url = media_url[1:]
        urlpatterns += patterns('', (r'^%s(?P<path>.*)$' % media_url, serve, {'document_root': settings.MEDIA_ROOT}))

    del(media_url, serve)
