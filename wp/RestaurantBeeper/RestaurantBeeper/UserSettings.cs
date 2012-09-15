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
    public static class UserSettings
    {
        public static bool IsWaiting { get; set; }
        public static string UserKey { get; set; }
        public static Uri HostUri { get; set; }
        public static Uri RegistrationUri { get; set; }
        public static Uri RetrievalUri { get; set; }
        public static int StartTimeToWait { get; set; }
        public static int LastTimeToWait { get; set; }
        public static DateTime TimeStarted { get; set; }
        public static DateTime TimeLastChecked { get; set; }
        public static DateTime TimeExpected { get; set; }
    }
}
