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
        private string id;
        public ImageSource ImaSrc;
        public string CurMsg = "sb";
        public Useritems(string uid, string nname, BitmapImage Is)
        {
            id = uid;
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
