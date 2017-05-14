using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
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

namespace DriverChat {
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Login : Page {
        public Login() {
            this.InitializeComponent();
            ApplicationView.PreferredLaunchViewSize = new Size(800, 800);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            DriverChat.Socket.Client.GetClient().Listener();
        }

        async void Error(string msg) {
            MessageDialog t = new MessageDialog(msg);
            await t.ShowAsync();
        }
        void Success(string msg) {
            DriverChat.Control.CurrentUser.CreateUser(DriverChat.Socket.Client.GetClient().did, DriverChat.Socket.Client.GetClient().name, DriverChat.Socket.Client.GetClient().badge);
            Frame.Navigate(typeof(MainPage));
        }
        private void signUp(object sender, RoutedEventArgs e) {
            Frame.Navigate(typeof(Signup));
        }
        private void signIn(object sender, RoutedEventArgs e) {
            DriverChat.Socket.Client.GetClient().Create_Signin_json(Username.Text, Password.Password);

        }
        protected override void OnNavigatedTo(NavigationEventArgs e) {
            DriverChat.Socket.Client.GetClient().GotSigninError += Error;
            DriverChat.Socket.Client.GetClient().GotSigninSucceed += Success;
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e) {
            DriverChat.Socket.Client.GetClient().GotSigninError -= Error;
            DriverChat.Socket.Client.GetClient().GotSigninSucceed -= Success;
        }
        private void Password_KeyDown(object sender, KeyRoutedEventArgs e) {
            if (e.Key == Windows.System.VirtualKey.Enter) {
                signIn(this, new RoutedEventArgs());
                e.Handled = true;
            }
        }
        private void Share_Click(object sender, RoutedEventArgs e) {
            var s = sender as FrameworkElement;
            DataTransferManager.GetForCurrentView().DataRequested += OnShareDataRequested;
            DataTransferManager.ShowShareUI();
        }

        void OnShareDataRequested(DataTransferManager sender, DataRequestedEventArgs args) {
            DataRequest request = args.Request;
            DataRequestDeferral getFiles = request.GetDeferral();
            request.Data.Properties.Title = "老司机分享";
            request.Data.Properties.Description = "我在老司机聊天室收获了好多，你也快来吧";

           request.Data.SetBitmap(Windows.Storage.Streams.RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets///Login.png")));

            request.Data.SetText("恭喜FA♂財VAN♂事顺心");
            getFiles.Complete();
        }
    }
}
