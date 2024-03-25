using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net.Mail;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace AmperChatServer
{
    struct MESSAGE 
    {
        public string mess;
        public bool check;
        public byte[] buffer;
        public int recB;
    }
    class ClientCon
    {
        public TcpClient clientCon;
        public NetworkStream Stream = null;
        public readonly string nickname;
        public MESSAGE message;

        public ClientCon(TcpClient clientCon)
        {
            this.clientCon = clientCon;
            Stream = clientCon.GetStream();
            message = new MESSAGE();
            message.check = true;
            message.mess = string.Empty;
            message.buffer = new byte[1024];

            message.recB = Get(ref message.buffer);
            nickname = Encoding.UTF8.GetString(message.buffer, 0, message.recB);

            Thread thread = new Thread(() => { 
                checkRead();
            });
            thread.Start();
        }

        ~ClientCon() 
        {
            clientCon.Dispose();
            Stream.Dispose();
            clientCon.Close();
            Stream.Close();
        }
        
        public void Send(byte[] buffer) 
        {
            Stream.Write(buffer, 0, buffer.Length);
        }

        public int Get(ref byte[] buffer) 
        {
            return Stream.Read(buffer, 0, buffer.Length);
        }

        private void checkRead()
        {
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[1024];
                    message.recB = Get(ref buffer);
                    if (message.recB > 0) 
                    {
                        message.buffer = buffer;
                        message.mess = Encoding.UTF8.GetString(buffer, 0, message.recB);
                        if (!String.IsNullOrEmpty(message.mess)) { message.check = false; }
                    }
                } catch(Exception ex) { break; }
            }
        }
    }
}
