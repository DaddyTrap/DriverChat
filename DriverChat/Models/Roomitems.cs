using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

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
        }
    }
}
