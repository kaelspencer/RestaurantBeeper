from django.shortcuts import render_to_response, RequestContext
from django.contrib.auth.decorators import login_required

@login_required(login_url='/login/')
def default_view(request):
    return render_to_response('home.html', context_instance=RequestContext(request))
