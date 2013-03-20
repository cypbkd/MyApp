using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace MyApp
{
    public sealed partial class PopupInputContent : UserControl
    {
        public PopupInputContent()
        {
            this.InitializeComponent();

            //for (int i = 0; i < 24; i++)
            //{
            //    hour.Items.Add(i);
            //}

            //for (int i = 0; i < 60; i++)
            //{
            //    minite.Items.Add(i);
            //}
        }

        public event EventHandler<object> OK;

        // Handles the Click event of the 'Save' button simulating a save and close
        private void SimulateSaveClicked(object sender, RoutedEventArgs e)
        {
            // in this example we assume the parent of the UserControl is a Popup
            Popup p = this.Parent as Popup;
            p.IsOpen = false; // close the Popup
            OK(p,e);
        }

        private void SimulateCancleClicked(object sender, RoutedEventArgs e)
        {
            // in this example we assume the parent of the UserControl is a Popup
            Popup p = this.Parent as Popup;
            p.IsOpen = false; // close the Popup
        }
    }
}
