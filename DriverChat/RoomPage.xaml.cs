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
using Windows.UI.Popups;
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

    }
    void AskImage() {
      DriverChat.Socket.Client.GetClient().Ask_For_DriverImage();
    }
    void HandleRecieveMsg(int from, string msg) {
      ViewModel.SelectedItem.RecivedMsg(msg, from);
      //DriverChat.Socket.Client.GetClient().Ask_For_UserImage();
    }
    void HandleRecieveImgMsg(int from, ImageSource msg) {
      ViewModel.SelectedItem.RecivedImgMsg(msg, from);
      //DriverChat.Socket.Client.GetClient().Ask_For_UserImage();
    }
    protected override void OnNavigatedTo(NavigationEventArgs e) {
      ViewModel = (ViewModels.RoomViewModel)e.Parameter;
      RName.Text = ViewModel.SelectedItem.RoomName;
      this.DataContext = ViewModel.SelectedItem;

      DriverChat.Socket.Client.GetClient().GotMessage += HandleRecieveMsg;
      DriverChat.Socket.Client.GetClient().GotDriverList += AskImage;
      DriverChat.Socket.Client.GetClient().GotChatImage += HandleRecieveImgMsg;
      DriverChat.Socket.Client.GetClient().Ask_For_Driverlist();
    }
    protected override void OnNavigatedFrom(NavigationEventArgs e) {
      DriverChat.Socket.Client.GetClient().Quit_Room_json();
      DriverChat.Socket.Client.GetClient().GotMessage -= HandleRecieveMsg;
      DriverChat.Socket.Client.GetClient().GotDriverList -= AskImage;
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

    private async void CheckPic(object sender, ItemClickEventArgs e) {
      if ((e.ClickedItem as DriverChat.Models.Msg).IsPic == false)
        return;
      var img = (e.ClickedItem as DriverChat.Models.Msg).MsgPic;
      ContentDialog t = new ContentDialog();
      Image imgContainer = new Image();
      imgContainer.Source = img;
      imgContainer.Height = 500;
      imgContainer.Width = 500;
      t.Content = imgContainer;
      t.UseLayoutRounding = true;
      t.PrimaryButtonText = "确定";
      t.SecondaryButtonText = "保存";
      t.SecondaryButtonClick += async (t_sender, args) => {
        var savePicker = new FileSavePicker();
        savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
        savePicker.FileTypeChoices.Add("Jpg", new List<string>() { ".jpg" });
        savePicker.SuggestedFileName = "Chat File";

        Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
        if (file == null)
          return;

        IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite);
        BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
        // Get pixels of the WriteableBitmap object 
        var wr_img = img as WriteableBitmap;
        Stream pixelStream = wr_img.PixelBuffer.AsStream();
        byte[] pixels = new byte[pixelStream.Length];
        await pixelStream.ReadAsync(pixels, 0, pixels.Length);
        // Save the image file with jpg extension 
        encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)wr_img.PixelWidth, (uint)wr_img.PixelHeight, 96.0, 96.0, pixels);
        await encoder.FlushAsync();
      };
      await t.ShowAsync();
    }
    private bool ctrl_down = false;

    private void Msg_Input_KeyDown(object sender, KeyRoutedEventArgs e) {
      if (e.Key == Windows.System.VirtualKey.Control) {
        ctrl_down = true;
        e.Handled = true;
      } else if (e.Key == Windows.System.VirtualKey.Enter) {
        if (ctrl_down) {
          SendMsg(this, new RoutedEventArgs());
        }
        e.Handled = true;
      }
    }


    private void Msg_Input_KeyUp(object sender, KeyRoutedEventArgs e) {
      if (e.Key == Windows.System.VirtualKey.Control) {
        ctrl_down = false;
        e.Handled = true;
      }
    }

    private async void SearchRecord(object sender, RoutedEventArgs e) {
      string rec = ViewModel.SelectedItem.SearchMsg();
      MessageDialog t = new MessageDialog(rec);
      await t.ShowAsync();
    }

    private void DeleteRecord(object sender, RoutedEventArgs e) {
      ViewModel.SelectedItem.DeleteMsg();
    }
  }
}
