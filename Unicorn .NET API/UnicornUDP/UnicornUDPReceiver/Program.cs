using System;
using System.Net;
using System.Net.Sockets;

namespace UnicornUDPReceiver
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Unicorn UDP Receiver Example");
            Console.WriteLine("----------------------------");
            Console.WriteLine();
            try
            {
                //define an IP endpoint
                Console.Write("Destination port: ");
                int port = Convert.ToInt32(Console.ReadLine());
                IPAddress ip = IPAddress.Any;
                IPEndPoint endPoint = new IPEndPoint(ip, port);
                Console.WriteLine("Listening on port '{0}'...",port);

                //initialize upd socket
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socket.Bind(endPoint);
                byte[] receiveBufferByte = new byte[1024];
                float[] receiveBufferFloat= new float[receiveBufferByte.Length / sizeof(float)];

                //acquisition loop
                while (true)
                {
                    int numberOfBytesReceived = socket.Receive(receiveBufferByte);
                    if (numberOfBytesReceived > 0)
                    {
                        //convert byte array to float array
                        for (int i = 0; i < numberOfBytesReceived / sizeof(float); i++)
                        {
                            receiveBufferFloat[i] = BitConverter.ToSingle(receiveBufferByte, i * sizeof(float));
                            if(i+1< numberOfBytesReceived / sizeof(float))
                                Console.Write("{0},", receiveBufferFloat[i].ToString("n2"));
                            else
                                Console.WriteLine("{0}", receiveBufferFloat[i].ToString("n2"));
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
                Console.WriteLine("Press ENTER to terminate the application.");
                Console.ReadLine();
            }
        }
    }
}
