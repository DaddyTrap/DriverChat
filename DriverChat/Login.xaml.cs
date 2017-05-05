using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace DriverChat
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Login : Page
    {
        DriverChat.Socket.Client c = DriverChat.Socket.Client.GetClient();
        public Login()
        {
            this.InitializeComponent();
            ApplicationView.PreferredLaunchViewSize = new Size(400, 300);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
   
        }


        private void signIn(object sender, RoutedEventArgs e)
        {
            bool flag = true;
            c.Create_Signin_json(Username.Text, Password.Text);
            try
            {
                c.GotSigninError += async (msg) =>
                {
                    MessageDialog t = new MessageDialog(msg);
                    await t.ShowAsync();
                    flag = false;
                };
            }
            finally
            {
                if (flag)
                    Frame.Navigate(typeof(MainPage));
            }
        }

        private void signUp(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Signup));
        }
    }
}
