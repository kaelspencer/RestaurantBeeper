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

        private void PhoneApplicationPage_Unloaded(object sender, RoutedEventArgs e)
        {
            this.qrScanner.StopScanning();
        }

        private void toggleSwitch1_Checked(object sender, RoutedEventArgs e)
        {
            this.qrScanner.Visibility = Visibility.Visible;
            this.qrScanner.StartScanning();
        }

        private void toggleSwitch1_Unchecked(object sender, RoutedEventArgs e)
        {
            this.qrScanner.StopScanning();
            this.qrScanner.Visibility = Visibility.Collapsed;
        }
    }
}