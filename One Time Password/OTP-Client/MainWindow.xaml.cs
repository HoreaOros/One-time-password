using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace OTP_Client
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{

		int secretW;
		public int ident=0;
		public int t = 1000;

		Random rnd = new Random();

		public MainWindow()
		{
			InitializeComponent();
		}

		private void SendValidation()
		{
			IPAddress IP = IPAddress.Parse("127.0.0.1");
			TcpClient client = new TcpClient();
			client.Connect(IP, 80);

			int wi = H(secretW, t - ident);
			byte[] data = Encoding.ASCII.GetBytes(ident+"|"+wi + "");

			NetworkStream stream = client.GetStream();

			stream.Write(data, 0, data.Length);
			data = new byte[256];
			int bytes = stream.Read(data, 0, data.Length);
			string responseData = Encoding.ASCII.GetString(data, 0, bytes);
			int aux = Convert.ToInt32(responseData);
			if (ident == aux)
			{
				validationLabel.Content = "Not Authenticated";
			}
			else
			{
				validationLabel.Content = "Authenticated";
			}

			stream.Close();
			client.Close();
		}

		private void Initialization()
		{
			IPAddress IP = IPAddress.Parse("127.0.0.1");
			TcpClient client = new TcpClient();
			client.Connect(IP, 80);
			
			secretW = rnd.Next(10000, 100000000);
			int w0 = H(secretW, t);
			byte[] data = Encoding.ASCII.GetBytes(w0 + "");

			NetworkStream stream = client.GetStream();

			stream.Write(data, 0, data.Length);
			int bytes = stream.Read(data, 0, data.Length);
			string responseData = Encoding.ASCII.GetString(data, 0, bytes);
			ident = Convert.ToInt32(responseData);

			stream.Close();
			client.Close();

		}
		private int H(int input, int times)
		{
			int N = 10007 * 25471;
			long aux = input;
			for (int i = 0; i < times; i++)
			{
				aux = (aux * aux) % N;
			}
			return (int) aux;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			if (ident == 0) Initialization();
			else SendValidation();
		}
	}
}
