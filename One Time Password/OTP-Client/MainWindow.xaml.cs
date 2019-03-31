using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Net;
using System.Net.Sockets;

namespace OTP_Client
{
    public partial class MainWindow : Window
    {
        int secretW;
        public int ident = 0;
        public int t = 1000;
        public int port = 5000;

        Random rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Initialization()
        {
            IPAddress IP = IPAddress.Parse("127.0.0.1");
            TcpClient client = new TcpClient();
            client.Connect(IP, port);
			
            secretW = rnd.Next(10000, 100000000);
            int w0 = H(secretW, t);

			byte[] data = Encoding.ASCII.GetBytes(userTextBox.Text + "|" + w0);
			userTextBox.IsReadOnly = true;
            NetworkStream stream = client.GetStream();

            stream.Write(data, 0, data.Length);
            int bytes = stream.Read(data, 0, data.Length);
            string responseData = Encoding.ASCII.GetString(data, 0, bytes);
            ident = Convert.ToInt32(responseData);
            validationLabel.Content = "Initialised";

            stream.Close();
            client.Close();
        }

        private void SendValidation()
        {
            IPAddress IP = IPAddress.Parse("127.0.0.1");
            TcpClient client = new TcpClient();
            client.Connect(IP, port);
			
            string wi = codeBox.Text;

			byte[] data = Encoding.ASCII.GetBytes(userTextBox.Text + "|" + ident + "|" + wi);
            NetworkStream stream = client.GetStream();
            stream.Write(data, 0, data.Length);
            data = new byte[256];
            int bytes = stream.Read(data, 0, data.Length);
            string responseData = Encoding.ASCII.GetString(data, 0, bytes);
            int aux = Convert.ToInt32(responseData);
            if (ident == aux)
            {
                validationLabel.Content = "Status: Code is not valid";
            }
            else
            {
                validationLabel.Content = "Authenticated";
                ident = aux;
            }

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
            return (int)aux;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ident == 0) Initialization();
            else SendValidation();
        }

        private void GetCodeButton_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() => MessageBox.Show(H(secretW, t - ident) + ""));
        }
    }
}
