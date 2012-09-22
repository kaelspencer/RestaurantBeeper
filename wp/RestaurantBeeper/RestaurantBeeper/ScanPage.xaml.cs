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

namespace RestaurantBeeper
{
    public partial class ScanPage : PhoneApplicationPage
    {
        public ScanPage()
        {
            InitializeComponent();
        }

        private void buttonDone_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(this.textBox1.Text))
            {
                // TODO: Add some validation here?
                string result = this.textBox1.Text;
                NavigationService.Navigate(new Uri("/MainPage.xaml?ManualCode=" + result, UriKind.Relative));
            }
            else
            {
                MessageBoxResult messageboxResult = MessageBox.Show("No code entered. Please enter a manual code or tap cancel to go back.", "Uh-oh", MessageBoxButton.OKCancel);
                if (messageboxResult == MessageBoxResult.Cancel)
                {
                    NavigationService.GoBack();
                }
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
            DataRetriever.CodeRetrieved(e.Result);
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
                this.ScanPivot.SelectedItem = this.ScanPivot.Items[1];
            }
        }

        private void buttonWaiting_Click(object sender, RoutedEventArgs e)
        {
            DataRetriever.RegisterUser();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //this.InitWaitingPano();
            //SetDefaultPanoPage(PanoPages.Wait);
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //InternalStorage.EmptyIsolatedStorage();
                //this.SetDefaultPanoPage(PanoPages.Reserve);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
            userSettings.RestaurantImagePath = new Uri("http://www.sattestpreptips.com/wp-content/plugins/sociable/buffalo-wild-wings-sauces-buy-747.jpg", UriKind.Absolute);
            //userSettings.RestaurantImagePath = new Uri("http://wac.450f.edgecastcdn.net/80450F/103gbfrocks.com/files/2011/11/Buffalo-Wild-Wings-wings.jpg", UriKind.Absolute);
            userSettings.RestaurantImageName = "Image2.jpg";

            InternalStorage.SaveToIsolatedStorage("UserSettings", userSettings);
            InternalStorage.CommitToIsolatedStorage();
        }
    }
}