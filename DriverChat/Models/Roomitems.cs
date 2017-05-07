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
    class Roomitems 
    {
        BitmapImage defaultPic = new BitmapImage(new Uri("ms-appx:Assets/bg.jpg"));
        private int rid;
        public string RoomName;
        public int Speed;
        public string Brief_Intro;
        public string CreateTime;
        public string Direction;
        public ImageSource RoomPic;
        public ObservableCollection<Useritems> CurrentUser = new ObservableCollection<Useritems>();
        public ObservableCollection<Msg> CurrentMsg = new ObservableCollection<Msg>();

        public Roomitems(int id, string rN, int sP, string dR, string Ct)
        {
            rid = id;
            RoomName = rN;
            Speed = sP;
            RoomPic = defaultPic;
            Direction = dR;
            CreateTime = Ct;
            DriverChat.Socket.Client.GetClient().GotDriver += (rid, did, nickname, badge) =>
            {
                if (rid != this.GetId()) return;
                bool flag = true;
                for (int i = 0; i < CurrentUser.Count(); i++)
                {
                    if (CurrentUser[i].GetId() == did)
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    CurrentUser.Add(new Useritems(did, nickname, badge));
                }
            };
            DriverChat.Socket.Client.GetClient().LostDriver += (rid, did, nickname, badge) =>
            {
                if (rid != this.GetId()) return;
                for (int i = 0; i < CurrentUser.Count(); i++)
                {
                    if (CurrentUser[i].GetId() == did)
                    {
                        CurrentUser.RemoveAt(i);
                        break;
                    }
                }

            };
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
        public int GetId()
        {
            return rid;
        }
    }
}
