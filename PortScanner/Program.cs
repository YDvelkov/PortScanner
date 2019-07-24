using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PingSAPServer
{
    class Program
    {
        public static int PortFrom { get; set; }
        public static int PortTo { get; set; }
        public static string IPv4 { get; set; }

        static void Main(string[] args)
        {
            try
            {
                GetUserInput();
                ScanPorts();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Port scan finished!");
        }

        static private void ScanPorts()
        {
            for (int i = PortFrom; i <= PortTo; i++)
            {
                Console.WriteLine("Scanning port {0} ({1})...", i, IPv4);

                using (TcpClient tcpClient = new TcpClient())
                {
                    tcpClient.ReceiveTimeout = 2000;
                    tcpClient.SendTimeout = 2000;
                    tcpClient.LingerState.LingerTime = 2000;

                    try
                    {
                        tcpClient.Connect(IPv4, i);
                        Console.WriteLine("Port {0} is OPEN and there is a process listening to it on the server!", i);
                    }
                    catch (SocketException ex)
                    {
                        switch (ex.ErrorCode)
                        {
                            case 10061:
                                Console.WriteLine("Port {0} is most probably OPEN but there is NO process listeting on the server. \r\nThe following message was returned: \r\n{1} \r\n", i, ex.Message);
                                break;
                            case 10060:
                                Console.WriteLine("Port {0} is CLOSED. \r\nTry opening it in the Firewall. \r\nThe following message was returned: \r\n{1} \r\n", i, ex.Message);
                                break;
                            default:
                                Console.WriteLine("There was an error connecting to the server: \r\n{0} \r\n", ex.Message);
                                break;
                        }
                    }
                }
            }
        }

        static private void GetUserInput()
        {
            try
            {
                IPAddress ip;
                int portFrom = 0;
                int portTo = 0;
                string read = String.Empty;

                Console.WriteLine("Please enter port number to start from:");
                read = Console.ReadLine();
                portFrom = Convert.ToInt32(String.IsNullOrEmpty(read) || String.IsNullOrWhiteSpace(read) ? "135" : read);
                
                Console.WriteLine("Please enter port number to end with:");
                read = Console.ReadLine();
                portTo = Convert.ToInt32(String.IsNullOrEmpty(read) || String.IsNullOrWhiteSpace(read) ? "135" : read);

                if (portFrom > portTo)
                    throw new ArgumentException("The last port in the scanned range cannot be smaller then the first!");

                PortTo = portTo;
                PortFrom = portFrom;

                Console.WriteLine("Please enter the IP address of the server:");
                read = Console.ReadLine();

                if (String.IsNullOrEmpty(read) || String.IsNullOrWhiteSpace(read))
                    read = "172.27.6.69";

                bool isIpAddress = IPAddress.TryParse(read, out ip);

                if (!isIpAddress)
                    throw new ArgumentException("The argument provided is not a valid IP address!");

                IPv4 = read;
            }
            catch (FormatException ex)
            {
                throw ex;
            }
            catch (OverflowException ex)
            {
                throw ex;
            }
        }
    }
}
