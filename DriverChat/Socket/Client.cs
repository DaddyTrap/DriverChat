using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Diagnostics;

namespace DriverChat.Socket {
  public class Client {
    public string username, name, badge, created_at;
    bool status;

    public Windows.Networking.Sockets.StreamSocket clientsocket;
    public Windows.Networking.HostName serverHost;
    public string serverPort;
    Stream streamIn;
    StreamReader reader;
    bool working;
    string msg;

    public int cur_rid, did;

    public delegate void GotMessageHandler(int chat_form, string chat_msg);
    public event GotMessageHandler GotMessage;

    public delegate void GotErrorHandler(string msg);
    public event GotErrorHandler GotSigninError;
    public event GotErrorHandler GotSigninSucceed;
    public event GotErrorHandler GotSignupError;
    public event GotErrorHandler GotSignupSucceed;
    public event GotErrorHandler GotSysError;

    public delegate void GotChatImageHandler(int from, BitmapImage Image);
    public event GotChatImageHandler GotChatImage;

    public delegate void GotAvatarHandler(int id, BitmapImage Image);
    public event GotAvatarHandler GotDriverAvatar;
    public event GotAvatarHandler GotRoomAvatar;


    public delegate void GotRoomHandler(int rid, string name, string direction, int activeness, string created_at);
    public event GotRoomHandler GotRoom;
    public event GotRoomHandler LostRoom;

    public delegate void GotDriverHandler(int rid, int did, string nickname, string badge);
    public event GotDriverHandler GotDriver;
    public event GotDriverHandler LostDriver;

    public delegate void GotListHandler();
    public event GotListHandler GotDriverList;
    public event GotListHandler GotRoomList;


    // public delegate void GotDriverHandler(int rid, int did, string nickname, string badge, ava);
    //        public event GotRoomHandler GotDrier;

    private static Client ins = null;

    public static Client GetClient() {                                                          ///instance
      if (ins == null) {
        ins = new Client("9999", "119.29.238.158");
      }
      return ins;
    }

    List<int> Room_list = new List<int>();
    List<int> Driver_list = new List<int>();

    public Client(string _port, string HostIp) {
      serverPort = _port;
      working = true;
      clientsocket = new Windows.Networking.Sockets.StreamSocket();
      serverHost = new Windows.Networking.HostName(HostIp);
      Connection();

    }
    private async void Connection() {
      await clientsocket.ConnectAsync(serverHost, serverPort);
    }
    int min(int x, int y) { return x < y ? x : y; }

