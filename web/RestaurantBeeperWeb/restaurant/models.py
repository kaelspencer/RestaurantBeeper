from django.db import models
from django.contrib.auth.models import User
import random, string

class Restaurant(models.Model):
    user = models.OneToOneField(User)
    name = models.CharField(max_length=200)

    class Meta:
        verbose_name_plural = 'restaurants'
        ordering = ('name',)

    def __unicode__(self):
        return self.name

class Visitor(models.Model):
    restaurant = models.ForeignKey(Restaurant)
    name = models.CharField(max_length=200)
    guests = models.IntegerField()
    time_to_wait = models.IntegerField()
    key = models.CharField(max_length=20, primary_key=True)
    registered = models.BooleanField(default=False)
    push_enabled = models.BooleanField(default=False)

    class Meta:
        verbose_name_plural = 'visitors'
        ordering = ('restaurant',)

    def __unicode__(self):
        return self.name + ' (' + str(self.guests) + ' guests)'

    def save(self, *args, **kwargs):
        self.key = ''.join(random.choice(string.ascii_letters + string.digits) for x in range(20))
        super(Visitor, self).save(*args, **kwargs)

    def get_dict(self):
        data = dict()
        data['restaurant'] = str(self.restaurant)
        data['name'] = self.name
        data['guests'] = self.guests
        data['time_to_wait'] = self.time_to_wait
        data['key'] = self.key

        # TODO Kael: This is for testing only.
        data['registered'] = self.registered
        data['push_enabled'] = self.push_enabled

        return data

    def get_poll_url(self):
        return '/retrieve/' + self.key + '/'
