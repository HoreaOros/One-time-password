using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Net;
using System.Net.Sockets;
using System.IO;
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
	class User
	{
		public string Name { get; set; }
		public int verificationVariable { get; set; }
		public int ident { get; set; }
	}

    public partial class MainWindow : Window
    {
		TcpListener server;
		static char[] separators = { '|' };
        int port = 5000;
		static List<User> users = new List<User>();
		static string path = "../../Users.txt";
		public MainWindow()
        {
            InitializeComponent();
			GetUsers();
        }

		private static void GetUsers()
		{
			StreamReader stream = new StreamReader(path);
			string buffer = null;
			while ((buffer=stream.ReadLine())!=null)
			{
				string[] aux = buffer.Split(separators, StringSplitOptions.RemoveEmptyEntries);
				User user = new User()
				{
					Name = aux[0],
					verificationVariable = Convert.ToInt32(aux[1]),
					ident = Convert.ToInt32(aux[2])
				};
				users.Add(user);
			}
			stream.Close();
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
					if ((dataSplit = data.Split(separators, StringSplitOptions.RemoveEmptyEntries)).Length == 2)
					{
						User foundUser = null;
						if ((foundUser = users.Find(x => x.Name == dataSplit[0])) == null)
						{
							User user = new User()
							{
								Name = dataSplit[0],
								verificationVariable = Convert.ToInt32(dataSplit[1]),
								ident = 1
							};
							

							File.AppendAllText(path,user.Name + separators[0] + user.verificationVariable + separators[0] + user.ident+"\n");
							users.Add(user);
							foundUser = user;
						}
						byte[] aux = Encoding.ASCII.GetBytes(foundUser.ident + "");
						stream.Write(aux, 0, aux.Length);

					}
					else if(dataSplit.Length==3)
					{
						int aux1 = H(Convert.ToInt32(dataSplit[2]), 1);
						User foundUser = users.Find(x => x.Name == dataSplit[0]);
						if (aux1 == foundUser.verificationVariable)
						{
							foundUser.verificationVariable = Convert.ToInt32(dataSplit[2]);
							foundUser.ident++;
							RewriteUsersFile();
						}
						byte[] byteoutput = Encoding.ASCII.GetBytes(foundUser.ident+"");
						stream.Write(byteoutput, 0, byteoutput.Length);
					}
				}
			}
		}

		private void RewriteUsersFile()
		{
			File.WriteAllText(path, String.Empty);
			foreach (User user in users)
			{
				File.AppendAllText(path, user.Name + separators[0] + user.verificationVariable + separators[0] + user.ident+"\n");
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
