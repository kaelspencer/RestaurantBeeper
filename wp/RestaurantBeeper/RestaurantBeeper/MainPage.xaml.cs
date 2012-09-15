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
    public class RetrievedHeader
    {
        public int code { get; set; }
        public string message { get; set; }
        public RetrievedData data { get; set; }
    }

    public class RetrievedData
    {
        public int time_to_wait { get; set; }
        public int guests { get; set; }
        public string name { get; set; }
        public string key { get; set; }
        public string restaurant { get; set; }
    }

    public class RegistrationData
    {
        public string poll_url { get; set; }
    }

    public static class UserURLs
    {
        /// <summary>
        /// The URL to hit when registering the user. Use String.Format() to insert the provided key
        /// </summary>
        public static Uri HostUri { get; set; }
        public static Uri RegistrationUri { get; set; }
        public static Uri RetrievalUri { get; set; }
    }

    public partial class MainPage : PhoneApplicationPage
    {

        private IsolatedStorageSettings isolatedStorageSettings = IsolatedStorageSettings.ApplicationSettings;

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

            if (this.TryLoadSettings())
            {
                // TODO: Navigate to waiting page
            }

        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }

        private bool TryLoadSettings()
        {
            try
            {
                UserSettings.IsWaiting = (bool)isolatedStorageSettings["IsWaiting"];
                if (UserSettings.IsWaiting)
                {
                    UserSettings.UserKey = (string)isolatedStorageSettings["Key"];
                    UserSettings.HostUri = (Uri)isolatedStorageSettings["HostUri"];
                    UserSettings.RegistrationUri = (Uri)isolatedStorageSettings["RegistrationUri"];
                    UserSettings.RetrievalUri = (Uri)isolatedStorageSettings["RetrievalUri"];
                    UserSettings.StartTimeToWait = (int)isolatedStorageSettings["StartTimeToWait"];
                    UserSettings.LastTimeToWait = (int)isolatedStorageSettings["LastTimeToWait"];
                    UserSettings.TimeStarted = (DateTime)isolatedStorageSettings["TimeStarted"];
                    UserSettings.TimeLastChecked = (DateTime)isolatedStorageSettings["TimeLastChecked"];
                    UserSettings.TimeExpected = (DateTime)isolatedStorageSettings["TimeExpected"];

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
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
            RegisterUser();
        }

        private void RegisterUser()
        {
            WebClient webClient = new WebClient();
            webClient.DownloadStringAsync(UserURLs.RegistrationUri);
            webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(DownloadRegisterCompleted);
        }

        private void RetrieveUser()
        {
            WebClient webClient = new WebClient();
            webClient.DownloadStringAsync(UserURLs.RetrievalUri);
            webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(DownloadRetrieveCompleted);
        }

        private void DownloadRegisterCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                string result = e.Result;

                RegistrationData registrationData = ParseJson<RegistrationData>(result);

                UriBuilder uriBuilder = new UriBuilder(UserURLs.HostUri);
                uriBuilder.Path += registrationData.poll_url;
                UserURLs.RetrievalUri = uriBuilder.Uri;

                MessageBox.Show(UserURLs.RetrievalUri.ToString());
                this.RetrieveUser();

            }
            catch (System.Exception ex)
            {
                // TODO: Properly handle errors. e.g. 404, not found, etc.
                MessageBox.Show(ex.Message);
            }
        }

        private void DownloadRetrieveCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                string result = e.Result;
                RetrievedHeader codeData = ParseJson<RetrievedHeader>(result);

                if (codeData.code == 0)
                {
                    // TODO: Move on here...
                    MessageBox.Show(String.Format("Restaurant: {0}, Guests: {1}, Name: {2}, Time To Wait: {3}, Key: {4}", codeData.data.restaurant, codeData.data.guests, codeData.data.name, codeData.data.time_to_wait, codeData.data.key));
                }
                else
                {
                    switch (codeData.code)
                    {
                        case 1:
                            // TODO: What's this error code definition?
                            MessageBox.Show("When attempting to contact the server, we received an error code of '1'. Please try again or ask for assistance.", "Hmm...", MessageBoxButton.OK);
                            break;
                        default:
                            MessageBox.Show(String.Format("When attempting to contact the server, we received an unknown error code of '{0}'. Please try again or ask for assistance.", codeData.code), "Whoops...", MessageBoxButton.OK);
                            break;
                    }
                }
            }
            catch (System.Exception ex)
            {
                // TODO: Properly handle errors. e.g. 404, not found, etc.
                MessageBox.Show(ex.Message);
            }
        }

        private T ParseJson<T>(string json)
        {
            // convert string to stream
            byte[] byteArray = Encoding.UTF8.GetBytes(json);
            MemoryStream stream = new MemoryStream(byteArray);

            // Open the stream in a StreamReader
            StreamReader streamReader = new StreamReader(stream);
            string json2 = streamReader.ReadToEnd();
            var serializer = new DataContractJsonSerializer(typeof(T));
            T codeData = (T)serializer.ReadObject(stream);
            streamReader.Close();

            return codeData;
        }
    }
}