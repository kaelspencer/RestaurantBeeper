from django.core.exceptions import ObjectDoesNotExist
from django.views.generic import DetailView
from django.http import HttpResponse, HttpResponseNotFound
from .models import Visitor
import json

def get_visitor_json(request, slug):
    try:
        visitor = Visitor.objects.get(key=slug)
        return HttpResponse(json.dumps(visitor.get_dict()), mimetype='application/json')
    except ObjectDoesNotExist:
        return HttpResponseNotFound()
