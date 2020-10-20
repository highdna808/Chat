using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiSockets
{
    public class MultiSockets_FileTransfer
    {
        // Server(Listen)
        public class FileSocketServer
        {
            // Incoming data from the client
            public static string data = null;

            // Socket listen, accept
            private Socket listener = null;
            private Socket handler = null;
            private int i_PacketCnt = 0;
            private byte[] arr_Data;
            public static string TruncateLeft(string value, int maxLength)
            {
                if (string.IsNullOrEmpty(value)) return value;
                return value.Length <= maxLength ? value : value.Substring(0, maxLength);
            }

            // Get local IP
            public static string LocalIPAddress()
            {
                IPHostEntry host;
                string localIP = "";
                host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (IPAddress ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        localIP = ip.ToString();
                        return localIP;
                    }
                }
                return "127.0.0.1";
            }

            private void DataInsert(byte[] byteData, byte[] b)
            {
                // 수신받은 데이터(바이트배열) 복사 위치
                int i_Packagelen = 0;
                if (i_PacketCnt > 0)
                    i_Packagelen = (i_PacketCnt * 1024);

                // 마지막 패킷일 경우 마지막으로 받은 패킷 크기 지정
                if (2 > ReceivedFile._PacketCnt)
                    Array.Copy(b, 0, ReceivedFile._Data, i_Packagelen, ReceivedFile._RemainPacket);
                else
                    Array.Copy(b, 0, ReceivedFile._Data, i_Packagelen, b.Length);
                // 받은 패킷 카운트 증가, 남은 패킷 수 계산
                i_PacketCnt += 1;
                ReceivedFile._PacketCnt -= 1;

                // 파일 전송 완료
                if (1 > ReceivedFile._PacketCnt)
                {
                    i_PacketCnt = 0;
                }
            }

            public void Listen()
            {
                // Data buffer from incoming data
                byte[] bytes = new byte[1024];

                // Establish the local endpoint for the socket
                // Dns.GetHostName returns the name of the
                // host running the application
                IPAddress localIPAddress = IPAddress.Parse(LocalIPAddress());
                IPEndPoint localEndPoint = new IPEndPoint(localIPAddress, 12000);

                // Create a TCP/IP Socket
                listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

                // Bind the socket to the local endpoint and
                // listen for incoming connections.
                try
                {
                    listener.Bind(localEndPoint);
                    listener.Listen(10);

                    new Thread(delegate ()
                    {
                        // Start listening for connections.
                        while (true)
                        {
                            try
                            {
                                // Program is suspended while waiting for an incoming connection.
                                handler = listener.Accept();
                            }
                            catch (SocketException ex)
                            {
                                listener.Close();
                                listener.Dispose();
                                break;
                            }
                            try
                            {
                                arr_Data = new byte[ReceivedFile._Size];
                                while (true)
                                {
                                    int bytesRec = handler.Receive(bytes);
                                    DataInsert(arr_Data, bytes);

                                    // 남은 패킷 0
                                    if (i_PacketCnt < 1)
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.ToString());
                                handler.Close();
                                handler.Dispose();
                                break;
                            }
                        }
                    }).Start();
                }
                catch (SocketException se)
                {
                    MessageBox.Show("SocketException 에러 : " + se.ToString());
                    switch (se.SocketErrorCode)
                    {
                        case SocketError.ConnectionAborted:
                        case SocketError.ConnectionReset:
                            handler.Shutdown(SocketShutdown.Both);
                            handler.Close();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Exception 에러 : " + ex.ToString());
                }
            }

            // 서버 소켓 종료
            public void SockClose()
            {
                if (handler != null)
                {
                    handler.Close();
                    handler.Dispose();
                }

                if (listener != null)
                {
                    listener.Close();
                    listener.Dispose();
                }
            }
        }

        // Client(Send)
        public class FileSocketClient
        {
            // Client socket
            private Socket sock = null;

            // Data buffer from incomming data
            byte[] bytes = new byte[1024];

            public void ClientFileSend(SocketMSG p_Msg, SockFileTransfer.DataPacket p_Packet, string pIPAddress)
            {
                new Thread(() =>
                {
                    try
                    {
                        //// Create a TCP / IP Socket
                        sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                        // //Connect to the server
                        sock.Connect(new IPEndPoint(IPAddress.Parse(pIPAddress), 12000));

                        // 데이터 전송
                        foreach (byte[] buff in p_Packet.Data.Values)
                        {
                            sock.Send(buff);
                        }
                    }
                    catch (SocketException se)
                    {
                        MessageBox.Show("SocketException = " + se.Message.ToString());
                        sock.Close();
                        sock.Dispose();
                    }
                }).Start();
            }

            private void btnDisconnect_Click(object sender, EventArgs e)
            {
                sock.Shutdown(SocketShutdown.Both);
                sock.Close();
            }
        }
    }
}