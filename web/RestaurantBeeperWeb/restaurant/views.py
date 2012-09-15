from django.core.exceptions import ObjectDoesNotExist
from django.views.generic import DetailView
from django.http import HttpResponse, HttpResponseNotFound
from .models import Visitor
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

        return HttpResponse(json.dumps(data), mimetype='application/json')
    except ObjectDoesNotExist:
        return HttpResponseNotFound()
