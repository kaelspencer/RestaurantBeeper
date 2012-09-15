from django.db import models
from django.contrib.auth.models import User
import random, string

class Restaurant(models.Model):
    user = models.OneToOneField(User)
    name = models.CharField(max_length=200)
    background_image_url = models.URLField(null=True)
    primary_color = models.CharField(max_length=7, null=True)
    secondary_color = models.CharField(max_length=7, null=True)

    class Meta:
        verbose_name_plural = 'restaurants'
        ordering = ('name',)

    def __unicode__(self):
        return self.name

    def get_data(self):
        data = dict()
        data['name'] = self.name
        data['background_image_url'] = self.background_image_url
        data['primary_color'] = self.primary_color
        data['secondary_color'] = self.secondary_color

        return data

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

    def register(self):
        if not self.registered:
            self.registered = True
            super(Visitor, self).save()

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
        return 'retrieve/' + self.key + '/'

    def get_delay_url(self):
        return 'delay/' + self.key + '/'

    def get_cancel_url(self):
        return 'cancel/' + self.key + '/'

    def get_restaurant_url(self):
        return 'restaurant/' + self.key + '/'

    def delay(self):
        self.time_to_wait = self.time_to_wait + 5
        super(Visitor, self).save()
