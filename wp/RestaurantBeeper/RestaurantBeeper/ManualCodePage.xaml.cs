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
    public partial class ManualCodePage : PhoneApplicationPage
    {
        public string Result { get; set; }

        public ManualCodePage()
        {
            InitializeComponent();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void buttonDone_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(this.textBox1.Text))
            {
                // TODO: Add some validation here?
                this.Result = this.textBox1.Text;
                NavigationService.Navigate(new Uri("/MainPage.xaml?ManualCode=" + this.Result, UriKind.Relative));
            }
            else
            {
                MessageBoxResult messageboxResult = MessageBox.Show("No code entered. Please enter a manual code or tap cancel to go back.", "Uh-oh", MessageBoxButton.OKCancel);
                if (messageboxResult == MessageBoxResult.Cancel)
                {
                    NavigationService.GoBack();
                }
            }
        }
    }
}