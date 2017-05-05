using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace DriverChat
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class RoomPage : Page
    {
        ViewModels.RoomViewModel ViewModel;
        public RoomPage()
        {
            this.InitializeComponent();
            var viewTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            viewTitleBar.BackgroundColor = Windows.UI.Colors.CornflowerBlue;
            viewTitleBar.ButtonBackgroundColor = Windows.UI.Colors.CornflowerBlue;


        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel = (ViewModels.RoomViewModel)e.Parameter;
            RName.Text = ViewModel.SelectedItem.RoomName;
            this.DataContext = ViewModel.SelectedItem;
        }

        private void SendMsg(object sender, RoutedEventArgs e)
        {
            string Msg = Msg_Input.Text;
            Msg_Input.Text = "";
            ViewModel.SelectedItem.SendMsg(Msg);
        }

        private void NewMsgCome(object sender, SizeChangedEventArgs e)
        {
            double d = MsgList.ActualHeight;
            MsgRoll.ScrollToVerticalOffset(d);
        }
    }
}
