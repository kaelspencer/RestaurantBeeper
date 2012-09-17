using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows;

namespace RestaurantBeeper
{
    public static class DataRetriever
    {
        public delegate void CallBackOnRegistration();
        public delegate void CallBackOnRetrieval(UserSettings userSettings);

        /// <summary>
        /// The method to call after the user is successfully registered
        /// </summary>
        public static CallBackOnRegistration UserRegisteredSuccessfully { get; set; }

        /// <summary>
        /// The method to call after the data is successfully retrieved
        /// </summary>
        public static CallBackOnRetrieval DataRetrievedSuccessfully { get; set; }

        /// <summary>
        /// The method to call after the user is successfully registered
        /// </summary>
        public static CallBackOnRegistration UserRegisteredFailure { get; set; }

        /// <summary>
        /// The method to call after the data is successfully retrieved
        /// </summary>
        public static CallBackOnRetrieval DataRetrievedFailure { get; set; }

        public static void RegisterUser()
        {
            WebClient webClient = new WebClient();
            webClient.DownloadStringAsync(UserURLs.RegistrationUri);
            webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(DataRetriever.DownloadRegisterCompleted);
        }

        public static void RetrieveUser()
        {
            WebClient webClient = new WebClient();
            webClient.DownloadStringAsync(UserURLs.RetrievalUri);
            webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(DataRetriever.DownloadRetrieveCompleted);
        }

        public static void DownloadRegisterCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                string result = e.Result;

                RegistrationData registrationData = ParseJson<RegistrationData>(result);

                UriBuilder uriBuilder = new UriBuilder(UserURLs.HostUri);
                uriBuilder.Path += registrationData.poll_url;
                UserURLs.RetrievalUri = uriBuilder.Uri;

                MessageBox.Show(UserURLs.RetrievalUri.ToString());

                if (DataRetriever.UserRegisteredSuccessfully != null)
                {
                    DataRetriever.UserRegisteredSuccessfully();
                }
            }
            catch (System.Exception ex)
            {
                // TODO: Properly handle errors. e.g. 404, not found, etc.
                MessageBox.Show(ex.Message);
                
                if (DataRetriever.UserRegisteredFailure != null)
                {
                    DataRetriever.UserRegisteredFailure();
                }
            }
        }

        private static void DownloadRetrieveCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                string result = e.Result;
                RetrievedHeader retrievedHeader = ParseJson<RetrievedHeader>(result);

                if (retrievedHeader.code == 0)
                {
                    UserSettings userSettings = new UserSettings();
                    userSettings.IsWaiting = true;
                    userSettings.UserKey = retrievedHeader.data.key;
                    userSettings.RestaurantName = retrievedHeader.data.restaurant;
                    userSettings.GuestName = retrievedHeader.data.name;
                    userSettings.NumberOfGuests = retrievedHeader.data.guests;
                    userSettings.HostUri = UserURLs.HostUri;
                    userSettings.RegistrationUri = UserURLs.RegistrationUri;
                    userSettings.RetrievalUri = UserURLs.RetrievalUri;
                    userSettings.StartTimeToWait = retrievedHeader.data.time_to_wait;
                    userSettings.LastTimeToWait = retrievedHeader.data.time_to_wait;
                    userSettings.TimeStarted = DateTime.Now;
                    userSettings.TimeLastChecked = DateTime.Now;
                    userSettings.TimeExpected = DateTime.Now.AddMinutes(retrievedHeader.data.time_to_wait);
                    MessageBox.Show(String.Format("Restaurant: {0}, Guests: {1}, Name: {2}, Time To Wait: {3}, Key: {4}", retrievedHeader.data.restaurant, retrievedHeader.data.guests, retrievedHeader.data.name, retrievedHeader.data.time_to_wait, retrievedHeader.data.key));

                    if (DataRetriever.DataRetrievedSuccessfully != null)
                    {
                        DataRetriever.DataRetrievedSuccessfully(userSettings);
                    }
                }
                else
                {
                    switch (retrievedHeader.code)
                    {
                        case 1:
                            // TODO: What's this error code definition?
                            MessageBox.Show("When attempting to contact the server, we received an error code of '1'. Please try again or ask for assistance.", "Hmm...", MessageBoxButton.OK);
                            break;
                        default:
                            MessageBox.Show(String.Format("When attempting to contact the server, we received an unknown error code of '{0}'. Please try again or ask for assistance.", retrievedHeader.code), "Whoops...", MessageBoxButton.OK);
                            break;
                    }

                    if (DataRetriever.DataRetrievedFailure != null)
                    {
                        DataRetriever.DataRetrievedFailure(null);
                    }
                }
            }
            catch (System.Exception ex)
            {
                // TODO: Properly handle errors. e.g. 404, not found, etc.
                MessageBox.Show(ex.Message);

                if (DataRetriever.DataRetrievedFailure != null)
                {
                    DataRetriever.DataRetrievedFailure(null);
                }
            }
        }

        public static T ParseJson<T>(string json)
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
