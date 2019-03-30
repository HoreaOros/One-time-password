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

		public int secretW;
		public int ident;
		public int t = 1000;

		Random rnd = new Random();

		public MainWindow()
		{
			InitializeComponent();
			Load();
		}

		private void Load()
		{
			IPAddress IP = IPAddress.Parse("127.0.0.1");
			TcpClient client = new TcpClient();
			client.Connect(IP, 80);
			
			secretW = rnd.Next(10000, 100000000);
			byte[] data = Encoding.ASCII.GetBytes(secretW + "");

			NetworkStream stream = client.GetStream();
			stream.Write(data, 0, data.Length);

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
	}
}
