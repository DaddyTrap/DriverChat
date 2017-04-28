using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        private string rid;
        public string RoomName;
        public int Speed;
        public string Brief_Intro;
        public string CreateTime;
        public string Direction;
        public ImageSource RoomPic;
        public ObservableCollection<Useritems> CurrentUser = new ObservableCollection<Useritems>();
        public List<Msg> CurrentMsg = new List<Msg>();
        public Roomitems(string id, string rN, int sP, string bI, string dR, ImageSource rP, List<Models.Useritems> list)
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
            m.Comment += "改啊ssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss";
            m.IsSelf = false;
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
                CurrentMsg.Add(me);
            }
        }
    }
}
