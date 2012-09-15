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
using Microsoft.Phone.Shell;

namespace RestaurantBeeper
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);

            //ApplicationBar = new ApplicationBar();
            //ApplicationBar.Mode = ApplicationBarMode.Default;
            //ApplicationBar.IsVisible = true;
            //ApplicationBar.IsMenuEnabled = true;

            //ApplicationBarIconButton button1 = new ApplicationBarIconButton();
            //button1.IconUri = new Uri("/Images/appbar.add.rest.png", UriKind.Relative);
            //button1.Text = "New";
            //ApplicationBar.Buttons.Add(button1);

            this.qrScanner.Visibility = Visibility.Collapsed;
            this.qrScanner.StopScanning();
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
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
            CodeRetrieved(e.Result);
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
                CodeRetrieved(result);

                // While there are pages on the backstack, clear them out.
                while (NavigationService.CanGoBack)
                {
                    NavigationService.RemoveBackEntry();
                }
            }
        }

        private void CodeRetrieved(string code)
        {
            this.textBlockResult.Text = code;
        }
    }
}