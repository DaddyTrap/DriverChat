using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace DriverChat.Models
{
    class Useritems
    {
        BitmapImage default_pic = new BitmapImage(new Uri("ms-appx:Assets/bg.jpg"));
        public string nickname;
        private int uid;
        public string badge;
        public ImageSource ImaSrc;
        public string CurMsg = "sb";
        public Useritems(int id, string nname, string ba)
        {
            uid = id;
            nickname = nname;
            badge = ba;
            ImaSrc = default_pic;
        }
        public int GetId()
        {
            return uid;
        }
    }
}
