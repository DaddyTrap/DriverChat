using DriverChat.Models;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace DriverChat.DataServe {
    class DataService {
        private static SQLiteConnection db = null;
        private static DataService ins = null;
        private static Boolean LoadStatus = false;
        public static DataService GetdbIns() {
            if (ins == null)
                return ins = new DataService();
            return ins;
        }
        private DataService() {
        }
        public void LoadDatabase() {
            if (db == null)
                db = new SQLiteConnection("ChatRecord.db");
            string sql = @"
                            CREATE TABLE IF NOT EXISTS
                            ChatRecord (
                                rid VARCHAR(1024) ,
                                Comment       VARCHAR(1024),
                                Username VARCHAR(1024)
                            );";
            using (var statement = db.Prepare(sql)) {
                statement.Step();
            }
        }

        public void Insert(Msg it, int rid) {
            using (var statement = db.Prepare("INSERT INTO ChatRecord (rid, Comment, Username) VALUES (?, ?, ?);")) {
                statement.Bind(1, rid.ToString());
                statement.Bind(2, (it.IsPic == true ? "[Picture]" : it.Comment));
                statement.Bind(3, it.username);
                statement.Step();
            }
        }
        public void Remove(int rid) {
            using (var statement = db.Prepare("DELETE FROM ChatRecord WHERE rid = ?;")) {
                statement.Bind(1, rid.ToString());
                statement.Step();
            }
        }

        public string Search(int rid) {
            var message = new StringBuilder();
            using (var statement = db.Prepare("SELECT Comment, Username FROM ChatRecord WHERE rid = ?")) {
                statement.Bind(1, rid.ToString());

                while (SQLiteResult.ROW == statement.Step()) {
                    message.Append(statement[1]);
                    message.Append(" : ");
                    message.Append(statement[0]);
                    message.Append("\n");
                }
            }
            return message.ToString();
        }

    }

}
