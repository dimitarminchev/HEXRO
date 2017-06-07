using Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.WiFi;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409
namespace Hexapod
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // Private Part
        private int _port = 1234;
        private NewTcpListener listener;

        // Constructor
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        // Loaded
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            listener = new NewTcpListener("0.0.0.0", _port, new Action<string>(WriteToLog));
            LOG.Text += "Socket: " + "0.0.0.0" + ":" + _port + SocketSettings.NewLine;            
        }
        
        // Log
        private async void WriteToLog(string data)
        {
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => LOG.Text += data);
        }

    }
}
