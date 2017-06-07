using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class NewTcpListener
    {
        private TcpListener _listener;
        private Task task;
        private Action<string> _logger;

        public NewTcpListener(string ip, int port, Action<string> logger)
        {
            this._logger = logger;
            try
            {
                this._listener = new TcpListener(IPAddress.Parse(ip), port);
                this._listener.Start(100);
            }
            catch (Exception ex)
            {
                this._logger?.Invoke(ex.Message);
            }
            
            this.task = Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    var socket = await this._listener.AcceptSocketAsync();
                    this._logger?.Invoke(socket.Connected.ToString() + Environment.NewLine);
                    WaitForData(socket);
                }
            });

        }

        public EndPoint GetEndpoint()
        {
            return _listener.LocalEndpoint;
        }

        private void WaitForData(Socket socket)
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    try
                    {
                        byte[] dataSize = new byte[4];
                        socket.Receive(dataSize);
                        int size = BitConverter.ToInt32(dataSize, 0);
                        byte[] buffer = new byte[size];
                        socket.Receive(buffer);
                        string command = new string(Encoding.UTF8.GetChars(buffer));
                        this._logger?.Invoke(command);
                        command = command.Trim('\n', '\r');

                        Commander.Instance.EnqueueCommand(command);
                    }
                    catch (Exception ex)
                    {
                        this._logger?.Invoke(ex.Message);
                        return socket;
                    }
                }

            });
        }


    }
}
