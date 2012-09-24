#define DEBUG_AGENT
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Scheduler;
using System.Collections.Generic;
using System.Threading;

namespace RestaurantBeeper
{
    public partial class MainPage : PhoneApplicationPage
    {
        private enum PanoPages { Reserve = 0, Wait, Action, Offers, Info, Help }
        public UserSettings userSettings { get; set; }

        private Timer updateTimer;
        private bool ImageLoaded;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            this.ImageLoaded = false;

            if (this.TryLoadSettings())
            {
                if (this.userSettings.RetrievalUri != null)
                {
                    this.InitWaitingPano();

                    DataRetriever.DataRetrievedSuccessfully = new DataRetriever.CallBackOnRetrieval(this.UpdateUserSettings);
                    updateTimer = new Timer(new TimerCallback(this.UpdateReservation), null, 0, 1000);

                    this.SetDefaultPanoPage(PanoPages.Wait);
                }
                else
                {
                    MessageBox.Show("There was a problem with the information stored from last time. Please try again or ask for assistance.", "Huh...", MessageBoxButton.OK);
                    InternalStorage.RemoveFromIsolatedStorage("UserSettings");
                }
            }
        }

        private void UpdateReservation(Object obj)
        {
            // Step 1 - Ping Web service for updated info
            DataRetriever.RetrieveUser();
            // Step 2 - Copy the start time to wait from the last update to the newest one -- Handled in UpdateUserSettings
            // Step 3 - Replace the old info with the new info -- Handled in UpdateUserSettings
            // Step 4 - Save 
            // Step 5 - Update UI
            this.userSettings.LastTimeToWait--;
            this.InitWaitingPano();

        }

        private void UpdateUserSettings(UserSettings updatedUserSettings)
        {
            updatedUserSettings.StartTimeToWait = this.userSettings.StartTimeToWait;
            this.userSettings = updatedUserSettings;
            InternalStorage.SaveToIsolatedStorage("UserSettings", this.userSettings);

            if (this.userSettings.LastTimeToWait > 0)
            {
                this.InitWaitingPano();
            }
            else
            {
                // TODO: Change UI to notify the user that time has elapsed, clear out settings?
                // Kill timer?
            }
        }

        private void InitWaitingPano()
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    this.textBlockRestaurantName.Text = userSettings.RestaurantName;
                    this.testBlockGuestName.Text = userSettings.GuestName;
                    this.textBlockNumberOfGuests.Text = userSettings.NumberOfGuests.ToString();
                    this.textBlockTimeToWait.Text = userSettings.LastTimeToWait + "minutes";

                    // TODO: Get the real color via the service...
                    this.MainPano.Foreground = this.ConvertHexToBrush("#FFF5F500");

                    foreach (UIElement element in this.stackPanelWait.Children)
                    {
                        if (element.Visibility == Visibility.Collapsed && element != this.textBlockNotWaiting)
                        {
                            this.ToggleVisibility(element);
                        }
                    }

                    this.SetVisibility(this.textBlockNotWaiting, Visibility.Collapsed);

                    if (userSettings.StartTimeToWait > 0 && userSettings.LastTimeToWait > 0)
                    {
                        this.progressBarWaitTime.IsIndeterminate = false;
                        this.progressBarWaitTime.Value = 100 - ((((float)userSettings.StartTimeToWait - (float)userSettings.LastTimeToWait) / (float)userSettings.StartTimeToWait) * 100);
                    }

