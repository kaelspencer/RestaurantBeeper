using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows;
using Microsoft.Phone.Controls;
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.ComponentModel;
using JeffWilcox.Controls;

namespace RestaurantBeeper
{
    public partial class MainPage : PhoneApplicationPage
    {
        private bool firstLoad;
        private bool objectsHidden;
        private bool skipToWaitingPage;
        private enum PanoPages { Reserve = 0, Wait, Action, Offers, Info, Help}

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            if (this.TryLoadSettings())
            {
                //this.InitWaitingPano();
                //SetDefaultPanoPage(PanoPages.Wait);
            }
        }

        private bool TryLoadSettings()
        {
            try
            {
                UserSettings userSettings = InternalStorage.LoadFromIsolatedStorage<UserSettings>("UserSettings");
                return userSettings.IsWaiting;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void SetDefaultPanoPage(PanoPages page)
        {
            this.MainPano.DefaultItem = this.MainPano.Items[(int)page];
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string result = "";

            // Check to see if the page is navigated to from another page (e.g. the manual code entry page)
            if (NavigationContext.QueryString.TryGetValue("ManualCode", out result))
            {
                DataRetriever.CodeRetrieved(result);

                // Clear out the backstack
                this.ClearBackStack();
            }
        }

        // WAITING PAGE LOGIC
        private bool failed;
        public UserSettings userSettings { get; set; }
        private ImageBrush imageBrush;

        public void InitWaitingPano()
        {
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

                // TODO: Loading a background image into the pano causes the app to chug... Figure out why and how to fix it.
                LoadImage();

                failed = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was a problem loading your data... We're really sorry. Please try again. " + ex.Message, "We goofed", MessageBoxButton.OK);
                failed = true;
            }
        }

        private void LoadImage ()
        {
            // Try to load the image from internal storage
            BitmapImage bitmapImage = null;
            try
            {
                // Try to load the image from internal storage in case we already have it
                bitmapImage = InternalStorage.RetrieveImage(this.userSettings.RestaurantImageName);

                if (bitmapImage != null)
                {
                    this.imageBrush = new ImageBrush();
                    this.imageBrush.ImageSource = bitmapImage;
                    SetPanoBackground();
                }
                else
                {
                    throw new NullReferenceException();
                }
            }
            catch (Exception)
            {
                // Something went wrong while trying to retrieve the image from internal storage. 
                // Try to download and save the image
                try
                {
                    DataRetriever.CallBackOnImageRetrieval imageRetrieved = new DataRetriever.CallBackOnImageRetrieval(this.ImageReady);
                    DataRetriever.ImageRetrieved = imageRetrieved;

                    DataRetriever.GetImageFile(this.userSettings.RestaurantImagePath);
                }
                catch (System.Exception ex2)
                {
                    MessageBox.Show(ex2.Message);
                }
            }
        }

        private void ImageReady(byte[] imageContents)
        {
            if (InternalStorage.SaveImage(this.userSettings.RestaurantImageName, imageContents))
            {
                BitmapImage bitmapImage = InternalStorage.RetrieveImage(this.userSettings.RestaurantImageName);
                this.imageBrush = new ImageBrush();
                this.imageBrush.ImageSource = bitmapImage;
                SetPanoBackground();
            }            
        }

        private void SetPanoBackground ()
        {
            this.MainPano.Background = this.imageBrush;
        }

        private void ClearBackStack()
        {
            // While there are pages on the backstack, clear them out.
            while (NavigationService.CanGoBack)
            {
                NavigationService.RemoveBackEntry();
            }
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/ScanPage.xaml", UriKind.Relative));
        }
    }
}