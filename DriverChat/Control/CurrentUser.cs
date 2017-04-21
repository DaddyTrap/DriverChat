using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverChat.Models;
using System.Collections.ObjectModel;

namespace DriverChat.Models
{
    class CurrentUser
    {
        private string UserId;
        private string UserName;
        private ObservableCollection<Roomitems> AllRooms = new ObservableCollection<Roomitems>();
        public ObservableCollection<Roomitems> Allrooms { get { return this.AllRooms; } }
        static public CurrentUser ins = null;
        static void CreateUser(string id, string name, List<Roomitems> r)
        {
            if (ins == null)
                ins = new CurrentUser(id, name, r);
        }
        static CurrentUser GetCurrentUser()
        {
            return ins;
        }
        private CurrentUser(string id, string name, List<Roomitems> r)
        {
            UserId = id;
            UserName = name;
            for (int i = 0; i < r.Count; i++)
            {
                AllRooms.Add(r[i]);
            }
        }
    }
}
