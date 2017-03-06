using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Socket_Client
{
    class SocketClient_11500
    {
        //connectport: Connect erver Port number 
        //screenPort: Screen sharing port number
        //port: local port address
        private static string local_IP, server_IP = "192.168.0.28",
            connectPort = "11500", PC_name, mac, screenPort, port, denemeport = "11501";

        readonly static string fileName = "RemoteConnection_Client.exe";
        private readonly static string path = Path.Combine(Environment.CurrentDirectory, fileName);

        private static Socket client_socket = new Socket
            (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static IPEndPoint endpoint = new IPEndPoint
            (IPAddress.Parse(server_IP), Convert.ToInt32(connectPort));

        public static void Start()
        {
            local_IP = GetIPAddress();
            mac = GetMACAddress();
            try
            {
                ConnectLoop();
                SendPCInfo();
                WaitServer_Command();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                Environment.Exit(0);
            }
        }

        //Server'a baglanma denemesi
        private static void ConnectLoop()
        {
            int attempts = 0;

            while (!client_socket.Connected)
            {
                try
                {
                    attempts++;
                    IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(server_IP), Convert.ToInt32(connectPort));
                    client_socket.Connect(endpoint);
                }
                catch (SocketException ex)
                {
                    Console.Clear();
                    Console.WriteLine("Bağlanma Denemesi: " + attempts);
                }
            }
            Console.WriteLine("Bağlandı");
        }

        private static void SendPCInfo()
        {
            byte[] buffer = new byte[128];
            if (PC_name == null)
                PC_name = Environment.MachineName;
            string info = "client_info:" + local_IP + ":" + PC_name + ":" + mac;
            buffer = Encoding.ASCII.GetBytes(info);
            client_socket.Send(buffer);

            //client_socket.Close();
            //client_socket.Dispose();
            //Environment.Exit(0);
        }

        //Server'dan gelen komutları dinleme
        private static void WaitServer_Command()
        {
            try
            {
                Console.WriteLine("11500'e bağlandı");
                Console.WriteLine("Server Komutu Bekleniyor...");

                byte[] buffer = new byte[1024];
                int recieved = client_socket.Receive(buffer);
                byte[] data = new byte[recieved];
                Array.Copy(buffer, data, recieved); //source, destination, lenght

                string request = Encoding.UTF8.GetString(data);

                if (request == "ekran")
                {
                    Start_EkranPaylas();
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message.ToString());
                client_socket.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                client_socket.Close();
            }
        }

        //Server'dan komut olarak "ekran" girilirse
        private static void Start_EkranPaylas()
        {
            Process start_ClientRemote = new Process();
            try
            {
                start_ClientRemote.StartInfo.FileName = path;
                start_ClientRemote.StartInfo.Arguments = server_IP + ':' + denemeport;
                start_ClientRemote.Start();
                start_ClientRemote.WaitForExit();
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                start_ClientRemote.Kill();
                start_ClientRemote.Dispose();
            }
        }

        private static string GetMACAddress()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            String sMacAddress = string.Empty;
            foreach (NetworkInterface adapter in nics)
            {
                if (sMacAddress == String.Empty)// only return MAC Address from first card  
                {
                    sMacAddress = adapter.GetPhysicalAddress().ToString();
                }
            }
            return sMacAddress;
        }

        private static string GetIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }
    }
}
