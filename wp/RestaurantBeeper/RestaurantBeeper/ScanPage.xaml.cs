using System;
using System.Windows;
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
            MessageBox.Show("There was a problem accessing the camera. Please try again. If that does not work, please close and re-open the app. " + e.Exception.Message, "Hmm...", MessageBoxButton.OK);
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
            NavigationService.Navigate(new Uri("/MainPage.xaml?ReadyToRegister=true", UriKind.Relative));
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

        private void ScanPivot_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Sometimes switching the pivot while the camera is being used causes issues (it also slows down the system).
            // If scanning, disable the QRScanner control while it's off the screen. When it comes back into view, re-enable it.
            if (this.qrScanner.IsScanning && e.AddedItems.Count > 0 && e.AddedItems[0] == this.ManualPivotItem)
            {
                this.qrScanner.StopScanning();
            }
            else if(this.toggleSwitch1.IsChecked.Value)
            {
                this.qrScanner.StartScanning();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.QRCodeScanner_ScanComplete(this, new JeffWilcox.Controls.ScanCompleteEventArgs("http://restaurant.kaelspencer.com/register/jlCOyvDERtgJrScoJ73D/"));
            buttonWaiting_Click(this, null);
        }
    }
}
