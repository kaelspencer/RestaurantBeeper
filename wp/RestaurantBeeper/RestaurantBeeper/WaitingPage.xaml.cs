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

                // Try to load the image from internal storage
                BitmapImage bitmapImage = null;
                try
                {
                    // Try to load the image from internal storage in case we already have it
                    bitmapImage = InternalStorage.RetrieveImage(this.userSettings.RestaurantImageName);

                    if (bitmapImage != null)
                    {
                        ImageBrush imageBrush = new ImageBrush();
                        imageBrush.ImageSource = bitmapImage;
                        this.WaitingPanorama.Background = imageBrush;
                    }
                    else
                    {
                        throw new NullReferenceException();
                    }
                }
                catch (Exception ex)
                {
                    // Something went wrong while trying to retrieve the image from internal storage. 
                    // Try to download and save the image
                    try
                    {
                        DataRetriever.CallBackOnImageRetrieval ImageRetrieved = new DataRetriever.CallBackOnImageRetrieval(this.ImageReady);
                        DataRetriever.ImageRetrieved = ImageRetrieved;

                        DataRetriever.GetImageFile(this.userSettings.RestaurantImagePath);
                    }
                    catch (System.Exception ex2)
                    {
                        MessageBox.Show(ex2.Message);
                    }
                }

                failed = false;
            }
            catch (Exception)
            {
                MessageBox.Show("There was a problem loading your data... We're really sorry. Please try again.", "We goofed", MessageBoxButton.OK);
                failed = true;
            }
        }

        private void ImageReady(byte[] imageContents)
        {
            if (InternalStorage.SaveImage(this.userSettings.RestaurantImageName, imageContents))
            {
                BitmapImage bitmapImage = InternalStorage.RetrieveImage(this.userSettings.RestaurantImageName);
                ImageBrush imageBrush = new ImageBrush();
                imageBrush.ImageSource = bitmapImage;
                this.WaitingPanorama.Background = imageBrush;
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