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
        private bool failed;
        public UserSettings userSettings { get; set; }

        public WaitingPage()
        {
            InitializeComponent();

            //TODO: Save the image to isolated storage. This requires saving as a file as a BitmapImage is not serializable
            BitmapImage bitmapImage = new BitmapImage(new Uri("http://www.sattestpreptips.com/wp-content/plugins/sociable/buffalo-wild-wings-sauces-buy-747.jpg", UriKind.Absolute));
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = bitmapImage;
            this.WaitingPanorama.Background = imageBrush;

            try
            {
                this.userSettings = InternalStorage.LoadFromIsolatedStorage<UserSettings>("UserSettings");
                this.textBlockRestaurantName.Text = userSettings.RestaurantName;
                this.testBlockGuestName.Text = userSettings.GuestName;
                this.textBlockNumberOfGuests.Text = userSettings.NumberOfGuests.ToString();
                this.textBlockTimeToWait.Text = userSettings.LastTimeToWait + "minutes";

                if (userSettings.StartTimeToWait > 0 && userSettings.LastTimeToWait > 0)
                {
                    this.progressBarWaitTime.IsIndeterminate = false;
                    this.progressBarWaitTime.Value = (((float)userSettings.StartTimeToWait - (float)userSettings.LastTimeToWait) / (float)userSettings.StartTimeToWait) * 100;
                }

                failed = false;

                InternalStorage.SaveToIsolatedStorage("Background", bitmapImage);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("There was a problem loading your data... We're really sorry. Please try again.", "We goofed", MessageBoxButton.OK);
                failed = true;
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!failed)
            {
                ClearBackStack();
            }
            else
            {
                NavigationService.GoBack();
            }
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