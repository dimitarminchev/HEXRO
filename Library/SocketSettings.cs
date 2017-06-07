using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    // Socket Settings Class
    public class SocketSettings
    {
        /// <summary>
        /// Environment NewLine Replacement
        /// </summary>
        public static string NewLine
        {
            get { return _newline; }
        }
        private static string _newline = "\n";

        /// <summary>
        /// Socket Settings Internet Protocol Host Address
        /// </summary>
        public string Host 
        {
            get { return _host; }
        }
        private string _host;

        /// <summary>
        /// Socket Settings Port Number
        /// </summary>
        public int Port
        {
            get { return _port; }
        }
        private int _port;

        /// <summary>
        /// Socket Settings Constructor
        /// </summary>
        /// <param name="host">Internet Protocol Host Address</param>
        /// <param name="port">Port Number</param>
        public SocketSettings(string host, int port)
        {
            this._host = host;
            this._port = port;            
        }
    }
}
