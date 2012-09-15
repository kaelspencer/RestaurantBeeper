from django.core.exceptions import ObjectDoesNotExist
from django.views.generic import DetailView
from django.http import HttpResponse, HttpResponseNotFound
from .models import Visitor
import json

def retrieve_visitor(request, slug):
    try:
        visitor = Visitor.objects.get(key=slug)
        return HttpResponse(json.dumps(visitor.get_dict()), mimetype='application/json')
    except ObjectDoesNotExist:
        return HttpResponseNotFound()

def register_visitor(request, slug):
    try:
        visitor = Visitor.objects.get(key=slug)

        data = dict()
        data['poll_url'] = visitor.get_poll_url()
        data['registered'] = visitor.registered

        return HttpResponse(json.dumps(data), mimetype='application/json')
    except ObjectDoesNotExist:
        return HttpResponseNotFound()
