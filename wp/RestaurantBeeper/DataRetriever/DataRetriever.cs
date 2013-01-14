using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows.Resources;

namespace RestaurantBeeper
{
    public static class DataRetriever
    {
        public delegate void CallBackOnRegistration();

        public delegate void CallBackOnRetrieval(UserSettings userSettings);

        public delegate void CallBackOnRestaurant(RestaurantSettings restaurantData);

        public delegate void CallBackOnImageRetrieval(byte[] image);

        /// <summary>
        /// The method to call after the user is successfully registered
        /// </summary>
        public static CallBackOnRegistration UserRegisteredSuccessfully { get; set; }

        /// <summary>
        /// The method to call if the user fails to be registered
        /// </summary>
        public static CallBackOnRegistration UserRegisteredFailure { get; set; }

        /// <summary>
        /// The method to call after the data is successfully retrieved
        /// </summary>
        public static CallBackOnRetrieval DataRetrievedSuccessfully { get; set; }

        /// <summary>
        /// The method to call if the data retrieval fails
        /// </summary>
        public static CallBackOnRetrieval DataRetrievedFailure { get; set; }

        /// <summary>
        /// The method to call after the restaurant info is successfully retrieved
        /// </summary>
        public static CallBackOnRestaurant RestaurantRetrievedSuccessfully { get; set; }

        /// <summary>
        /// The method to call after the restaurant info fails to be retrieved
        /// </summary>
        public static CallBackOnRestaurant RestaurantRetrievedFailure { get; set; }

        /// <summary>
        /// The method to call after an image has been downloaded
        /// </summary>
        public static CallBackOnImageRetrieval ImageRetrieved { get; set; }

        public static void RegisterUser()
        {
            WebClient webClient = new WebClient();
            webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(DataRetriever.DownloadRegisterCompleted);
            webClient.DownloadStringAsync(UserURLs.RegistrationUri);
        }

        public static void RetrieveUser()
        {
            WebClient webClient = new WebClient();
            webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(DataRetriever.DownloadRetrieveCompleted);
            webClient.DownloadStringAsync(UserURLs.RetrievalUri);
        }

        public static void RetrieveRestaurant()
        {
            WebClient webClient = new WebClient();
            webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(DataRetriever.DownloadRestaurantCompleted);
            webClient.DownloadStringAsync(UserURLs.RestaurantUri);
        }

        public static void GetImageFile(Uri imageUri)
        {
            WebClient webClient = new WebClient();
            webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(DownloadImageCompleted);
            webClient.OpenReadAsync(imageUri);
        }

        public static void DownloadImageCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            var resInfo = new StreamResourceInfo(e.Result, null);
            var reader = new StreamReader(resInfo.Stream);
            byte[] contents;

            using (BinaryReader bReader = new BinaryReader(reader.BaseStream))
            {
                contents = bReader.ReadBytes((int)reader.BaseStream.Length);
            }

            ImageRetrieved(contents);
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
                uriBuilder.Path = uriBuilder.Path.Replace(registrationData.poll_url, registrationData.cancel_url);
                UserURLs.CancelUri = uriBuilder.Uri;
                uriBuilder.Path = uriBuilder.Path.Replace(registrationData.cancel_url, registrationData.delay_url);
                UserURLs.DelayUri = uriBuilder.Uri;
                uriBuilder.Path = uriBuilder.Path.Replace(registrationData.delay_url, registrationData.restaurant_url);
                UserURLs.RestaurantUri = uriBuilder.Uri;

                if (DataRetriever.UserRegisteredSuccessfully != null)
                {
                    DataRetriever.UserRegisteredSuccessfully();
                }
            }
            catch (System.Exception ex)
            {
                // TODO: Properly handle errors. e.g. 404, not found, etc.
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
                            //MessageBox.Show("When attempting to contact the server, we received an error code of '1'. Please try again or ask for assistance.", "Hmm...", MessageBoxButton.OK);
                            break;

                        default:

                            //MessageBox.Show(String.Format("When attempting to contact the server, we received an unknown error code of '{0}'. Please try again or ask for assistance.", retrievedHeader.code), "Whoops...", MessageBoxButton.OK);
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

                if (DataRetriever.DataRetrievedFailure != null)
                {
                    DataRetriever.DataRetrievedFailure(null);
                }
            }
        }

        private static void DownloadRestaurantCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                string result = e.Result;
                RestaurantData restaurantInfo = ParseJson<RestaurantData>(result);

                RestaurantSettings restaurantSettings = new RestaurantSettings();
                restaurantSettings.RestaurantImagePath = new Uri(restaurantInfo.background_image_url);
                restaurantSettings.RestaurantImageName = "Image.jpg";
                restaurantSettings.PrimaryColor = restaurantInfo.primary_color;
                restaurantSettings.SecondaryColor = restaurantInfo.secondary_color;

                if (DataRetriever.RestaurantRetrievedSuccessfully != null)
                {
                    DataRetriever.RestaurantRetrievedSuccessfully(restaurantSettings);
                }
            }
            catch (System.Exception ex)
            {
                // TODO: Properly handle errors. e.g. 404, not found, etc.

                if (DataRetriever.RestaurantRetrievedFailure != null)
                {
                    DataRetriever.RestaurantRetrievedFailure(null);
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

        public static void CodeRetrieved(string result)
        {
            result = result.Replace("descartes:8000", "restaurant.kaelspencer.com");
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
    }
}