using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
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
    public sealed partial class Signup : Page
    {
        public Signup()
        {
            this.InitializeComponent();
        }

        private void Confirm(object sender, RoutedEventArgs e)
        {
            DriverChat.Socket.Client.GetClient().Create_Signup_json(UserName.Text, Password.Text, NickName.Text, DateTimeOffset.Now.ToString());
            DriverChat.Socket.Client.GetClient().GotSignupError += async (msg) =>
            {
                MessageDialog t = new MessageDialog(msg);
                await t.ShowAsync();
            };
            DriverChat.Socket.Client.GetClient().GotSignupSucceed += async (msg) =>
            {
                MessageDialog t = new MessageDialog("注册成功");
                await t.ShowAsync();
                Frame.Navigate(typeof(Login));
            };
        }

        private void Quit(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Login));
        }

    }
}
