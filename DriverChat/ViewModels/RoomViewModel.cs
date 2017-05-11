using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace DriverChat.ViewModels {
    class RoomViewModel {
        private ObservableCollection<Models.Roomitems> allItems = new ObservableCollection<Models.Roomitems>();
        public ObservableCollection<Models.Roomitems> AllItems { get { return this.allItems; } }
        private Models.Roomitems selectedItem = default(Models.Roomitems);
        public Models.Roomitems SelectedItem { get { return selectedItem; } set { this.selectedItem = value; } }
        private static RoomViewModel ins = null;
        public static RoomViewModel CreateView() {
            if (ins == null) {
                ins = new RoomViewModel();
            }
            return ins;
        }
        private RoomViewModel() {
            /*
            List<Models.Useritems> Temp = new List<Models.Useritems>();
            Temp.Add(new Models.Useritems(1,"fuck+C", new BitmapImage(new Uri("ms-appx:Assets/bg.jpg"))));
            Temp.Add(new Models.Useritems(2, "fuckjjh", new BitmapImage(new Uri("ms-appx:Assets/bg.jpg"))));
            Temp.Add(new Models.Useritems(3, "fuck+A", new BitmapImage(new Uri("ms-appx:Assets/bg.jpg"))));
            Temp.Add(new Models.Useritems(4, "fuck+B", new BitmapImage(new Uri("ms-appx:Assets/bg.jpg"))));
            Temp.Add(new Models.Useritems(5, "fuck+D", new BitmapImage(new Uri("ms-appx:Assets/bg.jpg"))));
            Temp.Add(new Models.Useritems(6, "fuck+E", new BitmapImage(new Uri("ms-appx:Assets/bg.jpg"))));
            Temp.Add(new Models.Useritems(7, "fuck+F", new BitmapImage(new Uri("ms-appx:Assets/bg.jpg"))));
            Temp.Add(new Models.Useritems(8, "fuck+G", new BitmapImage(new Uri("ms-appx:Assets/bg.jpg"))));
            Temp.Add(new Models.Useritems(9, "fuck+H", new BitmapImage(new Uri("ms-appx:Assets/bg.jpg"))));
            allItems.Add(new Models.Roomitems(1,"test1", 10, "Nothing to say", "Fuck Boy", null, Temp));
            allItems.Add(new Models.Roomitems(2,"test2", 10, "Nothing to say", "Fuck Me", null, Temp));
            allItems.Add(new Models.Roomitems(3,"test3", 10, "Nothing to say", "Fuck Me", null, Temp));
            allItems.Add(new Models.Roomitems(4,"test4", 10, "Nothing to say", "Fuck Me", null, Temp));
            */
            DriverChat.Socket.Client.GetClient().Ask_For_Roomlist();
            DriverChat.Socket.Client.GetClient().GotRoom += (rid, name, direction, activeness, created_at) => {
                allItems.Add(new Models.Roomitems(rid, name, activeness, direction, created_at));
            };
            DriverChat.Socket.Client.GetClient().LostRoom += (rid, name, direction, activeness, created_at) => {
                for (int i = 0; i < allItems.Count(); i++) {
                    if (allItems[i].GetId() == rid) {
                        allItems.RemoveAt(i);
                        break;
                    }
                }
            };
            DriverChat.Socket.Client.GetClient().GotDriver += (rid, did, nickname, badge) => {
                foreach (var i in allItems) {
                    if (i.GetId() == rid) {
                        i.AddUser(did, nickname, badge);
                        break;
                    }
                }
            };



            DriverChat.Socket.Client.GetClient().LostDriver += (rid, did, nickname, badge) => {
                foreach (var i in allItems) {
                    if (i.GetId() == rid) {
                        i.RemoveUser(did);
                        break;
                    }
                }

            };
            DriverChat.Socket.Client.GetClient().GotRoomAvatar += (rid, image) => {
                foreach (var i in allItems) {
                    if (i.GetId() == rid) {
                        i.RoomPic = image;
                        break;
                    }
                }
            };
        }
        public void Add_ALL_Pics() {
            DriverChat.Socket.Client.GetClient().Ask_For_RoomImage();
        }
    }
}