    public async void Listener() {
      //try {

      //GotMessage(0, "connect succeed");                                                  ///for console connect status
      streamIn = clientsocket.InputStream.AsStreamForRead();
      reader = new StreamReader(streamIn);

      byte[] buffer = new byte[4096];
      int last_data_count = 0;
      int offset = 0;
      string str_msg;
      int newline = 0;

      while (working) {
        last_data_count = await streamIn.ReadAsync(buffer, offset, 1024);
        offset += last_data_count;
        str_msg = System.Text.Encoding.UTF8.GetString(buffer);
        newline = str_msg.IndexOf('\n');
        bool isFile = false;

        bool flag = false;

        while (newline != -1) {
          str_msg = str_msg.Substring(0, newline + 1);
          JObject list = (JObject)JsonConvert.DeserializeObject(str_msg);

          // Cut the json bytes
          byte[] json_bytes = System.Text.Encoding.UTF8.GetBytes(str_msg);
          int json_len = json_bytes.Length;

          //buffer.CopyTo(buffer, json_len);
          Array.Copy(buffer, json_len, buffer, 0, offset);
          buffer[offset] = 0;
          offset = offset - json_len;

          if (list["type"].ToString() == "sys") {                                         ///handle system response
            if (list["detail"].ToString() == "sign in") {                                   ///handle signin
              status = list["status"].ToString() == "True" ? true : false;
              msg = list["msg"].ToString();
              if (status) {
                did = Convert.ToInt32(list["driver"]["did"].ToString());
                name = list["driver"]["name"].ToString();
                badge = list["driver"]["badge"].ToString();
                created_at = list["driver"]["created_at"].ToString();
                this.GotSigninSucceed(msg);
              } else {                                                                    ///handle singin error
                GotSigninError(msg);
              }
            } else if (list["detail"].ToString() == "sign up") {                            ///handle signup
              status = list["status"].ToString() == "True" ? true : false;
              msg = list["msg"].ToString();
              if (!status) {                                                              ///handle signup error
                GotSignupError(msg);
              } else
                GotSignupSucceed(msg);
            } else if (list["detail"].ToString() == "room list") {                          ///handle roomlist (need test)

              List<int> temp_list = new List<int>();

              foreach (var item in list["rooms"]) {
                temp_list.Add(Convert.ToInt32(item["rid"].ToString()));
                flag = false;
                foreach (var i in Room_list)
                  if (i == Convert.ToInt32(item["rid"].ToString()))
                    flag = true;
                if (!flag)
                  GotRoom(Convert.ToInt32(item["rid"].ToString()), item["name"].ToString(), item["direction"].ToString(),
                          Convert.ToInt32(item["activeness"].ToString()), item["created_at"].ToString());
                //GotMessage(0, item.ToString());
              }
              foreach (var i in Room_list) {
                flag = false;
                foreach (var j in temp_list)
                  if (i == j)
                    flag = true;
                if (!flag)
                  LostRoom(i, "", "", 0, "");
              }
              Room_list = temp_list;
              DriverChat.ViewModels.RoomViewModel.CreateView().Add_ALL_Pics();
            } else if (list["detail"].ToString() == "driver list") {                        ///handler driver list (need test)

              List<int> temp_list = new List<int>();
              if (Convert.ToInt32(list["rid"].ToString()) == cur_rid) {
                foreach (var item in list["drivers"]) {
                  temp_list.Add(Convert.ToInt32(item["did"].ToString()));
                  GotDriver(Convert.ToInt32(list["rid"].ToString()), Convert.ToInt32(item["did"].ToString()),
                            item["name"].ToString(), item["badge"].ToString());
                }
                foreach (int i in Driver_list) {
                  flag = false;
                  foreach (int j in temp_list)
                    if (i == j)
                      flag = true;
                  if (!flag)
                    LostDriver(cur_rid, i, "", "");
                }
                Driver_list = temp_list;

                GotDriverList();
              }
            } else if (list["detail"].ToString() == "enter room") {                         ///handle enter room
              cur_rid = Convert.ToInt32(list["rid"].ToString());
            }
          } else if (list["type"].ToString() == "chat") {                                     ///handle chat
            int chat_from = Convert.ToInt32(list["from"].ToString());
            int chat_to = Convert.ToInt32(list["to"].ToString());
            string chat_msg = list["msg"].ToString();
            if (chat_to == cur_rid)
              GotMessage(chat_from, chat_msg);
          } else if (list["type"].ToString() == "file") {                                     ///handle file(img)
            isFile = true;
            string format = list["format"].ToString();
            int length = Convert.ToInt32(list["length"].ToString());

            byte[] Imgbytes = new byte[length];
            int imgbytes_now_index = 0;

            while (length > 0) {
              if (offset >= length) {
                for (int i = 0; i < length; ++i) {
                  Imgbytes[imgbytes_now_index++] = buffer[i];
                }
                offset -= length + 1;
                Array.Copy(buffer, length + 1, buffer, 0, offset);
                // buffer.CopyTo(buffer, offset);
                buffer[offset] = 0;
                break;
              } else {
                Array.Copy(buffer, 0, Imgbytes, imgbytes_now_index, offset);
                imgbytes_now_index += offset;
                length -= offset;
                if (length > 0)
                  offset = await streamIn.ReadAsync(buffer, 0, 1024);
              }
            }

            var image = new BitmapImage();
            using (InMemoryRandomAccessStream imgstream = new InMemoryRandomAccessStream()) {
              await imgstream.WriteAsync(Imgbytes.AsBuffer());
              imgstream.Seek(0);
              await image.SetSourceAsync(imgstream);
            }
            if (list["detail"].ToString() == "driver avatar") {
              GotDriverAvatar(Convert.ToInt32(list["driver"]["did"].ToString()), image);
              Debug.WriteLine("GotDriverAvatar");
            } else if (list["detail"].ToString() == "room avatar") {
              GotRoomAvatar(Convert.ToInt32(list["room"]["rid"].ToString()), image);
              Debug.WriteLine("GotRoomAvatar");
            } else {
              GotChatImage(Convert.ToInt32(list["from"].ToString()), image);
            }
          }
          if (offset > 0) {
            str_msg = System.Text.Encoding.UTF8.GetString(buffer);
            for (int i = 0; i < str_msg.Length; ++i) {
              if (str_msg[i] == '\n') {
                newline = i;
                break;
              } else if (str_msg[i] == '\0') {
                newline = -1;
                break;
              }
            }
          } else {
            newline = -1;
          }
        }
      }

      //while (working) {
      //  int count, st = 0;
      //  byte[] c = new byte[1024 * 10240 + 300];
      //  byte[] first = new byte[1024];
      //  bool flag = false;
      //  count = await streamIn.ReadAsync(first, 0, first.Length);
      //  for (int i = 0; i < 1024; i++)
      //    c[st * 1024 + i] = first[i];
      //  st++;
      //  string str_msg = System.Text.Encoding.UTF8.GetString(first);
      //  if (str_msg[0] != '{' || str_msg == null)
      //    continue;             /*Maybe cause error*/
      //  string response = "";
      //  while (!flag) {
      //    response = "";
      //    for (int i = 0; i < str_msg.Length; i++) {
      //      response += str_msg[i];
      //      if (str_msg[i] == '\n') {
      //        flag = true;
      //        break;
      //      }
      //    }
      //    if (flag)
      //      break;
      //    count = await streamIn.ReadAsync(first, 0, first.Length);
      //    for (int i = 0; i < 1024; i++)
      //      c[st * 1024 + i] = first[i];
      //    st++;
      //    str_msg = System.Text.Encoding.UTF8.GetString(c);
      //  }
      //  if (response == null)
      //    continue;
      //  JObject list = (JObject)JsonConvert.DeserializeObject(response);
      //  if (list["type"].ToString() == "sys") {                                         ///handle system response
      //    if (list["detail"].ToString() == "sign in") {                                   ///handle signin
      //      status = list["status"].ToString() == "True" ? true : false;
      //      msg = list["msg"].ToString();
      //      if (status) {
      //        did = Convert.ToInt32(list["driver"]["did"].ToString());
      //        name = list["driver"]["name"].ToString();
      //        badge = list["driver"]["badge"].ToString();
      //        created_at = list["driver"]["created_at"].ToString();
      //        this.GotSigninSucceed(msg);
      //      } else {                                                                    ///handle singin error
      //        GotSigninError(msg);
      //      }
      //    } else if (list["detail"].ToString() == "sign up") {                            ///handle signup
      //      status = list["status"].ToString() == "True" ? true : false;
      //      msg = list["msg"].ToString();
      //      if (!status) {                                                              ///handle signup error
      //        GotSignupError(msg);
      //      } else
      //        GotSignupSucceed(msg);
      //    } else if (list["detail"].ToString() == "room list") {                          ///handle roomlist (need test)

      //      List<int> temp_list = new List<int>();

      //      foreach (var item in list["rooms"]) {
      //        temp_list.Add(Convert.ToInt32(item["rid"].ToString()));
      //        flag = false;
      //        foreach (var i in Room_list)
      //          if (i == Convert.ToInt32(item["rid"].ToString()))
      //            flag = true;
      //        if (!flag)
      //          GotRoom(Convert.ToInt32(item["rid"].ToString()), item["name"].ToString(), item["direction"].ToString(),
      //                  Convert.ToInt32(item["activeness"].ToString()), item["created_at"].ToString());
      //        //GotMessage(0, item.ToString());
      //      }
      //      foreach (var i in Room_list) {
      //        flag = false;
      //        foreach (var j in temp_list)
      //          if (i == j)
      //            flag = true;
      //        if (!flag)
      //          LostRoom(i, "", "", 0, "");
      //      }
      //      Room_list = temp_list;
      //      DriverChat.ViewModels.RoomViewModel.CreateView().Add_ALL_Pics();
      //    } else if (list["detail"].ToString() == "driver list") {                        ///handler driver list (need test)

      //      List<int> temp_list = new List<int>();

      //      if (Convert.ToInt32(list["rid"].ToString()) == cur_rid) {
      //        foreach (var item in list["drivers"]) {
      //          temp_list.Add(Convert.ToInt32(item["did"].ToString()));
      //          flag = false;
      //          /*foreach (int i in Driver_list) if (i == Convert.ToInt32(item["did"].ToString())) flag = true;*/
      //          if (!flag)
      //            GotDriver(Convert.ToInt32(list["rid"].ToString()), Convert.ToInt32(item["did"].ToString()),
      //                         item["name"].ToString(), item["badge"].ToString());
      //        }
      //        foreach (int i in Driver_list) {
      //          flag = false;
      //          foreach (int j in temp_list)
      //            if (i == j)
      //              flag = true;
      //          if (!flag)
      //            LostDriver(cur_rid, i, "", "");
      //        }
      //        Driver_list = temp_list;
      //        Ask_For_DriverImage();
      //      }
      //    } else if (list["detail"].ToString() == "enter room") {                         ///handle enter room
      //      cur_rid = Convert.ToInt32(list["rid"].ToString());
      //    }
      //  } else if (list["type"].ToString() == "chat") {                                     ///handle chat
      //    int chat_from = Convert.ToInt32(list["from"].ToString());
      //    int chat_to = Convert.ToInt32(list["to"].ToString());
      //    string chat_msg = list["msg"].ToString();
      //    if (chat_to == cur_rid)
      //      GotMessage(chat_from, chat_msg);
      //  } else if (list["type"].ToString() == "file") {                                     ///handle file(img)
      //    string format = list["format"].ToString();
      //    int length = Convert.ToInt32(list["length"].ToString());
      //    byte[] json_bytes = System.Text.Encoding.UTF8.GetBytes(response);
      //    int index = json_bytes.Length % 1024;
      //    int res_first = count - index;
      //    int res_len = length - res_first;
      //    byte[] Imgbytes = new byte[length];
      //    for (int i = 0; i < res_first; i++)
      //      Imgbytes[i] = c[i + index];
      //    index = res_first;
      //    while (res_len > 0) {
      //      count = await streamIn.ReadAsync(first, 0, min(first.Length, res_len));
      //      res_len -= count;
      //      for (int i = 0; i < count; i++)
      //        Imgbytes[index++] = first[i];
      //    }
      //    var image = new BitmapImage();
      //    using (InMemoryRandomAccessStream imgstream = new InMemoryRandomAccessStream()) {
      //      await imgstream.WriteAsync(Imgbytes.AsBuffer());
      //      imgstream.Seek(0);
      //      await image.SetSourceAsync(imgstream);
      //    }
      //    if (list["detail"].ToString() == "driver avatar") {
      //      GotDriverAvatar(Convert.ToInt32(list["driver"]["did"].ToString()), image);
      //    } else if (list["detail"].ToString() == "room avatar") {
      //      GotRoomAvatar(Convert.ToInt32(list["room"]["rid"].ToString()), image);
      //    } else {
      //      GotChatImage(Convert.ToInt32(list["from"].ToString()), Convert.ToInt32(list["to"].ToString()), image);
      //    }
      //  }
      //}
      //} catch (Exception ee) {
      //  GotSysError(ee.ToString());
      //}
    }

