namespace RestaurantBeeper
{
    public class RetrievedHeader
    {
        public string status { get; set; }

        public string message { get; set; }

        public int code { get; set; }

        public RetrievedData data { get; set; }
    }

    public class RetrievedData
    {
        public int time_to_wait { get; set; }

        public int guests { get; set; }

        public string name { get; set; }

        public string key { get; set; }

        public string restaurant { get; set; }

        public bool registered { get; set; }

        public bool push_enabled { get; set; }
    }

    public class RegistrationData
    {
        public string cancel_url { get; set; }

        public string delay_url { get; set; }

        public string poll_url { get; set; }

        public string restaurant_url { get; set; }
    }

    public class RestaurantData
    {
        public string name { get; set; }

        public string background_image_url { get; set; }

        public string primary_color { get; set; }

        public string secondary_color { get; set; }
    }
}