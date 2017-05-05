using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace DriverChat.ViewModels
{
    class NewBornRoom
    {
        private ObservableCollection<Models.Roomitems> allItems = new ObservableCollection<Models.Roomitems>();
        public ObservableCollection<Models.Roomitems> AllItems { get { return this.allItems; } }
        private Models.Roomitems selectedItem = default(Models.Roomitems);
        public Models.Roomitems SelectedItem { get { return selectedItem; } set { this.selectedItem = value; } }
        private static NewBornRoom ins = null;
        public static NewBornRoom CreateView()
        {
            if (ins == null)
            {
                ins = new NewBornRoom();
            }
            return ins;
        }
        private NewBornRoom()
        {
            List<Models.Useritems> Temp = new List<Models.Useritems>();
            Temp.Add(new Models.Useritems(1, "fuck+C", new BitmapImage(new Uri("ms-appx:Assets/bg.jpg"))));
            Temp.Add(new Models.Useritems(2, "fuckjjh", new BitmapImage(new Uri("ms-appx:Assets/bg.jpg"))));
            Temp.Add(new Models.Useritems(3, "fuck+A", new BitmapImage(new Uri("ms-appx:Assets/bg.jpg"))));
            Temp.Add(new Models.Useritems(4, "fuck+B", new BitmapImage(new Uri("ms-appx:Assets/bg.jpg"))));
            Temp.Add(new Models.Useritems(5, "fuck+D", new BitmapImage(new Uri("ms-appx:Assets/bg.jpg"))));
            Temp.Add(new Models.Useritems(6, "fuck+E", new BitmapImage(new Uri("ms-appx:Assets/bg.jpg"))));
            Temp.Add(new Models.Useritems(7, "fuck+F", new BitmapImage(new Uri("ms-appx:Assets/bg.jpg"))));
            Temp.Add(new Models.Useritems(8, "fuck+G", new BitmapImage(new Uri("ms-appx:Assets/bg.jpg"))));
            Temp.Add(new Models.Useritems(9, "fuck+H", new BitmapImage(new Uri("ms-appx:Assets/bg.jpg"))));
            allItems.Add(new Models.Roomitems(5, "test5", 10, "Nothing to say", "Fuck Boy", null, Temp));
            allItems.Add(new Models.Roomitems(6, "test6", 10, "Nothing to say", "Fuck Me", null, Temp));
            allItems.Add(new Models.Roomitems(7, "test7", 10, "Nothing to say", "Fuck Me", null, Temp));
            allItems.Add(new Models.Roomitems(8, "test8", 10, "Nothing to say", "Fuck Me", null, Temp));
        }
    }
}
