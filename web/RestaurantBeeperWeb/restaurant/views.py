from django.core.exceptions import ObjectDoesNotExist
from django.views.generic import DetailView
from django.http import HttpResponse, HttpResponseNotFound, HttpResponseRedirect
from django.shortcuts import render_to_response, RequestContext
from django.template.defaultfilters import stringfilter
from django.core.context_processors import csrf
from django.forms import ModelForm
import urllib
from .models import Visitor, Restaurant
import json

def retrieve_visitor(request, slug):
    try:
        visitor = Visitor.objects.get(key=slug)

        data = dict()

        if visitor.registered:
            data['status'] = 'ok'
            data['code'] = 0
            data['message'] = ''
            data['data'] = visitor.get_dict()
        else:
            data = dict()
            data['status'] = 'bad'
            data['code'] = 1
            data['message'] = 'Visitor is not registered'
            data['data'] = dict()

        return HttpResponse(json.dumps(data), mimetype='application/json')
    except ObjectDoesNotExist:
        return HttpResponseNotFound()

def register_visitor(request, slug):
    try:
        visitor = Visitor.objects.get(key=slug)
        visitor.register()

        data = dict()
        data['poll_url'] = visitor.get_poll_url()
        data['delay_url'] = visitor.get_delay_url()

        return HttpResponse(json.dumps(data), mimetype='application/json')
    except ObjectDoesNotExist:
        return HttpResponseNotFound()

class NewReservationForm(ModelForm):
    class Meta:
        model = Visitor
        exclude = ('restaurant', 'key', 'registered', 'push_enabled')

    def save(self, *args, **kwargs):
        kwargs['commit']=False
        obj = super(NewReservationForm, self).save(*args, **kwargs)

        if self.request:
            restaurant = Restaurant.objects.get(user=self.request.user)
            obj.restaurant = restaurant

        obj.save()
        self.key = obj.key
        return self

def reservation_new(request):
    if request.method == 'POST':
        form = NewReservationForm(request.POST)
        form.request = request

        if form.is_valid():
            visitor = form.save()

            return HttpResponseRedirect('/reserve/' + visitor.key + '/')
    else:
        form = NewReservationForm()

    context = {'form': form}
    context.update(csrf(request))

    return render_to_response('restaurant/reservation.new.html', context, context_instance=RequestContext(request))

def reservation_view(request, slug):
    context = {'qrcode_url': qrcode('http://descartes:8000/register/' + slug)}

    return render_to_response('restaurant/reservation.view.html', context)

@stringfilter
def qrcode(value):
    return "http://chart.apis.google.com/chart?" + urllib.urlencode({'chs':'500x500', 'cht':'qr', 'chl':value})

def delay(request, slug):
    try:
        visitor = Visitor.objects.get(key=slug)
        visitor.delay()

        return retrieve_visitor(request, slug)
    except ObjectDoesNotExist:
        return HttpResponseNotFound()
