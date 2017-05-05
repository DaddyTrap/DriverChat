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

namespace DriverChat.Control
{
    class CurrentUser : INotifyPropertyChanged
    {
        private int UserId { get; }
        private string UserName_;
        private ImageSource HeadPic_;
        public ImageSource HeadPic {
            get
            {
                return HeadPic_;
            }
            set
            {
                HeadPic_ = value;
                OnPropertyChanged();
            }
        }
        public string UserName
        {
            get
            {
                return UserName_;
            }
            set
            {
                UserName_ = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Roomitems> AllRooms = new ObservableCollection<Roomitems>();
        public ObservableCollection<Roomitems> Allrooms { get { return this.AllRooms; } }
        static CurrentUser ins = null;
        static public void CreateUser(string id, string name, ImageSource Is)
        {
            /*
            if (ins == null)
                ins = new CurrentUser(id, name, Is);
            */
            if (ins == null)
                ins = new CurrentUser();
        }
        static public CurrentUser GetCurrentUser()
        {
            if (ins == null)
                ins = new CurrentUser();
            return ins;
        }
        
        public CurrentUser(int id, string name, ImageSource Is)
        {
            UserId = id;
            UserName = name;
            HeadPic = Is;
        }
        public void SetHeadPic(ImageSource Is)
        {
            HeadPic = Is;
        }
        public CurrentUser()
        {
            UserId = 1;
            UserName = "LJJ";
            HeadPic = new BitmapImage(new Uri("ms-appx:Assets/bg.jpg"));
        }
        public int GetId()
        {
            return UserId;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
