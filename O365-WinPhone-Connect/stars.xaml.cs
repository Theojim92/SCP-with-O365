using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace O365_WinPhone_Connect
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class stars : Page
    {
        

        public stars()
        {
            this.InitializeComponent();

            
        }

        
        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            
        }

        #endregion

        private void radioButton_Checked(object sender, RoutedEventArgs e)
        {
            
            
            image.Source = new BitmapImage(new Uri(image.BaseUri, "Pictures/yellow_star.png"));
            image1.Source = new BitmapImage(new Uri(image.BaseUri, "Pictures/white_star.png"));
            image2.Source = new BitmapImage(new Uri(image.BaseUri, "Pictures/white_star.png"));
            image3.Source = new BitmapImage(new Uri(image.BaseUri, "Pictures/white_star.png"));
            image4.Source = new BitmapImage(new Uri(image.BaseUri, "Pictures/white_star.png"));
            

        }

        private void radioButton1_Checked(object sender, RoutedEventArgs e)
        {
            image.Source = new BitmapImage(new Uri(image.BaseUri, "Pictures/yellow_star.png"));
            image1.Source = new BitmapImage(new Uri(image.BaseUri, "Pictures/yellow_star.png"));
            image2.Source = new BitmapImage(new Uri(image.BaseUri, "Pictures/white_star.png"));
            image3.Source = new BitmapImage(new Uri(image.BaseUri, "Pictures/white_star.png"));
            image4.Source = new BitmapImage(new Uri(image.BaseUri, "Pictures/white_star.png"));
        }

        private void radioButton2_Checked(object sender, RoutedEventArgs e)
        {
            image.Source = new BitmapImage(new Uri(image.BaseUri, "Pictures/yellow_star.png"));
            image1.Source = new BitmapImage(new Uri(image.BaseUri, "Pictures/yellow_star.png"));
            image2.Source = new BitmapImage(new Uri(image.BaseUri, "Pictures/yellow_star.png"));
            image3.Source = new BitmapImage(new Uri(image.BaseUri, "Pictures/white_star.png"));
            image4.Source = new BitmapImage(new Uri(image.BaseUri, "Pictures/white_star.png"));
        }

        private void radioButton3_Checked(object sender, RoutedEventArgs e)
        {
            image.Source = new BitmapImage(new Uri(image.BaseUri, "Pictures/yellow_star.png"));
            image1.Source = new BitmapImage(new Uri(image.BaseUri, "Pictures/yellow_star.png"));
            image2.Source = new BitmapImage(new Uri(image.BaseUri, "Pictures/yellow_star.png"));
            image3.Source = new BitmapImage(new Uri(image.BaseUri, "Pictures/yellow_star.png"));
            image4.Source = new BitmapImage(new Uri(image.BaseUri, "Pictures/white_star.png"));
        }

        private void radioButton4_Checked(object sender, RoutedEventArgs e)
        {
            image.Source = new BitmapImage(new Uri(image.BaseUri, "Pictures/yellow_star.png"));
            image1.Source = new BitmapImage(new Uri(image.BaseUri, "Pictures/yellow_star.png"));
            image2.Source = new BitmapImage(new Uri(image.BaseUri, "Pictures/yellow_star.png"));
            image3.Source = new BitmapImage(new Uri(image.BaseUri, "Pictures/yellow_star.png"));
            image4.Source = new BitmapImage(new Uri(image.BaseUri, "Pictures/yellow_star.png"));
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            
            Frame.Navigate(typeof(UserPage));

        }
    }
}
