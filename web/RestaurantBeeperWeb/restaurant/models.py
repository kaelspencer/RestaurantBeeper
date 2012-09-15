from django.db import models
from django.contrib.auth.models import User

class Restaurant(models.Model):
    user = models.OneToOneField(User)
    name = models.CharField(max_length=200)

    class Meta:
        verbose_name_plural = "restaurants"
        ordering = ('name',)

    def __unicode__(self):
        return self.name
