using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace DriverChat {
  /// <summary>
  /// 可用于自身或导航至 Frame 内部的空白页。
  /// </summary>
  public sealed partial class RoomPage : Page {
    ViewModels.RoomViewModel ViewModel;
    public RoomPage() {
      this.InitializeComponent();
      var viewTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
      viewTitleBar.BackgroundColor = Windows.UI.Colors.CornflowerBlue;
      viewTitleBar.ButtonBackgroundColor = Windows.UI.Colors.CornflowerBlue;

      // DriverChat.Socket.Client.GetClient().Listener();

      DriverChat.Socket.Client.GetClient().GotMessage += HandleRecieveMsg;
      DriverChat.Socket.Client.GetClient().Ask_For_Driverlist();
      DriverChat.Socket.Client.GetClient().GotDriverList += AskImage;
      DriverChat.Socket.Client.GetClient().GotChatImage += HandleRecieveImgMsg;
    }
    void AskImage() {
      DriverChat.Socket.Client.GetClient().Ask_For_DriverImage();
    }
    void HandleRecieveMsg(int from, string msg) {
      ViewModel.SelectedItem.RecivedMsg(msg, from);
      //DriverChat.Socket.Client.GetClient().Ask_For_UserImage();
    }
    void HandleRecieveImgMsg(int from, BitmapImage msg) {
      ViewModel.SelectedItem.RecivedImgMsg(msg, from);
      //DriverChat.Socket.Client.GetClient().Ask_For_UserImage();
    }
    protected override void OnNavigatedTo(NavigationEventArgs e) {
      ViewModel = (ViewModels.RoomViewModel)e.Parameter;
      RName.Text = ViewModel.SelectedItem.RoomName;
      this.DataContext = ViewModel.SelectedItem;
    }
    protected override void OnNavigatedFrom(NavigationEventArgs e) {
      DriverChat.Socket.Client.GetClient().Quit_Room_json();
      DriverChat.Socket.Client.GetClient().GotMessage -= HandleRecieveMsg;
      DriverChat.Socket.Client.GetClient().GotChatImage -= HandleRecieveImgMsg;
    }
    private void SendMsg(object sender, RoutedEventArgs e) {
      string Msg = Msg_Input.Text;
      Msg_Input.Text = "";
      ViewModel.SelectedItem.SendMsg(Msg);
    }

    private void NewMsgCome(object sender, SizeChangedEventArgs e) {
      double d = MsgList.ActualHeight;
      MsgRoll.ScrollToVerticalOffset(d);
    }

    private async void SendImg(object sender, RoutedEventArgs e) {
      FileOpenPicker openPicker = new FileOpenPicker();
      openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
      openPicker.ViewMode = PickerViewMode.List;
      openPicker.FileTypeFilter.Add(".jpg");
      openPicker.FileTypeFilter.Add(".jpeg");
      openPicker.FileTypeFilter.Add(".png");
      StorageFile file = await openPicker.PickSingleFileAsync();
      if (file != null) {
        using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read)) {
          // Set the image source to the selected bitmap
          var tempStream = fileStream.AsStream();
          byte[] s = new byte[tempStream.Length];
          await tempStream.ReadAsync(s, 0, s.Length);
          tempStream.Seek(0, SeekOrigin.Begin);
          DriverChat.Socket.Client.GetClient().Create_Chat_Image_json(s.Length, s, ViewModel.SelectedItem.GetId());
        }
      }
    }
  }
}