    public void Create_Chat_json(string words, int room_num) {
      msg = "{\"type\": \"chat\", \"msg\": \"" + words + "\", \"to\": \"" + room_num.ToString() + "\",\"from\":\"" + did.ToString() + "\"}" + "\n";
      this.Send_Message();
    }

    public void Create_Signin_json(string username, string password) {
      msg = "{\"type\": \"sys\",  \"detail\": \"sign in\", \"driver\": { \"username\": \"" + username + "\", \"password\": \"" + password + "\"} }" + "\n";
      this.Send_Message();
    }

    public void Create_Signup_json(string username, string password, string name, string date) {
      msg = "{ \"type\": \"sys\", \"detail\": \"sign up\", \"driver\": { \"username\": \"" + username + "\", \"password\": \"" + password + "\", \"name\": \"" + name + "\", \"created_at\": \"" + date + "\" } }" + "\n";
      this.Send_Message();
    }

    public void Ask_For_Roomlist() {
      msg = "{\"type\":\"sys\", \"detail\":\"room list\"}" + "\n";
      this.Send_Message();
    }

    public void Ask_For_Driverlist() {
      msg = "{\"type\":\"sys\", \"detail\":\"driver list\", \"rid\":\"" + cur_rid.ToString() + "\"}" + "\n";
      this.Send_Message();
    }

