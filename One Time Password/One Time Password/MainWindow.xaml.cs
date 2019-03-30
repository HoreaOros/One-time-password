using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SimpleTCP;

namespace One_Time_Password
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Eratosthenes e = new Eratosthenes();
            server = new SimpleTcpServer
            {
                Delimiter = 0x0a,
                StringEncoder = Encoding.UTF8
            };
            server.DataReceived += Server_DataReceived;
        }

        SimpleTcpServer server;
        public List<string> clients = new List<string>();
        bool inc = false;
        char[] split = { '«' };
        int i = 0;
        string host = "127.0.0.1";
        int port = 80;

        private void Server_DataReceived(object sender, SimpleTCP.Message e)
        {
            if (new string(e.MessageString.ToCharArray(), 0, e.MessageString.Length - 1) == "☺GetList☺")
            {
                string a = "";
                for (int k = 0; k < clients.Count; k++)
                {
                    a += clients[k] + "«";
                }
                e.Reply(a);
            }
            else
            {
                if (inc == false)
                {
                    string[] message = e.MessageString.Split(split, StringSplitOptions.RemoveEmptyEntries);
                    string a = "";
                    for (int i = 1; i < message.Length; i++)
                    {
                        a += message[i];
                    }
                    a = new string(a.ToCharArray(), 0, a.Length - 1);
                    string msg = "<" + clients[Convert.ToInt32(message[0])] + ">: " + a;
                    OutputListBox.Items.Add(Environment.NewLine + msg);
                    server.BroadcastLine(Environment.NewLine + msg);
                }
                else
                {
                    e.Reply((i - 1) + "«");

                    clients.Add(new string(e.MessageString.ToCharArray(), 0, e.MessageString.Length - 1));
                    inc = false;
                    string a = new string(e.MessageString.ToCharArray(), 0, e.MessageString.Length - 1);
                    OutputListBox.Items.Add(Environment.NewLine + a + " joined the chat.");
                    server.BroadcastLine(Environment.NewLine + a + " joined the chat.");
                }
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (server.IsStarted)
            {
                server.Stop();
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            OutputListBox.Items.Add("Server starting... ");
            System.Net.IPAddress ip = System.Net.IPAddress.Parse(host);
            server.Start(ip,port);
            OutputListBox.Items.Add(Environment.NewLine + "Server started.");
        }
    }
}
