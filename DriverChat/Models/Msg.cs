using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace DriverChat.Models {
    class Msg {
        public string Comment { get; set; }
        public ImageSource HeadPic { get; set; }
        public bool IsSelf { get; set; }
        public bool IsPic { get; set; }
        public ImageSource MsgPic { get; set; }
    }

}