    public void Ask_For_RoomImage() {
      foreach (var i in Room_list) {
        msg = "{\"type\":\"file\",\"updown\":\"down\",\"format\":\"image\",\"detail\": \"room avatar\",\"room\":{\"rid\":\"" + i.ToString() + "\"}}" + "\n";
        this.Send_Message();
      }
    }

    public void Ask_For_DriverImage() {
      foreach (var i in Driver_list) {
        msg = "{\"type\":\"file\",\"updown\":\"down\",\"format\":\"image\",\"detail\": \"driver avatar\",\"driver\":{\"did\":\"" + i.ToString() + "\"}}" + "\n";
        this.Send_Message();
      }
    }

    public void Ask_For_UserImage() {
      msg = "{\"type\":\"file\",\"updown\":\"down\",\"format\":\"image\",\"detail\": \"driver avatar\",\"driver\":{\"did\":\"" + did.ToString() + "\"}}" + "\n";
      this.Send_Message();
    }

    public void Enter_Room_json(int rid) {
      cur_rid = rid;
      msg = "{\"type\":\"sys\",\"detail\":\"enter room\",\"rid\":\"" + cur_rid.ToString() + "\"}" + "\r\n";
      this.Send_Message();
    }

    public void Quit_Room_json() {
      msg = "{\"type\":\"sys\",\"detail\":\"quit room\",\"rid\":\"" + cur_rid.ToString() + "\"}" + "\r\n";
      this.Send_Message();
      cur_rid = -1;
    }

