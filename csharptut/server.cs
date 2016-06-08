using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Collections;

namespace ConsoleApplication1
{
    class Program
    {
        public static Hashtable clientsList = new Hashtable();

        static void Main(string[] args)
        {
            TcpListener serverSocket = new TcpListener(8888);
            TcpClient clientSocket = default(TcpClient);
            int counter = 0;


            serverSocket.Start();
            Console.WriteLine("Chat Server Started ....");
            counter = 0;

            string dataFromClient;
            while ((true))
            {
                counter += 1;
                clientSocket = serverSocket.AcceptTcpClient();
                dataFromClient = null;

                byte[] bytesFrom = new byte[2048];
                NetworkStream networkStream = clientSocket.GetStream();
                //Int32 buffsize = clientSocket.ReceiveBufferSize;
                networkStream.Read(bytesFrom, 0, bytesFrom.Length);
                dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));

                clientsList.Add(dataFromClient, clientSocket);

                broadcast(dataFromClient + " Joined ", dataFromClient, false);
                Console.WriteLine(dataFromClient + " Joined chat room ");
                handleClinet client = new handleClinet();
                client.startClient(clientSocket, dataFromClient, clientsList);
            }

            clientSocket.Close();
            serverSocket.Stop();
            Console.WriteLine("exit");
            Console.ReadLine();
        }
        public static bool IsConnected(TcpClient _tcpClient)
        {
                try
                {
                    if (_tcpClient != null && _tcpClient.Client != null && _tcpClient.Client.Connected)
                    {

                        // Detect if client disconnected
                        if (_tcpClient.Client.Poll(0, SelectMode.SelectRead))
                        {
                            byte[] buff = new byte[1];
                            if (_tcpClient.Client.Receive(buff, SocketFlags.Peek) == 0)
                            {
                                // Client disconnected
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
        }
        public static void broadcast(string msg, string uName, bool flag)
        {
            foreach (DictionaryEntry Item in clientsList)
            {
                TcpClient broadcastSocket;
                broadcastSocket = (TcpClient)Item.Value;
                NetworkStream broadcastStream = broadcastSocket.GetStream();
                Byte[] broadcastBytes = null;
                    if (flag == true)
                    {
                        broadcastBytes = Encoding.ASCII.GetBytes(uName + " says : " + msg);
                    }
                    else
                    {
                        broadcastBytes = Encoding.ASCII.GetBytes(msg);
                    }

                broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                broadcastStream.Flush();
            }
        }  //end broadcast function
    }//end Main class


    public class handleClinet
    {
        TcpClient clientSocket;
        string clNo;
        Hashtable clientsList;

        public void startClient(TcpClient inClientSocket, string clineNo, Hashtable cList)
        {
            Thread ctThread = new Thread(doChat);
            try
            {
                this.clientSocket = inClientSocket;
            this.clNo = clineNo;
            this.clientsList = cList;
            ctThread.Start();
            }
            catch (Exception ex)
            {
            if (ex is InvalidOperationException)
                ctThread.Abort();
            else
                Console.WriteLine(ex.ToString());
            }
}

        private void doChat()
        {
            int requestCount = 0;
            byte[] bytesFrom = new byte[10025];
            string dataFromClient = null;
            Byte[] sendBytes = null;
            string serverResponse = null;
            string rCount = null;
            requestCount = 0;

            while ((true))
            {
                try
                {
                    requestCount = requestCount + 1;
                    NetworkStream networkStream = clientSocket.GetStream();
                    Int32 buffersize = (int)clientSocket.ReceiveBufferSize;
                    networkStream.Read(bytesFrom, 0, 2048);
                    dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                    dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));
                    if (dataFromClient == "Disconnect")
                    {
                        Console.WriteLine(clNo + " Left chat room ");
                    }
                    else {
                        Console.WriteLine("From client - " + clNo + " : " + dataFromClient);
                    }

                    rCount = Convert.ToString(requestCount);

                    Program.broadcast(dataFromClient, clNo, true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }//end while
        }//end doChat
    } //end class handleClinet
}//end namespace