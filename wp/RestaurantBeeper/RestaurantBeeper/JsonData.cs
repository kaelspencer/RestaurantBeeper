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
}
