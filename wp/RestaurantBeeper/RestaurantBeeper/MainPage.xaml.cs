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
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace RestaurantBeeper
{
    public class CodeData
    {
        public int time_to_wait { get; set; }
        public int guests { get; set; }
        public string name { get; set; }
        public string key { get; set; }
        public string restaurant { get; set; }
    }

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

            this.buttonWaiting.IsEnabled = true;
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

        private void buttonWaiting_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Put this in a better place
            DownloadStringCompleted(null, null);
            return;
            WebClient webClient = new WebClient();
            webClient.DownloadStringAsync(new Uri(@"http://descartes:8000/get/IL0LQO9ipGNEisHHmhhz/"));
            webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(DownloadStringCompleted);
        }

        void DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                //string result = e.Result;
                string result = "{\"time_to_wait\": 45, \"guests\": 1, \"name\": \"Goose\", \"key\": \"IL0LQO9ipGNEisHHmhhz\", \"restaurant\": \"Boom Noodle\"}";

                // convert string to stream
                byte[] byteArray = Encoding.UTF8.GetBytes(result);
                MemoryStream stream = new MemoryStream(byteArray);

                // Open the stream in a StreamReader
                StreamReader streamReader = new StreamReader(stream);
                string json = streamReader.ReadToEnd();
                var serializer = new DataContractJsonSerializer(typeof(CodeData));
                CodeData codeData = (CodeData)serializer.ReadObject(stream);
                streamReader.Close();

                MessageBox.Show(String.Format("Restaurant: {0}, Guests: {1}, Name: {2}, Time To Wait: {3}, Key: {4}", codeData.restaurant, codeData.guests, codeData.name, codeData.time_to_wait, codeData.key));
            }
            catch (System.Exception ex)
            {
                // TODO: Properly handle errors. e.g. 404, not found, etc.
                MessageBox.Show(ex.Message);
            }
        }
    }
}