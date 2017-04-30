using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace Library
{
    // Client Soket Class
    public class SocketClient
    {
        // Private Part
        private readonly string _host;
        private readonly int _port;
        private StreamSocket _socket;
        private DataWriter _writer;
        private DataReader _reader;

        // Public Part
        public string Host { get { return _host; } }
        public int Port { get { return _port; } }
        public delegate void Error(string message);
        public event Error OnError;
        public delegate void DataRecived(string data);
        public event DataRecived OnDataRecived;

        /// <summary>
        /// Socket Client Constructor
        /// </summary>
        /// <param name="host">Internet Protocol Host Address</param>
        /// <param name="port">Port Number</param>
        public SocketClient(string host, int port)
        {
            _host = host;
            _port = port;
        }

        /// <summary>
        /// Socket Client Connect
        /// </summary>
        public async void Connect()
        {
            try
            {
                var host = new HostName(this._host);
                _socket = new StreamSocket();
                await _socket.ConnectAsync(host, this._port.ToString());
                _writer = new DataWriter(_socket.OutputStream);
                Read();
            }
            catch (Exception ex)
            {
                // Error Handler
                if (OnError != null) OnError(ex.Message);
            }
        }

        /// <summary>
        /// Socket Client Send Byte Array To Server
        /// </summary>
        /// <param name="message">>Byte Array to Send</param>
        public async void SendBytes(byte[] message)
        {
            try
            {
                _writer.WriteBytes(message);
                await _writer.StoreAsync();
                await _writer.FlushAsync();
            }
            catch (Exception ex)
            {
                // Error Handler
                if (OnError != null) OnError(ex.Message);
            }
        }

        /// <summary>
        /// Socket Client Send Message To Server
        /// </summary>
        /// <param name="message">>Text Message to Send</param>
        public async void Send(string message)
        {            
            try
            {
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

        /// <summary>
        /// Socket Client Read Message From Client
        /// </summary>
        private async void Read()
        {            
            try
            {
                _reader = new DataReader(_socket.InputStream);
                while (true)
                {
                    uint _size = await _reader.LoadAsync(sizeof(byte));
                    uint _count = _reader.UnconsumedBufferLength;
                    if (OnDataRecived != null) OnDataRecived(_reader.ReadString(_count));
                }
            }
            catch (Exception ex)
            {
                // Error Handler
                if (OnError != null) OnError(ex.Message);
            }
        }

        /// <summary>
        /// Socket Client Close
        /// </summary>
        public void Close()
        {
            _writer.DetachStream();
            _writer.Dispose();
            _reader.DetachStream();
            _reader.Dispose();
            _socket.Dispose();
        }
    }
}