                    if (!this.ImageLoaded)
                    {
                        this.LoadImage();
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("There was a problem loading your data... We're really sorry. Please try again. " + ex.Message, "We goofed", MessageBoxButton.OK);
                }
            });
        }

        #region BackgroundAgent

        PeriodicTask periodicTask;

        string periodicTaskName = "PeriodicAgent";
        public bool agentsAreEnabled = true;

        private void StartPeriodicAgent()
        {
            try
            {
                if (InternalStorage.LoadFromIsolatedStorage<bool>("BackgroundAgentEnabled"))
                {
                    // Variable for tracking enabled status of background agents for this app.
                    agentsAreEnabled = true;

                    // Obtain a reference to the period task, if one exists
                    periodicTask = ScheduledActionService.Find(periodicTaskName) as PeriodicTask;

                    // If the task already exists and background agents are enabled for the
                    // application, you must remove the task and then add it again to update 
                    // the schedule
                    if (periodicTask != null)
                    {
                        RemoveAgent(periodicTaskName);
                    }

                    periodicTask = new PeriodicTask(periodicTaskName);

                    // The description is required for periodic agents. This is the string that the user
                    // will see in the background services Settings page on the device.
                    periodicTask.Description = "This background service is used to check on reservation statuses when in use. It will be disabled when no reservations are active.";

                    // Place the call to Add in a try block in case the user has disabled agents.
                    try
                    {
                        ScheduledActionService.Add(periodicTask);
                        //PeriodicStackPanel.DataContext = periodicTask;

                        // If debugging is enabled, use LaunchForTest to launch the agent in one minute.
#if(DEBUG_AGENT)
                        ScheduledActionService.LaunchForTest(periodicTaskName, TimeSpan.FromSeconds(60));
#endif
                    }
                    catch (InvalidOperationException exception)
                    {
                        if (exception.Message.Contains("BNS Error: The action is disabled"))
                        {
                            MessageBox.Show("Background agents for this application have been disabled by the user.");
                            agentsAreEnabled = false;
                            //PeriodicCheckBox.IsChecked = false;
                        }

                        if (exception.Message.Contains("BNS Error: The maximum number of ScheduledActions of this type have already been added."))
                        {
                            // No user action required. The system prompts the user when the hard limit of periodic tasks has been reached.

                        }
                        //PeriodicCheckBox.IsChecked = false;
                    }
                    catch (SchedulerServiceException)
                    {
                        // No user action required.
                        //PeriodicCheckBox.IsChecked = false;
                    }
                }
            }
            catch (KeyNotFoundException)
            {
                MessageBoxResult mbResult = MessageBox.Show(@"Would you mind if we enabled background services for you? Background services allow us to keep track of any reservations while the app isn't running. Don't worry, if you don't have any active " +
                    "reservations, we'll disable this automatically for you. You can always disable this at any time in settings.", "Do you mind?", MessageBoxButton.OKCancel);

                if (mbResult == MessageBoxResult.OK)
                {
                    InternalStorage.SaveToIsolatedStorage("BackgroundAgentEnabled", true);
                    this.StartPeriodicAgent();
                }
                else
                {
                    InternalStorage.SaveToIsolatedStorage("BackgroundAgentEnabled", false);
                }
            }
        }

        private void RemoveAgent(string name)
        {
            try
            {
                ScheduledActionService.Remove(name);
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region Helpers
        private bool TryLoadSettings()
        {
            try
            {
                this.userSettings = InternalStorage.LoadFromIsolatedStorage<UserSettings>("UserSettings");
                return this.userSettings.IsWaiting;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void ToggleVisibility(UIElement obj)
        {
            if (obj.Visibility == Visibility.Collapsed)
            {
                obj.Visibility = Visibility.Visible;
            }
            else
            {
                obj.Visibility = Visibility.Collapsed;
            }
        }

        private void SetVisibility(UIElement element, Visibility visibility)
        {
            element.Visibility = visibility;
        }

        private void SetDefaultPanoPage(PanoPages page)
        {
            this.MainPano.DefaultItem = this.MainPano.Items[(int)page];
        }

        private void SetPanoBackground(ImageBrush imageBrush)
        {
            this.MainPano.Background = imageBrush;
            this.ImageLoaded = true;
        }

        private void ClearBackStack()
        {
            // While there are pages on the backstack, clear them out.
            while (NavigationService.CanGoBack)
            {
                NavigationService.RemoveBackEntry();
            }
        }

        private void LoadImage()
        {
            // Try to load the image from internal storage
            BitmapImage bitmapImage = null;
            try
            {
                // Try to load the image from internal storage in case we already have it
                bitmapImage = InternalStorage.RetrieveImage(this.userSettings.RestaurantImageName);

                if (bitmapImage != null)
                {
                    ImageBrush imageBrush = new ImageBrush();
                    imageBrush = new ImageBrush();
                    imageBrush.ImageSource = bitmapImage;
                    this.SetPanoBackground(imageBrush);
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
                ImageBrush imageBrush = new ImageBrush();
                BitmapImage bitmapImage = InternalStorage.RetrieveImage(this.userSettings.RestaurantImageName);
                imageBrush = new ImageBrush();
                imageBrush.ImageSource = bitmapImage;
                this.SetPanoBackground(imageBrush);
            }
        }

        private Brush ConvertHexToBrush(string hexValue)
        {
            byte alpha;
            byte pos = 0;

            string hex = hexValue.ToString().Replace("#", "");

            if (hex.Length == 8)
            {
                alpha = System.Convert.ToByte(hex.Substring(pos, 2), 16);
                pos = 2;
            }
            else
            {
                alpha = System.Convert.ToByte("ff", 16);
            }

            byte red = System.Convert.ToByte(hex.Substring(pos, 2), 16);

            pos += 2;
            byte green = System.Convert.ToByte(hex.Substring(pos, 2), 16);

            pos += 2;
            byte blue = System.Convert.ToByte(hex.Substring(pos, 2), 16);

            return new SolidColorBrush(Color.FromArgb(alpha, red, green, blue));
        }

        #endregion

        #region Events
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

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/ScanPage.xaml", UriKind.Relative));
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }

        // Test code
        private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
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
            userSettings.LastTimeToWait = 40;
            userSettings.TimeStarted = DateTime.Now;
            userSettings.TimeLastChecked = DateTime.Now;
            userSettings.TimeExpected = DateTime.Now.AddMinutes(userSettings.LastTimeToWait);
            userSettings.RestaurantImagePath = new Uri("http://www.sattestpreptips.com/wp-content/plugins/sociable/buffalo-wild-wings-sauces-buy-747.jpg", UriKind.Absolute);
            //userSettings.RestaurantImagePath = new Uri("http://wac.450f.edgecastcdn.net/80450F/103gbfrocks.com/files/2011/11/Buffalo-Wild-Wings-wings.jpg", UriKind.Absolute);
            userSettings.RestaurantImageName = "Image2.jpg";

            InternalStorage.SaveToIsolatedStorage("UserSettings", userSettings);
            InternalStorage.CommitToIsolatedStorage();
            this.InitWaitingPano();
        }

        private void ApplicationBarMenuItem_Click_1(object sender, EventArgs e)
        {
            this.StartPeriodicAgent();
        }

        // End Test code
        #endregion

    }
}
