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
        public string nickname;
        private int uid;
        public ImageSource ImaSrc;
        public string CurMsg = "sb";
        public Useritems(int id, string nname, BitmapImage Is)
        {
            uid = id;
            Is.DecodePixelHeight = 50;
            Is.DecodePixelWidth = 50;
            nickname = nname;
            ImaSrc = Is;
        }
        public void ReceiveMsg()
        {

        }
    }
}
