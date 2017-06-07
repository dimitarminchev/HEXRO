using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.WiFi;
using Windows.Foundation;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace Library
{
    // Server Soket Class
    public class SocketServer
    {
        // Private Part
        private readonly string _host;
        private readonly int _port;
        private StreamSocketListener _listener;
        private DataWriter _writer;
        private DataReader _reader;

        // Public Part       
        public string Host { get { return _host; } }
        public int Port { get { return _port; } }
        public delegate void DataRecived(string data);
        public event DataRecived OnDataRecived;
        public delegate void Error(string message);
        public event Error OnError;

        /// <summary>
        /// Socket Server Constructor
        /// </summary>
        /// <param name="port">Port Number</param>
        public SocketServer(string host, int port)
        {
            _host = host;
            _port = port;
        }

        /// <summary>
        /// Start Socket Server
        /// </summary>
        public async Task Start()
        {
            try
            {
                if (_listener != null)
                {
                    await _listener.CancelIOAsync();
                    _listener.Dispose();
                    _listener = null;
                }
                _listener = new StreamSocketListener();

                _listener.ConnectionReceived += Listener_ConnectionReceived;
                //await _listener.BindServiceNameAsync(this._port.ToString());
                await _listener.BindEndpointAsync(new HostName(this._host), this._port.ToString());
            }
            catch (Exception e)
            {
                // Error Handler
                if (OnError != null) OnError(e.Message);
            }
        }


        /// <summary>
        /// Socket Server Listener
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void Listener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            _writer = new DataWriter(args.Socket.OutputStream);
            _reader = new DataReader(args.Socket.InputStream);
            _reader.InputStreamOptions = InputStreamOptions.Partial;
            try
            {
                while (true)
                {
                    // Read One Character
                    uint len = await _reader.LoadAsync(1);
                    if (OnDataRecived != null)
                    {
                        string data = _reader.ReadString(len);
                        OnDataRecived(data);
                    }
                }
            }
            catch (Exception ex)
            {
                // Error Handler
                if (OnError != null) OnError(ex.Message);
            }
        }

        /// <summary>
        /// Socket Server Send Message to Client
        /// </summary>
        /// <param name="message">Text Message to Send</param>
        public async void Send(string message)
        {
            if (_writer != null)
            {
                try
                {
                    _writer.WriteUInt32(_writer.MeasureString(message));
                    _writer.WriteString(message);
                    await _writer.StoreAsync();
                    await _writer.FlushAsync();
                }
                catch (Exception ex)
                {
                    // Error Handler
                    if (OnError != null) OnError(ex.Message);
                }
            }
        }
    }
}
