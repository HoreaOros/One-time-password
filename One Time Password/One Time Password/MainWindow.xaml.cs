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

namespace One_Time_Password
{
    public partial class MainWindow : Window
    {
		TcpListener server;
		char[] separators = { '|' };
		int verificationVariable;
        int port = 5000;

		public MainWindow()
        {
            InitializeComponent();
        }

		private void StartButton_Click(object sender, RoutedEventArgs e)
		{
			server = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
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
							verificationVariable = Convert.ToInt32(dataSplit[1]);
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
	}
}
