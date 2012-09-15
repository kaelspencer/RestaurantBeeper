using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Media.Imaging;

namespace RestaurantBeeper
{
    public partial class WaitingPage : PhoneApplicationPage
    {
        public WaitingPage()
        {
            InitializeComponent();

            BitmapImage bitmapImage = new BitmapImage(new Uri("http://www.sattestpreptips.com/wp-content/plugins/sociable/buffalo-wild-wings-sauces-buy-747.jpg", UriKind.Absolute));
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = bitmapImage;
            this.WaitingPanorama.Background = imageBrush;

            this.textBlockRestaurantName.Text = UserSettings.RestaurantName;
            this.testBlockGuestName.Text = UserSettings.GuestName;
            this.textBlockNumberOfGuests.Text = UserSettings.NumberOfGuests.ToString();
            this.textBlockTimeToWait.Text = UserSettings.LastTimeToWait + "minutes";
            
            if (UserSettings.StartTimeToWait > 0 && UserSettings.LastTimeToWait > 0)
            {
                this.progressBarWaitTime.IsIndeterminate = false;
                this.progressBarWaitTime.Value = (((float)UserSettings.StartTimeToWait - (float)UserSettings.LastTimeToWait) / (float)UserSettings.StartTimeToWait) * 100;
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            ClearBackStack();
        }

        private void ClearBackStack()
        {
            // While there are pages on the backstack, clear them out.
            while (NavigationService.CanGoBack)
            {
                NavigationService.RemoveBackEntry();
            }
        }
    }
}