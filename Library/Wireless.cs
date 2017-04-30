using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.WiFi;
using Windows.Networking.Connectivity;
using Windows.Security.Credentials;

namespace Library
{
    public class Wireless
    {
        // Private Part
        private WiFiAdapter _wifi;
        private WiFiAvailableNetwork _nets;
        private WiFiConnectionResult _conn;

        /// <summary>
        /// Wireless Network Connected Status
        /// </summary>
        public bool Connected
        {
            get { return _connected; }
        }
        private bool _connected;

        /// <summary>
        /// Wireless Network Constructor
        /// </summary>
        public Wireless()
        { }

        // Init
        public async Task<List<string>> Init()
        {
            return await Access();
        }

        // Access
        private async Task<List<string>> Access()
        {
            List<string> devices = new List<string>();

            // Request wifi access from the device
            var access = await WiFiAdapter.RequestAccessAsync();

            // Check if access status is allowed
            if (access == WiFiAccessStatus.Allowed)
            {
                // Find all wifi adapters on the equipment
                var adapters = await DeviceInformation.FindAllAsync(WiFiAdapter.GetDeviceSelector());

                if (adapters.Count > 0)
                    foreach (var item in adapters)
                        devices.Add(item.Id);
            }
            return devices;
        }


        /// <summary>
        /// Connect to Wireless Network
        /// </summary>
        /// <param name="_network">Wireless Network Name (SSID)</param>
        /// <param name="_password">Password Creditentials</param>
        /// <returns></returns>
        public async Task Connect(string _network, string _password, string id)
        {
            try
            {
                // Get WiFi Adapter from ID
                this._wifi = await WiFiAdapter.FromIdAsync(id);

                // Scan for Wireless Networks
                await this._wifi.ScanAsync();

                // Select Wireless Network
                List<WiFiAvailableNetwork> _list = new List<WiFiAvailableNetwork>();
                _list.AddRange(_wifi.NetworkReport.AvailableNetworks.ToList());
                _nets = _list.FirstOrDefault(x => x.Ssid.Equals(_network));

                // Enter Password Creditentials
                var credential = new PasswordCredential() { Password = _password };

                // Wireless Connection Result 
                _conn = await _wifi.ConnectAsync(_nets, WiFiReconnectionKind.Automatic, credential);

                // Connected Status
                if (_conn.ConnectionStatus != WiFiConnectionStatus.Success) _connected = false;
                else _connected = true;
            }
            catch
            {
                _connected = false;
            }
        }

        /// <summary>
        /// Internet Protocol Address
        /// </summary>
        /// <returns></returns>
        public string Host()
        {
            string host = null;
            var _networkAdapterId = this._wifi.NetworkAdapter.NetworkAdapterId;
            var _connectedProfile = NetworkInformation.GetConnectionProfiles().ToList();
            foreach (var item in _connectedProfile)
            {
                if (_networkAdapterId == item.NetworkAdapter.NetworkAdapterId)
                {
                    var hostname = NetworkInformation.GetHostNames().SingleOrDefault
                    (
                        hn => hn.IPInformation != null &&
                        hn.IPInformation.NetworkAdapter != null &&
                        hn.IPInformation.NetworkAdapter.NetworkAdapterId == _networkAdapterId
                    );
                    if (hostname != null) host = hostname.CanonicalName; // Internet Protocol Address
                }
            }
            return host;
        }

    }
}
