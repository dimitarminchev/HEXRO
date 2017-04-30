using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest
{
    class Program
    {
        // Private part        
        private static string _host = "192.168.5.104";
        private static int _port = 1234;

        // Main
        static void Main(string[] args)
        {
            Connect();
            Console.Read();
        }

        // Connect
        private async static void Connect()
        {
            // Connect
            TcpClient client = new TcpClient(AddressFamily.InterNetwork);
            begin:
            try
            {
                await client.ConnectAsync(_host, _port);
            }
            catch (Exception ex)
            {
                goto begin;
            }
            // Process
            if (client.Connected)
            {
                while (true)
                {
                    string command = Console.ReadLine();
                    byte[] buffer = Encoding.UTF8.GetBytes(command);
                    client.Client.Send(BitConverter.GetBytes(buffer.Length));
                    client.Client.Send(buffer);
                }
            }
        }
    }
}
