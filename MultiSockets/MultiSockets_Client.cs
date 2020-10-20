using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MultiSockets
{
    public class MultiSockets_Client
    {
        public class StateObject
        {            
            public Socket wSocket = null;                       // 소켓 생성            
            public const int BUFFER_SIZE = 1024;                // 버퍼 사이즈            
            public byte[] buffer = new byte[BUFFER_SIZE];    // 버퍼 크기

            public void ClearBuffer()
            {
                Array.Clear(buffer, 0, BUFFER_SIZE);
            }
        }

        public class ClientNode
        {
            private static Socket CLIENTSOCK;                                         // 클라이언트 메인 소켓
            private static IPAddress CLIENTIP;                                        // 클라이언트 IP
            public delegate void AppendClientDelegate(string p_Sender, string p_Msg);   // 노드 추가 델리게이트
            public delegate void AppendDataDelegate(string p_Sender, string[] p_Msg);   // 파일 정보 델리게이트
            public delegate void AppendDataOnly(byte[] p_Data);                         // 파일 델리게이트
            public event AppendDataOnly Append_DataOnly;
            public event AppendDataDelegate Append_DATA;                                // 폼->폼간 데이터
            public event AppendClientDelegate Append_NODE;                              // 클라이언트 추가
            public event AppendClientDelegate Append_SYSTEM_MSG;                        // 폼->폼간 단순 메세지            
            public event AppendClientDelegate Append_Msg;                               // 클라이언트로부터 받은 대화 메세지
            private readonly object _lockSending = new object();

            public void StartClient(string p_IPaddress, int p_Port)
            {
                // 서버 IP, Port, Endpoint 생성
                CLIENTIP = IPAddress.Parse(p_IPaddress);
                IPEndPoint o_EndPoint = new IPEndPoint(CLIENTIP, p_Port);

                // 접속 TCP 소켓 생성
                CLIENTSOCK = new Socket(CLIENTIP.AddressFamily, SocketType.Stream, ProtocolType.IP);

                // 클라이언트 소켓이 사용중
                if (CLIENTSOCK.Connected)
                {
                    return;
                }

                // 소켓 바인드, 클라이언트 접속 수락 비동기 대기
                try
                {
                    CLIENTSOCK.Connect(p_IPaddress, p_Port);
                }
                catch (Exception ex)
                {
                    Append_SYSTEM_MSG("", "서버 접속에 실패 했습니다. (" + p_IPaddress + ":" + p_Port + ")" + "\n" + ex.Message);
                }

                // 서버(소켓) 수신상태 객체
                StateObject serverState = new StateObject();

                // 서버에서 오는 메세지 수신 대기           
                serverState.wSocket = CLIENTSOCK;
                Append_SYSTEM_MSG(CLIENTSOCK.LocalEndPoint.ToString(), "서버에 접속중 입니다. (" + p_IPaddress + ":" + p_Port + ")");
                CLIENTSOCK.BeginReceive(serverState.buffer, 0, StateObject.BUFFER_SIZE, 0, DataReceived, serverState);
            }

            public void StartClient()
            {
                string myIP = CLIENTSOCK.RemoteEndPoint.ToString().Substring(0, CLIENTSOCK.RemoteEndPoint.ToString().IndexOf(':')).Trim();
                string myPort = CLIENTSOCK.RemoteEndPoint.ToString().Substring(CLIENTSOCK.RemoteEndPoint.ToString().IndexOf(':') + 1).Trim();

                // 받는 사람이 호스트일 경우
                if (myPort.Contains(" "))
                    myPort = myPort.Substring(0, myPort.IndexOf(' '));

                Append_SYSTEM_MSG("", "서버에 재연결중입니다. (" + CLIENTSOCK.RemoteEndPoint.ToString() + ")");

                // Endpoint 생성
                IPEndPoint endPoint = new IPEndPoint(CLIENTIP, int.Parse(myPort));

                // 접속 TCP 소켓 생성
                CLIENTSOCK = new Socket(CLIENTIP.AddressFamily, SocketType.Stream, ProtocolType.IP);

                // 클라이언트 소켓이 사용중
                if (CLIENTSOCK.Connected)
                    return;

                // 소켓 바인드, 클라이언트 접속 수락 비동기 대기
                try
                {
                    CLIENTSOCK.Connect(myIP, int.Parse(myPort));
                }
                catch (Exception ex) { }

                // 서버(소켓) 수신상태 객체
                StateObject serverState = new StateObject();

                // 서버에서 오는 메세지 수신 대기           
                serverState.wSocket = CLIENTSOCK;
                try
                {
                    CLIENTSOCK.BeginReceive(serverState.buffer, 0, StateObject.BUFFER_SIZE, 0, DataReceived, serverState);
                }
                catch (SocketException ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }

            /// <summary>
            /// 받은 데이터 처리
            /// </summary>
            /// <param name="ar"></param>
            private void DataReceived(IAsyncResult ar)
            {
                try
                {
                    StateObject connectState = (StateObject)ar.AsyncState;
                    if (!connectState.wSocket.Connected)
                    {
                        // 버퍼 비우기
                        connectState.ClearBuffer();
                        // 비동기 수신 대기
                        connectState.wSocket.BeginReceive(connectState.buffer, 0, StateObject.BUFFER_SIZE, 0, DataReceived, connectState);
                        return;
                    }

                    // 수신 끝                
                    int receivedBuffer = connectState.wSocket.EndReceive(ar);

                    // 받은 데이터가 없으면(연결끊어짐) 끝낸다.
                    if (receivedBuffer <= 0)
                    {
                        connectState.wSocket.Close();
                        return;
                    }

                    // 받은 데이터를 텍스트로 변환
                    string buffToText = Encoding.UTF8.GetString(connectState.buffer);
                    // s_Text = s_Text + '\x01' + o_State.wSocket.RemoteEndPoint.ToString();
                    string[] msg = buffToText.Split('\x01');
                    switch (msg[0])
                    {
                        //SYSTEM, CONNECT_LIST, DATA, MSG (default)
                        case "SYSTEM":
                            Append_SYSTEM_MSG(connectState.wSocket.LocalEndPoint.ToString(), msg[2]);
                            break;
                        case "CONNECT_LIST":
                            Append_NODE(connectState.wSocket.LocalEndPoint.ToString(), msg[2]);
                            break;
                        case "DATA":
                            Append_DATA(connectState.wSocket.RemoteEndPoint.ToString(), msg);
                            break;
                        case "MSG":
                            Append_Msg(msg[1], msg[2]);
                            break;
                        default:
                            Append_DataOnly(connectState.buffer);
                            break;
                    }
                    // 버퍼 비우기
                    connectState.ClearBuffer();

                    // 비동기 수신 대기
                    connectState.wSocket.BeginReceive(connectState.buffer, 0, StateObject.BUFFER_SIZE, 0, DataReceived, connectState);
                }
                catch (Exception ex)
                {
                    StartClient();
                }
            }

            public void OnSendData(MultiSockets.SocketMSG msg)
            {
                if (CLIENTSOCK == null)
                {
                    Append_SYSTEM_MSG("", "서버와 연결되어 있지 않습니다.");
                    return;
                }
                try
                {
                    // 현재 주소 획득
                    IPEndPoint iPEndPoint = (IPEndPoint)CLIENTSOCK.LocalEndPoint;
                    string sData = msg.sHeader.ToString() + '\x01' + iPEndPoint.ToString() + '\x01' + msg.sMsg;

                    // 문자열을 바이트로 변환
                    byte[] txtToByte = Encoding.UTF8.GetBytes(sData);

                    // 데이터 전송
                    CLIENTSOCK.Send(txtToByte);
                }
                catch (Exception ex)
                {
                    Append_SYSTEM_MSG("", "메세지 전송에 실패 했습니다.");
                }
            }

            public void OnSendData(MultiSockets.SocketMSG p_Msg, SockFileTransfer.DataPacket p_Packet)
            {
                lock (_lockSending)
                {
                    Thread thread = new Thread(() => Run(p_Msg, p_Packet));
                    thread.Start();
                }
            }

            private void Run(SocketMSG msg, SockFileTransfer.DataPacket Packet)
            {
                string sDataDesc = string.Empty;
                msg.sSender = CLIENTSOCK.LocalEndPoint.ToString();
                // 헤더/보낸사람/받는사람/파일이름,파일사이즈,[1024]로 나눠 받을 패킷 수
                sDataDesc = msg.sHeader.ToString() + '\x01' + msg.sSender + '\x01' + msg.sReceiver + '\x01' +
                             Packet.FileName + "/" + Packet.Size + "/" + Packet.PacketCnt + "/" + Packet.RemainPacket + '\x01';

                byte[] txtDataDesc = Encoding.UTF8.GetBytes(sDataDesc);

                // 파일 정보 전송
                CLIENTSOCK.Send(txtDataDesc);
            }

            public void CloseClient()
            {
                try
                {
                    CLIENTSOCK.Shutdown(SocketShutdown.Both);
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }
                finally
                {
                    CLIENTSOCK.Close(0);
                }
            }
        }
    }
}