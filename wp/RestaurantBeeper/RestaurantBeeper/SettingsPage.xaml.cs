using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

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