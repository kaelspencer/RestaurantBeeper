using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows;
using Microsoft.Phone.Controls;
using System.IO.IsolatedStorage;

namespace RestaurantBeeper
{
    public partial class MainPage : PhoneApplicationPage
    {
        private bool firstLoad;
        private bool objectsHidden;
        private bool skipToWaitingPage;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            this.qrScanner.Visibility = Visibility.Collapsed;
            this.qrScanner.StopScanning();
            
            firstLoad = true;

            if (this.TryLoadSettings())
            {
                this.HideObjects();
                this.skipToWaitingPage = true;
            }
        }

        private void HideObjects()
        {
            this.objectsHidden = true;

            this.LayoutRoot.Visibility = Visibility.Collapsed;
            this.TitlePanel.Visibility = Visibility.Collapsed;
            this.ApplicationTitle.Visibility = Visibility.Collapsed;
            this.PageTitle.Visibility = Visibility.Collapsed;
            this.ContentPanel.Visibility = Visibility.Collapsed;
            this.qrButton.Visibility = Visibility.Collapsed;
            this.qrScanner.Visibility = Visibility.Collapsed;
            this.textBlockResult.Visibility = Visibility.Collapsed;
            this.hyperlinkButton1.Visibility = Visibility.Collapsed;
            this.buttonWaiting.Visibility = Visibility.Collapsed;
            this.button1.Visibility = Visibility.Collapsed;
        }

        private void ShowObjects()
        {
            this.objectsHidden = false;

            this.LayoutRoot.Visibility = Visibility.Visible;
            this.TitlePanel.Visibility = Visibility.Visible;
            this.ApplicationTitle.Visibility = Visibility.Visible;
            this.PageTitle.Visibility = Visibility.Visible;
            this.ContentPanel.Visibility = Visibility.Visible;
            this.qrButton.Visibility = Visibility.Visible;
            this.qrScanner.Visibility = Visibility.Visible;
            this.textBlockResult.Visibility = Visibility.Visible;
            this.hyperlinkButton1.Visibility = Visibility.Visible;
            this.buttonWaiting.Visibility = Visibility.Visible;
            this.button1.Visibility = Visibility.Visible;
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

        private void qrButton_Click(object sender, RoutedEventArgs e)
        {
            switch (this.qrScanner.Visibility)
            {
                case Visibility.Collapsed:
                    // The control was hidden, make it visible and start
                    this.qrScanner.Visibility = Visibility.Visible;
                    this.qrScanner.StartScanning();
                    break;
                case Visibility.Visible:
                    // The control was already visible. Treating as toggle.
                    this.qrScanner.StopScanning();
                    this.qrScanner.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void QRCodeScanner_ScanComplete(object sender, JeffWilcox.Controls.ScanCompleteEventArgs e)
        {
            this.CodeRetrieved(e.Result);
            this.qrScanner.StopScanning();
            this.qrScanner.StartScanning();

            this.buttonWaiting.IsEnabled = true;
            this.buttonWaiting.Content = "Ready. Tap to continue.";
        }

        private void QRCodeScanner_Error(object sender, JeffWilcox.Controls.ScanFailureEventArgs e)
        {
            MessageBox.Show("There was a problem accessing the camera. Please try again. If that does not work, please close and re-open the app.", "Hmm...", MessageBoxButton.OK);
        }

        private void hyperlinkButton1_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageboxResult = MessageBox.Show("If you're having trouble scanning, we can try to enter a code manually.", "Having trouble?", MessageBoxButton.OKCancel);

            // If the user opted to continue with manual code entry, stop the QR scanner and change pages to the manual code entry page
            if (messageboxResult == MessageBoxResult.OK)
            {
                this.qrScanner.StopScanning();
                this.qrScanner.Visibility = Visibility.Collapsed;
                NavigationService.Navigate(new Uri("/ManualCodePage.xaml", UriKind.Relative));
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string result = "";

            // Check to see if the page is navigated to from another page (e.g. the manual code entry page)
            if (NavigationContext.QueryString.TryGetValue("ManualCode", out result))
            {
                this.CodeRetrieved(result);

                // While there are pages on the backstack, clear them out.
                while (NavigationService.CanGoBack)
                {
                    NavigationService.RemoveBackEntry();
                }
            }
        }

        private void CodeRetrieved(string result)
        {
            this.textBlockResult.Text = result;
            UserURLs.RegistrationUri = new Uri(result);

            UriBuilder uriBuilder = new UriBuilder(result);
            if (!String.IsNullOrEmpty(uriBuilder.Query))
            {
                uriBuilder.Query = String.Empty;
            }

            if (!String.IsNullOrEmpty(uriBuilder.Path))
            {
                uriBuilder.Path = String.Empty;
            }

            UserURLs.HostUri = uriBuilder.Uri;
        }

        private void buttonWaiting_Click(object sender, RoutedEventArgs e)
        {
            DataRetriever.RegisterUser();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {            
            NavigationService.Navigate(new Uri("/WaitingPage.xaml", UriKind.Relative));
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            // If this isn't the first time the page has been loaded, then the app is being redirected to it.
            // The objects should be unhidden if this is the case.
            // TODO: Use params here when navigating back?
            if (!firstLoad && objectsHidden)
            {
                this.ShowObjects();
            }
            this.firstLoad = false;

            if (this.skipToWaitingPage)
            {
                this.skipToWaitingPage = false;
                NavigationService.Navigate(new Uri("/WaitingPage.xaml", UriKind.Relative));
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            InternalStorage.EmptyIsolatedStorage();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            UserSettings userSettings = new UserSettings();
            userSettings.IsWaiting = true;
            userSettings.UserKey = "ABC";
            userSettings.RestaurantName = "BuffaloWildWings";
            userSettings.GuestName = "Jimbo";
            userSettings.NumberOfGuests = 3;
            userSettings.HostUri = UserURLs.HostUri;
            userSettings.RegistrationUri = UserURLs.RegistrationUri;
            userSettings.RetrievalUri = UserURLs.RetrievalUri;
            userSettings.StartTimeToWait = 40;
            userSettings.LastTimeToWait = 20;
            userSettings.TimeStarted = DateTime.Now;
            userSettings.TimeLastChecked = DateTime.Now;
            userSettings.TimeExpected = DateTime.Now.AddMinutes(userSettings.LastTimeToWait);

            InternalStorage.SaveToIsolatedStorage("UserSettings", userSettings);
            InternalStorage.CommitToIsolatedStorage();
        }
    }
}