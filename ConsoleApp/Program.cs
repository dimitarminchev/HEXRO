using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        // Private Part
        private static string _host;
        private static int _port;

        // Main
        static void Main(string[] args)
        {
            // Command Line Arguments
            if (args.Length < 2)
            {
                PrintLogo();
                System.Console.WriteLine("Syntax: HEXRO.exe [host] [port]");
                System.Console.WriteLine("Example Host: 192.168.1.103");
                System.Console.WriteLine("Example Port: 1234");
                return;
            }
            _host = args[0];
            _port = int.Parse(args[1]);

            // Connect
            Connect();
            Console.Read();
        }

        // Connect
        private static void Connect()
        {
            PrintLogo();
            Console.Write("Connecting ... ");
            TcpClient client = new TcpClient(AddressFamily.InterNetwork);
            begin:
            try
            {
                client.Client.Connect(_host, _port);
            }
            catch (Exception)
            {
                goto begin;
            }
            Console.WriteLine("DONE");
            Console.WriteLine("Socket: " + _host + ":" + _port);
            if (client.Connected)
            {
                Console.WriteLine("Waiting for Commands ...");
                Console.WriteLine("Syntax: [stop, walk, rotate, exit]");
                string command = null;
                do
                {
                    command = Console.ReadLine();
                    byte[] buffer = Encoding.UTF8.GetBytes(command);
                    client.Client.Send(BitConverter.GetBytes(buffer.Length));
                    client.Client.Send(buffer);
                }
                while (command != "exit");
            }
        }

        // Print
        static void PrintLogo()
        {
            System.Console.WriteLine("  _   _ _______  ______   ___  ");
            System.Console.WriteLine(" | | | | ____\\ \\/ /  _ \\ / _ \\");
            System.Console.WriteLine(" | |_| |  _|  \\  /| |_) | | | |");
            System.Console.WriteLine(" |  _  | |___ /  \\|  _ <| |_| |");
            System.Console.WriteLine(" |_| |_|_____/_/\\_\\_| \\_\\\\___/ ");
            System.Console.WriteLine(" H E X A P O D   R O B O T");
            System.Console.WriteLine(" ------------------------------");
        }

    }
}
