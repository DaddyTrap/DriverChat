using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace DriverChat.Models
{
    /*
    <Image x:Name="RoomPic_" />
    <TextBlock x:Name="RoomName" />
    <TextBlock x:Name="Brief_Intro" />
    <TextBlock x:Name="Speed" />
    <TextBlock x:Name="CreateTime" />
    <TextBlock x:Name="Direction" />
    */
    class Roomitems : INotifyPropertyChanged
    {
        private int rid;
        public string RoomName;
        public int Speed;
        public string Brief_Intro;
        public string CreateTime;
        public string Direction;
        public ImageSource RoomPic;
        public ObservableCollection<Useritems> CurrentUser = new ObservableCollection<Useritems>();
        public ObservableCollection<Msg> CurrentMsg = new ObservableCollection<Msg>();
        public Roomitems(int id, string rN, int sP, string bI, string dR, ImageSource rP, List<Models.Useritems> list)
        {
            rid = id;
            RoomName = rN;
            Speed = sP;
            Brief_Intro = bI;
            RoomPic = rP;
            Direction = dR;
            for (int i = 0; i < DateTimeOffset.Now.ToString().IndexOf('+'); i++)
                CreateTime += DateTimeOffset.Now.ToString()[i];
            for (int i = 0; i < list.Count(); i++)
                CurrentUser.Add(list[i]);
            Msg m = new Msg();
            m.Comment += "改啊ssssssssssssssssssss";
            m.IsSelf = false;
            m.IsPic = false;
            BitmapImage b = new BitmapImage(new Uri("ms-appx:Assets/bg.jpg"));
            b.DecodePixelHeight = 50;
            b.DecodePixelWidth = 50;
            m.HeadPic = b;
            CurrentMsg.Add(m);
            for (int i = 0; i < 10; i++)
            {
                Msg me = new Msg();
                me.Comment += "改个吉吉啊";
                me.IsSelf = true;
                me.HeadPic = b;
                me.IsPic = false;
                CurrentMsg.Add(me);
            }
            Msg o = new Msg();
            o.Comment += "改个吉吉啊";
            o.IsSelf = true;
            o.HeadPic = b;
            o.IsPic = true;
            o.MsgPic = b;
            CurrentMsg.Add(o);

        }
        public void SendMsg(string msg)
        {
            // CurrentMsg.Add(newOne);
            DriverChat.Socket.Client.GetClient().Create_Chat_json(msg, this.rid);
        }
        public void RecivedMsg(string msg, int from)
        {
            Msg Come = new Msg();
            Come.Comment += msg;
            if (DriverChat.Control.CurrentUser.GetCurrentUser().GetId() == from)
            {
                Come.IsSelf = true;
                Come.HeadPic = DriverChat.Control.CurrentUser.GetCurrentUser().HeadPic;
            }
            else
            {
                Come.IsSelf = false;
                for (int i = 0; i < CurrentUser.Count(); i++)
                    if (CurrentUser[i].GetId() == from)
                        Come.HeadPic = CurrentUser[i].ImaSrc;
            }
            CurrentMsg.Add(Come);
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
