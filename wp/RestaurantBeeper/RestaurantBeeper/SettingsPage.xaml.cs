using Microsoft.Phone.Controls;
using System.Collections.Generic;
using System.Windows;

namespace RestaurantBeeper
{
    public partial class PivotPage1 : PhoneApplicationPage
    {
        public PivotPage1()
        {
            InitializeComponent();

            try
            {
                this.toggleSwitch1.IsChecked = InternalStorage.LoadFromIsolatedStorage<bool>("BackgroundAgentEnabled");
            }
            catch (KeyNotFoundException)
            {
            }
        }

        private void toggleSwitch1_Checked(object sender, RoutedEventArgs e)
        {
            InternalStorage.SaveToIsolatedStorage("BackgroundAgentEnabled", true);
        }

        private void toggleSwitch1_Unchecked(object sender, RoutedEventArgs e)
        {
            InternalStorage.SaveToIsolatedStorage("BackgroundAgentEnabled", false);
        }
    }
}