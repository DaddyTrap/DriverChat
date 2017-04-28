using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace DriverChat
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        DriverChat.ViewModels.RoomViewModel ViewModel { get; set; }
        DriverChat.ViewModels.NewBornRoom NewRooms { get; set; }
        public MainPage()
        {
            this.InitializeComponent();
            ApplicationView.PreferredLaunchViewSize = new Size(800, 500);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            this.ViewModel = DriverChat.ViewModels.RoomViewModel.CreateView();
            this.NewRooms = DriverChat.ViewModels.NewBornRoom.CreateView();
        }

        private void Roomitem_Click(object sender, ItemClickEventArgs e)
        {
            ViewModel.SelectedItem = (Models.Roomitems)e.ClickedItem;
            Frame.Navigate(typeof(RoomPage), ViewModel);
        }

    }

    //
}
