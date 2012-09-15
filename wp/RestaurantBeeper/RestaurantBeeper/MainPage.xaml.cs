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
            switch(this.qrScanner.Visibility)
            {
                case Visibility.Collapsed:
                    // The control was hidden, make it visible and start
                    this.qrScanner.Visibility = Visibility.Visible;
                    this.qrScanner.StartScanning();
                    break;
                case Visibility.Visible:
                    this.qrScanner.StopScanning();
                    this.qrScanner.Visibility = Visibility.Collapsed;
                    // The control was already visible... Do something?
                    break;
            }
        }

        private void QRCodeScanner_ScanComplete(object sender, JeffWilcox.Controls.ScanCompleteEventArgs e)
        {
            this.textBlockResult.Text = e.Result;
            this.qrScanner.StopScanning();
            this.qrScanner.StartScanning();
        }

        private void QRCodeScanner_Error(object sender, JeffWilcox.Controls.ScanFailureEventArgs e)
        {
            MessageBox.Show("GTFO");
        }
    }
}