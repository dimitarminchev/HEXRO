using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Control
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ControlPage : Page
    {
        // Socket        
        private Library.SocketSettings _settings;
        private Library.SocketClient _socket;

        // Constructor
        public ControlPage()
        {
            this.InitializeComponent();
        }

        // Landing
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _settings = e.Parameter as Library.SocketSettings;
            if (_socket != null)
            {
                _socket.Close();
                _socket = null;
            }
            _socket = new Library.SocketClient(_settings.Host, _settings.Port);
            _socket.Connect();
        }

        // Stop Button
        private void Stop_Button_Click(object sender, RoutedEventArgs e)
        {

            string cmd = "stop" + Library.SocketSettings.NewLine;
            byte[] bytes = Encoding.UTF8.GetBytes(cmd);
            _socket.SendBytes(BitConverter.GetBytes(bytes.Length));
            _socket.SendBytes(bytes);
        }

        // Walk Button
        private void Walk_Button_Click(object sender, RoutedEventArgs e)
        {

            string cmd = "walk" + Library.SocketSettings.NewLine;
            byte[] bytes = Encoding.UTF8.GetBytes(cmd);
            _socket.SendBytes(BitConverter.GetBytes(bytes.Length));
            _socket.SendBytes(bytes);
        }

        // Rotate Button
        private void Rotate_Button_Click(object sender, RoutedEventArgs e)
        {

            string cmd = "rotate" + Library.SocketSettings.NewLine;
            byte[] bytes = Encoding.UTF8.GetBytes(cmd);
            _socket.SendBytes(BitConverter.GetBytes(bytes.Length));
            _socket.SendBytes(bytes);
        }

        // Back
        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
