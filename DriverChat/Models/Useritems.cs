using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace DriverChat.Models {
  class Useritems : INotifyPropertyChanged {
    BitmapImage default_pic = new BitmapImage(new Uri("ms-appx:Assets/bg.jpg"));
    private string nickname_;
    public string nickname {
      get {
        return nickname_;
      }
      set {
        nickname_ = value;
        OnPropertyChanged();
      }
    }
    private int uid;
    public string badge;
    private ImageSource ImaSrc_;
    public ImageSource ImaSrc { get { return ImaSrc_; } set { ImaSrc_ = value; OnPropertyChanged(); } }
    public string CurMsg = "sb";
    public Useritems(int id, string nname, string ba) {
      uid = id;
      nickname = nname;
      badge = ba;
      ImaSrc = default_pic;

      DriverChat.Socket.Client.GetClient().GotDriverAvatar += (uid, image) => {
        if (uid != this.uid)
          return;

        ImaSrc = image;
      };

    }
    public int GetId() {
      return uid;
    }
    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged([CallerMemberName]string propertyName = "") {
      if (PropertyChanged != null) {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }
  }
}
