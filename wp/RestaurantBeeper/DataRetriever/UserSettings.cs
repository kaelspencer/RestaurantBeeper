using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;

namespace RestaurantBeeper
{
    public static class InternalStorage
    {
        private const string ImageLocation = "Images";
        private static IsolatedStorageSettings isolatedStorageSettings = IsolatedStorageSettings.ApplicationSettings;

        public static void SaveToIsolatedStorage(string key, Object obj)
        {
            InternalStorage.isolatedStorageSettings[key] = obj;
        }

        public static T LoadFromIsolatedStorage<T>(string key)
        {
            return (T)InternalStorage.isolatedStorageSettings[key];
        }

        private static void EmptyIsolatedStorage()
        {
            InternalStorage.isolatedStorageSettings.Clear();

            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                store.Remove();
            }
        }

        public static void RemoveFromIsolatedStorage(string key)
        {
            InternalStorage.isolatedStorageSettings.Remove(key);
        }

        public static void CommitToIsolatedStorage()
        {
            InternalStorage.isolatedStorageSettings.Save();
        }

        public static bool SaveImage(string fileName, byte[] imageContents)
        {
            try
            {
                if (imageContents != null)
                {
                    using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        var path = System.IO.Path.Combine(ImageLocation, fileName);

                        if (!store.DirectoryExists(ImageLocation))
                        {
                            store.CreateDirectory(ImageLocation);
                        }

                        using (var stream = store.OpenFile(path, FileMode.Create))
                        {
                            stream.Write(imageContents, 0, imageContents.Length);
                            stream.Close();
                        }

                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
                return false;
            }
        }

        public static BitmapImage RetrieveImage(string fileName)
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                string path = System.IO.Path.Combine(ImageLocation, fileName);

                if (store.FileExists(path))
                {
                    using (var stream = store.OpenFile(path, FileMode.Open))
                    {
                        var image = new BitmapImage();
                        image.SetSource(stream);
                        return image;
                    }
                }
                else
                {
                    throw new FileNotFoundException("The image file does not exist.");
                }
            }
        }
    }

    public class UserSettings
    {
        public bool IsWaiting { get; set; }
        public string UserKey { get; set; }
        public string RestaurantName { get; set; }
        public string GuestName { get; set; }
        public int NumberOfGuests { get; set; }
        public int StartTimeToWait { get; set; }
        public int LastTimeToWait { get; set; }
        public Uri HostUri { get; set; }
        public Uri RegistrationUri { get; set; }
        public Uri RetrievalUri { get; set; }
        public DateTime TimeStarted { get; set; }
        public DateTime TimeLastChecked { get; set; }
        public DateTime TimeExpected { get; set; }
        public Uri RestaurantImagePath { get; set; }
        public string RestaurantImageName { get; set; }
    }

    public static class UserURLs
    {
        public static Uri HostUri { get; set; }
        public static Uri RegistrationUri { get; set; }
        public static Uri RetrievalUri { get; set; }
        public static Uri CancelUri { get; set; }
        public static Uri DelayUri { get; set; }
        public static Uri RestaurantUri { get; set; }

        public static void Save()
        {
            InternalStorage.SaveToIsolatedStorage("HostUri", UserURLs.HostUri);
            InternalStorage.SaveToIsolatedStorage("RegistrationUri", UserURLs.RegistrationUri);
            InternalStorage.SaveToIsolatedStorage("RetrievalUri", UserURLs.RetrievalUri);
            InternalStorage.SaveToIsolatedStorage("CancelUri", UserURLs.CancelUri);
            InternalStorage.SaveToIsolatedStorage("DelayUri", UserURLs.DelayUri);
            InternalStorage.SaveToIsolatedStorage("RestaurantUri", UserURLs.RestaurantUri);
        }

        public static void Load()
        {
            UserURLs.HostUri = InternalStorage.LoadFromIsolatedStorage<Uri>("HostUri");
            UserURLs.RegistrationUri = InternalStorage.LoadFromIsolatedStorage<Uri>("RegistrationUri");
            UserURLs.RetrievalUri = InternalStorage.LoadFromIsolatedStorage<Uri>("RetrievalUri");
            UserURLs.CancelUri = InternalStorage.LoadFromIsolatedStorage<Uri>("CancelUri");
            UserURLs.DelayUri = InternalStorage.LoadFromIsolatedStorage<Uri>("DelayUri");
            UserURLs.RestaurantUri = InternalStorage.LoadFromIsolatedStorage<Uri>("RestaurantUri");
        }
    }
}
