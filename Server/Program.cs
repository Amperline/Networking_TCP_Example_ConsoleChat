using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace AmperChatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            List<ClientCon> cons = new List<ClientCon>();
            Thread th = new Thread(() => 
            {
                TcpListener listener = new TcpListener(IPAddress.Any, 1111);
                listener.Start();
                Console.WriteLine("Listner started!");

                while (true)
                {
                    TcpClient lcon = listener.AcceptTcpClient();
                    cons.Add(new ClientCon(lcon));
                    Console.WriteLine("Client connected!");
                    Thread.Sleep(10);
                }
            });
            th.Start();
            while (true)
            {
                foreach (var client in cons.ToList())
                {
                    if (client != null && client.clientCon.Connected)
                    {
                        try
                        {
                            if(client.message.check == false && client.message.mess != null)
                            {
                                Console.WriteLine($"Message [{client.nickname}]:" + client.message.mess);
                                foreach (var con in cons.ToList())
                                {
                                    if (con.clientCon.Connected && con != client)
                                    {
                                        Thread thread = new Thread(() =>
                                        {
                                            con.Send(Encoding.UTF8.GetBytes(client.nickname));
                                            con.Send(client.message.buffer);
                                        });
                                        thread.Start();
                                    }
                                }
                                client.message.mess = null;
                                client.message.check = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    else if (client != null && !client.clientCon.Connected)
                    {
                        Console.WriteLine("Client disconnected!");
                        cons.Remove(client);
                    }
                }
            }
        }
    }
}