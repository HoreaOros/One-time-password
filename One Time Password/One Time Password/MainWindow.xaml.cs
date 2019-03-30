using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Net;
using System.Net.Sockets;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
//using SimpleTCP;

namespace One_Time_Password
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		TcpListener server;
		char[] separators = { '|' };
		int verificationVariable;

		public MainWindow()
        {
            InitializeComponent();
           /* Eratosthenes e = new Eratosthenes();
            server = new SimpleTcpServer
            {
                Delimiter = 0x0a,
                StringEncoder = Encoding.UTF8
            };
            server.DataReceived += Server_DataReceived;*/
        }

		private void StartButton_Click(object sender, RoutedEventArgs e)
		{
			server = new TcpListener(IPAddress.Parse("127.0.0.1"), 80);
			server.Start();
			OutputListBox.Items.Add("Server started");
			Task.Run(()=>Listening());
			StartButton.IsEnabled = false;
		}

		private void Listening()
		{
			byte[] bytes = new byte[256];
			string data;
			while (true)
			{
				OutputListBox.Dispatcher.BeginInvoke(new Action(delegate () {
					OutputListBox.Items.Add("Waiting for a connection... ");
				}));
				TcpClient client = server.AcceptTcpClient();
				OutputListBox.Dispatcher.BeginInvoke(new Action(delegate () {
					OutputListBox.Items.Add("Connected ");
				}));
				data = null;
				NetworkStream stream = client.GetStream();
				int i;
				while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
				{
					data = Encoding.ASCII.GetString(bytes, 0, i);
					OutputListBox.Dispatcher.BeginInvoke(new Action(delegate () {
						OutputListBox.Items.Add("Received: " + data);
					}));
					string[] dataSplit;
					if ((dataSplit = data.Split(separators, StringSplitOptions.RemoveEmptyEntries)).Length == 1)
					{
						verificationVariable = Convert.ToInt32(data);
						byte[] aux = Encoding.ASCII.GetBytes(1 + "");
						stream.Write(aux, 0, aux.Length);
					}
					else
					{
						int aux1 = H(Convert.ToInt32(dataSplit[1]), 1);
						string output = "";
						if (aux1 == verificationVariable)
						{
							verificationVariable = aux1;
							output += (Convert.ToInt32(dataSplit[0])+1)+"";
						}
						else
						{
							output += dataSplit[0];
						}
						byte[] byteoutput = Encoding.ASCII.GetBytes(output);
						stream.Write(byteoutput, 0, byteoutput.Length);
					}
				}
			}
		}

		private void StopButton_Click(object sender, RoutedEventArgs e)
		{

		}

		private int H(int input, int times)
		{
			int N = 10007 * 25471;
			long aux = input;
			for (int i = 0; i < times; i++)
			{
				aux = (aux * aux) % N;
			}
			return (int)aux;
		}



		/* SimpleTcpServer server;
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
		 }*/
	}
}
