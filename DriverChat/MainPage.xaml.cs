using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
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
        public MainPage()
        {
            this.InitializeComponent();
            this.ViewModel = DriverChat.ViewModels.RoomViewModel.CreateView();
            
            Control.CurrentUser c = Resources["CurrentUser"] as Control.CurrentUser;
            c = Control.CurrentUser.GetCurrentUser();

        }

        private void Roomitem_Click(object sender, ItemClickEventArgs e)
        {
            ViewModel.SelectedItem = (Models.Roomitems)e.ClickedItem;
            DriverChat.Socket.Client.GetClient().Enter_Room_json(ViewModel.SelectedItem.GetId());
            Frame.Navigate(typeof(RoomPage), ViewModel);
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            LeftSplit.IsPaneOpen = !LeftSplit.IsPaneOpen;
        }

        private async void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var a = e.ClickedItem as StackPanel;
            if (a.Name == "Head")
                await ResetUserHeadPic();
            if (a.Name == "AddRoom")
                Frame.Navigate(typeof(CreateRoom));
        }
        private async Task ResetUserHeadPic() {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.ViewMode = PickerViewMode.List;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            Control.CurrentUser c = Resources["CurrentUser"] as Control.CurrentUser;
            StorageFile file = await openPicker.PickSingleFileAsync();
            using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
            {
                // Set the image source to the selected bitmap
                BitmapImage bitmapImage = new BitmapImage();
                await bitmapImage.SetSourceAsync(fileStream);
                c.SetHeadPic(bitmapImage);
            }
        }
    }

    //
}
