using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverChat.Models;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DriverChat.Control {
    class CurrentUser : INotifyPropertyChanged {
        private int UserId { get; }
        private string UserName_;
        private ImageSource HeadPic_;
        private ImageSource badge_;
        public ImageSource HeadPic {
            get {
                return HeadPic_;
            }
            set {
                HeadPic_ = value;
                OnPropertyChanged();
            }
        }
        public string UserName {
            get {
                return UserName_;
            }
            set {
                UserName_ = value;
                OnPropertyChanged();
            }
        }
        public ImageSource badge {
            get {
                return badge_;
            }
            set {
                badge_ = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Roomitems> AllRooms = new ObservableCollection<Roomitems>();
        public ObservableCollection<Roomitems> Allrooms { get { return this.AllRooms; } }
        static CurrentUser ins = null;
        static public void CreateUser(int id, string name, string badge = "1") {
            /*
            if (ins == null)
                ins = new CurrentUser(id, name, Is);
            */
            if (ins == null)
                ins = new CurrentUser(id, name, badge);
        }
        static public CurrentUser GetCurrentUser() {
            return ins;
        }

        private CurrentUser(int id, string name, string ba) {
            UserId = id;
            UserName = name;
            string badgeUri = "ms-appx:Assets/" + ba + ".png";
            HeadPic = new BitmapImage(new Uri("ms-appx:Assets/bg.jpg"));  // default
            badge = new BitmapImage(new Uri(badgeUri));
            DriverChat.Socket.Client.GetClient().GotDriverAvatar += (uid, image) => {
                if (uid != this.UserId)
                    return;
                HeadPic = image;
            };
            DriverChat.Socket.Client.GetClient().Ask_For_UserImage();
            DriverChat.Socket.Client.GetClient().Update_User_Avatar_Succeed += () => {
                // DriverChat.Socket.Client.GetClient().Ask_For_UserImage();
            };
        }

        public void SetHeadPic(ImageSource Is) {
            HeadPic = Is;
        }

        public int GetId() {
            return UserId;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName = "") {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
