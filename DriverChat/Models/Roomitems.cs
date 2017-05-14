using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace DriverChat.Models {
    /*
    <Image x:Name="RoomPic_" />
    <TextBlock x:Name="RoomName" />
    <TextBlock x:Name="Brief_Intro" />
    <TextBlock x:Name="Speed" />
    <TextBlock x:Name="CreateTime" />
    <TextBlock x:Name="Direction" />
    */
    class Roomitems : INotifyPropertyChanged {
        BitmapImage defaultPic = new BitmapImage(new Uri("ms-appx:Assets/bg.jpg"));
        private int rid;

        private string RoomName_;
        private int Speed_;
        private string Brief_Intro_;
        private string CreateTime_;
        private string Direction_;
        private ImageSource RoomPic_;

        public string RoomName { get { return RoomName_; } set { RoomName_ = value; OnPropertyChanged(); } }
        public int Speed { get { return Speed_; } set { Speed_ = value; OnPropertyChanged(); } }
        public string Brief_Intro { get { return Brief_Intro_; } set { Brief_Intro_ = value; OnPropertyChanged(); } }
        public string CreateTime { get { return CreateTime_; } set { CreateTime_ = value; OnPropertyChanged(); } }
        public string Direction { get { return Direction_; } set { Direction_ = value; OnPropertyChanged(); } }
        public ImageSource RoomPic { get { return RoomPic_; } set { RoomPic_ = value; OnPropertyChanged(); } }
        public ObservableCollection<Useritems> CurrentUser = new ObservableCollection<Useritems>();
        public ObservableCollection<Msg> CurrentMsg = new ObservableCollection<Msg>();

        public Roomitems(int id, string rN, int sP, string dR, string Ct) {
            rid = id;
            RoomName = rN;
            Speed = sP;
            RoomPic = defaultPic;
            Direction = dR;
            CreateTime = Ct;

        }
        public void SendMsg(string msg) {
            // CurrentMsg.Add(newOne);
            DriverChat.Socket.Client.GetClient().Create_Chat_json(msg, this.rid);
        }
        public void RecivedMsg(string msg, int from) {
            Msg Come = new Msg();
            Come.Comment += msg;
            Come.IsPic = false;
            if (DriverChat.Control.CurrentUser.GetCurrentUser().GetId() == from) {
                Come.IsSelf = true;
                Come.HeadPic = DriverChat.Control.CurrentUser.GetCurrentUser().HeadPic;
                Come.username = DriverChat.Control.CurrentUser.GetCurrentUser().UserName;
            } else {
                Come.IsSelf = false;
                for (int i = 0; i < CurrentUser.Count(); i++)
                    if (CurrentUser[i].GetId() == from) {
                        Come.HeadPic = CurrentUser[i].ImaSrc;
                        Come.username = CurrentUser[i].nickname;
                    }
            }
            CurrentMsg.Add(Come);
            update_message(Come);
            DriverChat.DataServe.DataService.GetdbIns().Insert(Come, this.rid);
        }
        public void RecivedImgMsg(BitmapImage t, int from) {
            Msg Come = new Msg();
            Come.MsgPic = t;
            Come.IsPic = true;
            if (DriverChat.Control.CurrentUser.GetCurrentUser().GetId() == from) {
                Come.IsSelf = true;
                Come.HeadPic = DriverChat.Control.CurrentUser.GetCurrentUser().HeadPic;
                Come.username = DriverChat.Control.CurrentUser.GetCurrentUser().UserName;
            } else {
                Come.IsSelf = false;
                for (int i = 0; i < CurrentUser.Count(); i++)
                    if (CurrentUser[i].GetId() == from) {
                        Come.HeadPic = CurrentUser[i].ImaSrc;
                        Come.username = CurrentUser[i].nickname;
                    }
            }
            CurrentMsg.Add(Come);
            update_message(Come);
            DriverChat.DataServe.DataService.GetdbIns().Insert(Come, this.rid);
        }
        public int GetId() {
            return rid;
        }
        public void Ask_For_Image() {
            DriverChat.Socket.Client.GetClient().Ask_For_RoomImage();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName = "") {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public void AddUser(int did,string nickname,string badge) {
            bool flag = true;
            for (int i = 0; i < CurrentUser.Count(); i++) {
                if (CurrentUser[i].GetId() == did) {
                    flag = false;
                    break;
                }
            }
            if (flag) {
                CurrentUser.Add(new Useritems(did, nickname, badge));
            }
        }
        public void RemoveUser(int did) {
            for (int i = 0; i < CurrentUser.Count(); i++) {
                if (CurrentUser[i].GetId() == did) {
                    CurrentUser.RemoveAt(i);
                    break;
                }
            }
        }
        private void update_message(Msg t) {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText("tile.xml"));

            XmlNodeList textElements = doc.GetElementsByTagName("text");
            XmlNodeList imageElements = doc.GetElementsByTagName("image");

            ((XmlElement)textElements[0]).InnerText = t.username;
            ((XmlElement)textElements[1]).InnerText = t.IsPic == true ? "[Picture]" : t.Comment;
            ((XmlElement)textElements[2]).InnerText = t.username;
            ((XmlElement)textElements[3]).InnerText = t.IsPic == true ? "[Picture]" : t.Comment;
            ((XmlElement)textElements[4]).InnerText = t.username;
            ((XmlElement)textElements[5]).InnerText = t.IsPic == true ? "[Picture]" : t.Comment;
            ((XmlElement)textElements[6]).InnerText = t.username;
            ((XmlElement)textElements[7]).InnerText = t.IsPic == true ? "[Picture]" : t.Comment;
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            var notification = new TileNotification(doc);
            updater.Clear();
            updater.Update(notification);
            updater.EnableNotificationQueue(true);
        }
        public void DeleteMsg() {
            DriverChat.DataServe.DataService.GetdbIns().Remove(this.rid);
            CurrentMsg.Clear();
        }
        public string SearchMsg() {
            return DriverChat.DataServe.DataService.GetdbIns().Search(this.rid);
        }
    }
}
