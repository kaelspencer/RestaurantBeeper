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

namespace RestaurantBeeper
{
    public static class InternalStorage
    {
        private static IsolatedStorageSettings isolatedStorageSettings = IsolatedStorageSettings.ApplicationSettings;

        public static void SaveToIsolatedStorage(string key, Object obj)
        {
            InternalStorage.isolatedStorageSettings[key] = obj;
        }

        public static T LoadFromIsolatedStorage<T>(string key)
        {
            return (T)InternalStorage.isolatedStorageSettings[key];
        }

        public static void EmptyIsolatedStorage()
        {
            InternalStorage.isolatedStorageSettings.Clear();
        }

        public static void CommitToIsolatedStorage()
        {
            InternalStorage.isolatedStorageSettings.Save();
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
    }
}
