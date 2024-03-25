using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AmperChatClient
{
    class Program
    {
        static void Main(string[] args)
        {
            string nickname = null;
            while(String.IsNullOrWhiteSpace(nickname)) 
            {
                Console.Write("Enter your nickname: ");
                nickname = Console.ReadLine();
            }

            Console.Write("Enter to connect:");
            Console.ReadLine();

            TcpClient socket = new TcpClient();
            Console.WriteLine("Start connection!");
            socket.Connect("00.00.00.00", 1111);
            NetworkStream stream = socket.GetStream();
            if (socket.Connected) 
            {
                try
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(nickname);
                    stream.Write(buffer, 0, buffer.Length);
                } catch (Exception ex) { }
                Console.WriteLine("Connected!");
            }
            Thread thread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        byte[] buffer = new byte[1024];
                        int recB = stream.Read(buffer, 0, buffer.Length);
                        string name = Encoding.UTF8.GetString(buffer, 0, recB);

                        recB = stream.Read(buffer, 0, buffer.Length);
                        string message = Encoding.UTF8.GetString(buffer, 0, recB);
                        if (recB != 0 && message != null)
                        {
                            Console.WriteLine($"[{name}]:{message}");
                        }
                    }catch (Exception ex) 
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            });
            thread.Start();
            
            while(true) 
            {
                string str = Console.ReadLine();
                if(!String.IsNullOrEmpty(str))
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(str);
                    stream.Write(buffer, 0, buffer.Length);
                }
            }
        }
    }
}