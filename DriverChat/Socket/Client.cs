﻿using System;
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
using Windows.UI.Xaml.Media;
using Windows.Graphics.Imaging;

namespace DriverChat.Socket {
  public class Client {
    const int maxn = 1024;
    public string username, name, badge, created_at;
    bool status;
    bool flag_ = false;
    public Windows.Networking.Sockets.StreamSocket clientsocket;
    public Windows.Networking.HostName serverHost;
    public string serverPort;
    public Stream streamIn;
    StreamReader reader;
    bool working;
    string msg;

    public int cur_rid, did, got_room_driver_list = 0;

    public delegate void GotMessageHandler(int chat_form, string chat_msg);
    public event GotMessageHandler GotMessage;

    public delegate void GotErrorHandler(string msg);
    public event GotErrorHandler GotSigninError;
    public event GotErrorHandler GotSigninSucceed;
    public event GotErrorHandler GotSignupError;
    public event GotErrorHandler GotSignupSucceed;
    public event GotErrorHandler GotSysError;

    public delegate void GotChatImageHandler(int from, ImageSource Image);
    public event GotChatImageHandler GotChatImage;

    public delegate void GotAvatarHandler(int id, ImageSource Image);
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
    public event GotListHandler Update_User_Avatar_Succeed;


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
      if (flag_)
        return;
      else
        flag_ = true;
      System.Diagnostics.Debug.WriteLine("In Listener");
      byte[] last = new byte[maxn];
      int s, res_len = 0;
      int count, c_index = 0, index = 0;
      byte[] c = new byte[maxn * 10];
      bool isFile = false;
      byte[] first = new byte[1];
      byte[] Imgbytes = new byte[1];
      string str_msg = "";
      JObject list = (JObject)JsonConvert.DeserializeObject("");
      streamIn = clientsocket.InputStream.AsStreamForRead();
      reader = new StreamReader(streamIn);
      while (working) {
        bool flag = false;
        count = await streamIn.ReadAsync(first, 0, first.Length);
        s = -1;

        while (s < count - 1) {
          s++;
          if (isFile) {
            res_len = res_len - 1;
            if (res_len != 0) {
              Imgbytes[index++] = first[s];
              continue;
            }
          } else {
            c[c_index++] = first[s];
            str_msg = System.Text.Encoding.UTF8.GetString(c);
            if (str_msg.IndexOf('\n') == -1)
              continue;
          }

          ///handler event
          if (!isFile)
            list = (JObject)JsonConvert.DeserializeObject(str_msg);
          if (list["type"].ToString() == "sys") {                                         ///handle system response
            if (list["detail"].ToString() == "sign in") {                                   ///handle signin
              status = list["status"].ToString() == "True" ? true : false;
              msg = list["msg"].ToString();
              if (status) {
                did = Convert.ToInt32(list["driver"]["did"].ToString());
                name = list["driver"]["name"].ToString();
                badge = list["driver"]["badge"].ToString();
                created_at = list["driver"]["created_at"].ToString();
                GotSigninSucceed(msg);
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
                if (GotDriverList != null)
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
            if (!isFile) {
              if (list["updown"] != null && list["updown"].ToString() == "up") {
                Update_User_Avatar_Succeed();
                continue;
              }
              string format = list["format"].ToString();
              res_len = Convert.ToInt32(list["length"].ToString());
              Imgbytes = new byte[res_len];
              index = 0;
              isFile = true;
              res_len += 1;
            } else {
              WriteableBitmap image = null;
              BitmapDecoder decoder = null;
              using (MemoryStream imgstream = new MemoryStream(Imgbytes)) {
                // 不确定图片格式，因此用try/catch尝试
                try {
                  decoder = await BitmapDecoder.CreateAsync(BitmapDecoder.JpegDecoderId, imgstream.AsRandomAccessStream());
                } catch (Exception e) {
                  try {
                    decoder = await BitmapDecoder.CreateAsync(BitmapDecoder.PngDecoderId, imgstream.AsRandomAccessStream());
                  } catch (Exception e_) {
                    
                  }
                }
                // 仅在能获得正确decoder时继续处理
                if (decoder != null) {
                  var provider = await decoder.GetPixelDataAsync();
                  byte[] buffer = provider.DetachPixelData();
                  image = new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);
                  await image.PixelBuffer.AsStream().WriteAsync(buffer, 0, buffer.Length);
                }

                //await imgstream.WriteAsync(Imgbytes.AsBuffer());
                //imgstream.Seek(0);
                //await image.SetSourceAsync(imgstream);
              }
              // 仅在能处理图片时发生事件
              if (decoder != null) {
                if (list["detail"].ToString() == "driver avatar") {
                  GotDriverAvatar(Convert.ToInt32(list["driver"]["did"].ToString()), image);
                } else if (list["detail"].ToString() == "room avatar") {
                  GotRoomAvatar(Convert.ToInt32(list["room"]["rid"].ToString()), image);
                } else {
                  GotChatImage(Convert.ToInt32(list["from"].ToString()), image);
                }
                isFile = false;
              }
            }
          }

          c_index = 0;
          Array.Clear(c, 0, c.Length);
        }
      }
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
      msg = "{\"type\":\"file\",\"updown\":\"up\",\"format\":\"image\",\"detail\": \"chat\",\"length\":" + len.ToString() + ",\"driver\":{\"did\":\"" + did.ToString() + "\"},\"room\":{\"rid\":\"" + rid.ToString() + "\"},\"from\":\"" + did.ToString() + "\",\"to\":\"" + cur_rid.ToString() + "\"}";
      msg += "\n";
      this.Send_Msg_Img(len, Imgbytes);
    }


    public void Update_User_Avatar(int len, byte[] Imgbytes) {
      msg = "{\"type\":\"file\",\"updown\":\"up\",\"format\":\"image\",\"detail\": \"driver avatar\",\"length\":" + len.ToString() + ",\"driver\":{\"did\":" + did.ToString() + "}}";
      msg += "\n";
      this.Send_Msg_Img(len, Imgbytes);
    }

    Stream streamOut;
    StreamWriter writer;
    async public void Send_Message() {
      if (clientsocket == null)
        return;
      streamOut = clientsocket.OutputStream.AsStreamForWrite();
      writer = new StreamWriter(streamOut);
      byte[] json_bytes = System.Text.Encoding.UTF8.GetBytes(msg);
      await streamOut.WriteAsync(json_bytes, 0, json_bytes.Length);
      await streamOut.FlushAsync();
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
