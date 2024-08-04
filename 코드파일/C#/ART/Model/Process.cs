using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using ART.View;
using OpenCvSharp;

namespace ART.Model
{

    public class Server
    {
        public Server() { }

        private TcpClient Clnt;
        private NetworkStream Stream;

        private string IP = "10.10.21.114";
        private int Port = 5001;

        private int Connect()
        {
            try
            {
                Clnt = new TcpClient(IP, Port);
                Stream = Clnt.GetStream();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return 0;
            }

            return 1;
        }

        private void Disconnect()
        {
            Stream.Close();
            Clnt.Close();
        }

        private void SendMsg(Data MsgData)
        {
            string Sendmsg = JsonSerializer.Serialize(MsgData);
            byte[] SendByte = Encoding.UTF8.GetBytes(Sendmsg);
            Stream.Write(SendByte, 0, SendByte.Length);
        }

        private string ReadMsg()
        {
            byte[] Buffer = new byte[4096];
            int ReadByte = Stream.Read(Buffer, 0, Buffer.Length);
            string ReadJson = Encoding.UTF8.GetString(Buffer, 0, ReadByte);

            return ReadJson;
        }

        public int User_Info()
        {
            if (Connect() == 0)
            {
                return 0;
            }

            Data SendData = new Data()
            {
                Key = Main.data.Key,
                ID = Main.data.ID,  
                user = Main.data.user,
            };

            SendMsg(SendData);
            string ReadStr = ReadMsg();

            Disconnect();

            try
            {
                Data? ResultData = JsonSerializer.Deserialize<Data?>(ReadStr);

                return ResultData.Key;
            }
            catch (JsonException jsonEx)
            {
                Debug.WriteLine(jsonEx.Message);
                return 0;
            }
        }

        public int ID_Check()
        {
            if (Connect() == 0)
            {
                return 0;
            }

            Data SendData = new Data()
            {
                Key = Main.data.Key,
                ID = Main.data.ID,
            };

            SendMsg(SendData);
            string ReadStr = ReadMsg();

            Disconnect();

            try
            {
                Data? ResultData = JsonSerializer.Deserialize<Data?>(ReadStr);

                return ResultData.Key;
            }
            catch (JsonException jsonEx)
            {
                Debug.WriteLine(jsonEx.Message);
                return 0;
            }
        }

        public int ArtInfo()
        {
            if (Connect() == 0)
            {
                return 0;
            }

            Data SendData = new Data()
            {
                Key = (int)KEY.RESULT,
                ID = Main.data.ID,
                Value = Main.data.Value,
            };

            SendMsg(SendData);
            string ReadStr = ReadMsg();

            Disconnect();

            try
            {
                Data? ResultData = JsonSerializer.Deserialize<Data?>(ReadStr);

                Main.data.ArtistList = ResultData.ArtistList;

                return ResultData.Key;
            }
            catch (JsonException jsonEx)
            {
                Debug.WriteLine(jsonEx.Message);
                return 0;
            }
        }

        public int ReadArtistInfo()
        {
            if (Connect() == 0)
            {
                return 0;
            }

            Data SendData = new Data()
            {
                Key = Main.data.Key,
            };

            SendMsg(SendData);
            string ReadStr = ReadMsg();

            Disconnect();

            try
            {
                Data? ResultData = JsonSerializer.Deserialize<Data?>(ReadStr);

                if(ResultData.Key == (int)KEY.OK)
                {
                     Main.data.ArtistList = ResultData.ArtistList;
                }

                return ResultData.Key;
            }
            catch (JsonException jsonEx)
            {
                Debug.WriteLine(jsonEx.Message);
                return 0;
            }
        }

    }

    public class PyServer
    {
        public PyServer() { }

        private TcpClient PyClnt;
        private NetworkStream PyStream;

        private string PyIP = "192.168.0.142";
        private int PyPort = 5000;

        private int PyConnect()
        {
            try
            {
                PyClnt = new TcpClient(PyIP, PyPort);
                PyStream = PyClnt.GetStream();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return 0;
            }

            return 1;
        }

        private void PyDisconnect()
        {
            PyStream.Close();
            PyClnt.Close();
        }

        public int Send_Image(BitmapSource image)
        {
            if(PyConnect() == 0)
            {
                return 0;
            }

            JpegBitmapEncoder encoder = new JpegBitmapEncoder();

            encoder.Frames.Add(BitmapFrame.Create(image));

            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                byte[] ImBuffer = ms.ToArray();
                byte[] temp = BitConverter.GetBytes(ImBuffer.Length);
                Array.Reverse(temp);
                PyStream.Write(temp, 0, temp.Length);

                Thread.Sleep(100);
                PyStream.Write(ImBuffer, 0, ImBuffer.Length);
            }


            byte[] Buffer = new byte[4096];
            int ReadByte = PyStream.Read(Buffer, 0, Buffer.Length);
            string ReadJson = Encoding.UTF8.GetString(Buffer, 0, ReadByte);

            PyDisconnect();

            try
            {
                Data? Result = JsonSerializer.Deserialize<Data>(ReadJson);
                Main.data.Value = Result.Value;

                return 20;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return 0;
            }
        }

        public  BitmapImage ConvertBitmapSourceToBitmapImage(BitmapSource bitmapSource)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                encoder.Save(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }
    }

    public class Email
    {
        public Email() { }

        public void Send_Email()
        {

            SmtpClient client = new SmtpClient("smtp.naver.com", 465);
            //client.EnableSsl = true;
            client.Credentials = new NetworkCredential(Ask.MailInfo.To, "9duqls23");

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(Ask.MailInfo.From);
            mailMessage.To.Add(Ask.MailInfo.To);
            mailMessage.Body = Ask.MailInfo.Content;
            mailMessage.Subject = Ask.MailInfo.Titile;

            try
            {
                client.Send(mailMessage);
                Console.WriteLine("메일 전송 성공");
            }
            catch (SmtpException ex)
            {
                Console.WriteLine($"메일 전송 실패: {ex.Message}");
            }

        }
    }
}