    public void Create_Chat_Image_json(int len, byte[] Imgbytes, int rid) {
      //msg = "{\"type\":\"file\",\"format\":\"image\",\"length\":" + len.ToString() + ",\"to\":" + room_num.ToString() + "}" + "\r\n";
      /*
      {
          "type": "file",
          "updown": "up",
          "format": "image",
          "length": len.ToString(),
          "driver" : {
              "did": did.ToString()
          },
          "room": {
              "rid": rid.ToString()
          },
          "from": did.ToString(),
          "to": cur_rid.ToString
      }
      */
      msg = "{\"type\":\"file\",\"updown\":\"up\",\"format\":\"image\",\"detail\": \"chat\",\"length\":" + len.ToString() + ",\"driver\":{\"did\":\"" + did.ToString() + "\"},\"room\":{\"rid\":\"" + rid.ToString() + "\"},\"from\":\"" + did.ToString() + "\",\"to\":\"" + cur_rid.ToString() + "\"}";
      msg += "\n";
      this.Send_Msg_Img(len, Imgbytes);
    }

    Stream streamOut;
    StreamWriter writer;
    private Object _lock = new Object();
    public void Send_Message() {
      lock (_lock) {
        if (clientsocket == null)
          return;
        streamOut = clientsocket.OutputStream.AsStreamForWrite();
        writer = new StreamWriter(streamOut);
        byte[] json_bytes = System.Text.Encoding.UTF8.GetBytes(msg);
        streamOut.Write(json_bytes, 0, json_bytes.Length);
        streamOut.Flush();
      }
      /*await writer.WriteAsync(msg);
      await writer.FlushAsync();*/
    }

    async public void Send_Msg_Img(int len, byte[] Imgbytes) {
      streamOut = clientsocket.OutputStream.AsStreamForWrite();
      byte[] json_bytes = System.Text.Encoding.UTF8.GetBytes(msg);
      byte[] enter_n = System.Text.Encoding.UTF8.GetBytes("\n");
      byte[] combine = new byte[json_bytes.Length + len + 1];
      int json_count = json_bytes.Length;
      for (int i = 0; i < json_count; i++)
        combine[i] = json_bytes[i];
      for (int i = 0; i < len; i++)
        combine[i + json_count] = Imgbytes[i];
      combine[combine.Length - 1] = enter_n[0];

      await streamOut.WriteAsync(combine, 0, combine.Length);
      await streamOut.FlushAsync();
    }


    /*
        Create_Chat_json(string words, int room_num);
        Create_Signin_json(string username, string password);
        Create_Signup_json(string username, string password, string name, string date);
    */
  }
}
