from django.db import models
from django.contrib.auth.models import User

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

    class Meta:
        verbose_name_plural = 'visitors'
        ordering = ('restaurant',)

    def __unicode__(self):
        return self.name + ' (' + str(self.guests) + ' guests)'
