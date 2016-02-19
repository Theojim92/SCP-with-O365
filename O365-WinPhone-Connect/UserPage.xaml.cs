using O365_WinPhone_Connect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Band;
using System.Diagnostics;
using Newtonsoft.Json;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace O365_WinPhone_Connect
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    public class Demo
    {
        public string DemoContent { get; set; }
        public string Teamname { get; set; }
    }

 

    public sealed partial class UserPage : Page
    {
       
        ComboBoxItem FirstItem = new ComboBoxItem();
        int number_of_pending_polls = 5;

        public UserPage()
        {
            this.InitializeComponent();

            
            this.creat_teams_layout(10, 5);
            this.check_connection();
            this.combobox_list(10);
           
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
            UserLoginResponse data_from_login = e.Parameter as UserLoginResponse ;
            textBlock1.Text = "Hello " + data_from_login.FirstName + "!";
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            
        }

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            myDialog.Visibility = Visibility.Collapsed;
        }

        private void appBarButton1_Click(object sender, RoutedEventArgs e)
        {

            if (myDialog.Visibility == Visibility.Collapsed)
            {
                myDialog.Visibility = Visibility.Visible;
                appBarButton1.Foreground = new SolidColorBrush(Colors.Black);
                myDialog2.Visibility = Visibility.Collapsed;
                appBarButton2.Foreground = new SolidColorBrush(Colors.White);
                flipView.Visibility = Visibility.Collapsed;
                pending_poll_requests(1);


            }
            else
            {
                myDialog.Visibility = Visibility.Collapsed;
                appBarButton1.Foreground = new SolidColorBrush(Colors.White);
                flipView.Visibility = Visibility.Visible;
                pending_poll_requests(0);

            }

            textbox1.Text = String.Empty;
            textbox1.Visibility = Visibility.Visible;
            generatepoll.Visibility = Visibility.Visible;
            combobox.Visibility = Visibility.Visible;
            radiobutton.Visibility = Visibility.Visible;
            radiobutton1.Visibility = Visibility.Visible;
            textBlock2.Visibility = Visibility.Visible;
            textBlock3.Visibility = Visibility.Collapsed;
            combobox.SelectedItem = null;
        }

        private void retrieve_teams_and_members()
        {

        }

        //Dynamically Create the Team Hubsections depending on the number of the Teams that the employ participates either as a team leader
        //or as an employ
        
        private void creat_teams_layout(int number_of_teams, int number_of_teams_memebers)
        {
            List<Demo> demoList = new List<Demo>();
            
            for (int i = 1; i <= number_of_teams; i++)
            {
                String team = "Team " + i+ System.Environment.NewLine;
                String members = null;
                for (int j = 1; j <= 6; j++)
                {
                    members +="Member "+ j+System.Environment.NewLine;
                }

                demoList.Add(new Demo() { DemoContent = members, Teamname = team });

            }

            flipView.ItemsSource = demoList;
        }
        
        private async void check_connection()
        {
            try
            {
                IBandInfo[] pairedBands = await BandClientManager.Instance.GetBandsAsync();
                if (pairedBands.Length < 1)
                {
                    appBarButton.Foreground = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    appBarButton.Foreground = new SolidColorBrush(Colors.Green);
                }
            }
            catch (BandException ex)
            {
                
            }
        }

        private void appBarButton_Click(object sender, RoutedEventArgs e)
        {
            this.check_connection();
        }

        private void appBarButton2_Click(object sender, RoutedEventArgs e)
        {
            if (myDialog2.Visibility == Visibility.Collapsed)
            {
                myDialog2.Visibility = Visibility.Visible;
                appBarButton2.Foreground = new SolidColorBrush(Colors.Black);
                myDialog.Visibility = Visibility.Collapsed;
                appBarButton1.Foreground = new SolidColorBrush(Colors.White);
                flipView.Visibility = Visibility.Collapsed;
                textbox1.Text = String.Empty;
                pending_poll_requests(0);

            }
            else
            {
                myDialog2.Visibility = Visibility.Collapsed;
                appBarButton2.Foreground = new SolidColorBrush(Colors.White);
                flipView.Visibility = Visibility.Visible;
                
            }


            textbox1.Text = String.Empty;
            textbox1.Visibility = Visibility.Visible;
            generatepoll.Visibility = Visibility.Visible;
            combobox.Visibility = Visibility.Visible;
            radiobutton.Visibility = Visibility.Visible;
            radiobutton1.Visibility = Visibility.Visible;
            textBlock2.Visibility = Visibility.Visible;
            textBlock3.Visibility = Visibility.Collapsed;
            combobox.SelectedItem = FirstItem;

            



        }

        private void textbox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            textbox1.Background = new SolidColorBrush(Colors.Black);
            textbox1.Foreground = new SolidColorBrush(Colors.White);
        }

        private void generatepoll_Click(object sender, RoutedEventArgs e)
        {
            textbox1.Text = String.Empty;
            textbox1.Visibility = Visibility.Collapsed;
            generatepoll.Visibility = Visibility.Collapsed;
            combobox.Visibility = Visibility.Collapsed;
            radiobutton.Visibility = Visibility.Collapsed;
            radiobutton1.Visibility = Visibility.Collapsed;
            textBlock2.Visibility = Visibility.Collapsed;
            textBlock3.Visibility = Visibility.Visible;
            combobox.SelectedItem = FirstItem;
        }

        private void combobox_list(int numberofteams)
        {
            for (int i = 1; i <= numberofteams; i++)
            {
                ComboBoxItem helpitem = new ComboBoxItem();
                helpitem.Name = "Team:" + i;
                if (i == 1)
                {
                    
                    FirstItem = helpitem;
                    helpitem.IsSelected = true;
                }
                helpitem.Tag = "Team:" + i;
                helpitem.Content = "Team: " + i;
                combobox.Items.Add(helpitem);
            }
        }

        private void pending_poll_requests(int action)
        {
            if (action == 1)
            {
                int previous_top_button_position = 60;

                TextBlock poll_header = new TextBlock();
                poll_header.Height = 35;
                poll_header.Width = 337;
                poll_header.HorizontalAlignment = HorizontalAlignment.Center;
                poll_header.VerticalAlignment = VerticalAlignment.Top;
                poll_header.Text = "Your Pending Poll Requests";
                poll_header.FontSize = 25;
                poll_header.Foreground = new SolidColorBrush(Colors.White);
                poll_header.Margin = new Thickness(35, 14, 100, 50);
                myDialog.Children.Add(poll_header);
                Random random = new Random();
                for (int i = 1; i <= number_of_pending_polls; i++)
                {

                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //When we create the button we need to know if it corresponds to a 5-star question or a Yes/No question
                    // replace the id with real value and delete the Random function above
                    int id = random.Next(0, 2);

                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    Button pollrequest = new Button();
                    pollrequest.Name = "pending" + i;
                    pollrequest.HorizontalAlignment = HorizontalAlignment.Right;
                    pollrequest.VerticalAlignment = VerticalAlignment.Top;
                    pollrequest.Height = 40;
                    pollrequest.Width = 250;
                    pollrequest.Content = "Respond to Poll Request :" + i;
                    pollrequest.Margin = new Thickness(60, previous_top_button_position + 15, 100, 50);

                    if (id == 0)
                    {
                        pollrequest.Click += new RoutedEventHandler(question_star);

                    }
                    else
                    {
                        pollrequest.Click += new RoutedEventHandler(question_yesorno);
                    }

                    
                    myDialog.Children.Add(pollrequest);


                    previous_top_button_position += 60;



                }
            }
            else
            {
                myDialog.Children.Clear();
            }
        }

        public void question_star(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(stars));
        }

        public void question_yesorno(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(yesorno));
        }

        private void textbox1_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            textbox1.Focus(Windows.UI.Xaml.FocusState.Keyboard);
        }

        private void textbox1_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                Windows.UI.ViewManagement.InputPane.GetForCurrentView().TryHide();
            }
        }

        
    }
}
