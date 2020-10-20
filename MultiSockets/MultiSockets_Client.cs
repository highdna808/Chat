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
            // 소켓 생성
            public Socket wSocket = null;
            // 버퍼 사이즈
            public const int iBufferSize = 1024;
            // 버퍼 크기
            public byte[] arrBuffer = new byte[iBufferSize];

            public void ClearBuffer()
            {
                Array.Clear(arrBuffer, 0, iBufferSize);
            }
        }

        public class ClientNode
        {
            private static Socket o_CLIENTSOCK;                                         // 클라이언트 메인 소켓
            private static IPAddress o_CLIENTIP;                                        // 클라이언트 IP
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
                o_CLIENTIP = IPAddress.Parse(p_IPaddress);
                IPEndPoint o_EndPoint = new IPEndPoint(o_CLIENTIP, p_Port);

                // 접속 TCP 소켓 생성
                o_CLIENTSOCK = new Socket(o_CLIENTIP.AddressFamily, SocketType.Stream, ProtocolType.IP);

                // 클라이언트 소켓이 사용중
                if (o_CLIENTSOCK.Connected)
                {
                    return;
                }

                // 소켓 바인드, 클라이언트 접속 수락 비동기 대기
                try
                {
                    o_CLIENTSOCK.Connect(p_IPaddress, p_Port);
                }
                catch (Exception ex)
                {
                    Append_SYSTEM_MSG("", "서버 접속에 실패 했습니다. (" + p_IPaddress + ":" + p_Port + ")" + "\n" + ex.Message);
                }

                // 서버(소켓) 수신상태 객체
                StateObject o_ServerState = new StateObject();

                // 서버에서 오는 메세지 수신 대기           
                o_ServerState.wSocket = o_CLIENTSOCK;
                Append_SYSTEM_MSG(o_CLIENTSOCK.LocalEndPoint.ToString(), "서버에 접속중 입니다. (" + p_IPaddress + ":" + p_Port + ")");
                o_CLIENTSOCK.BeginReceive(o_ServerState.arrBuffer, 0, StateObject.iBufferSize, 0, DataReceived, o_ServerState);
            }

            public void StartClient()
            {
                string sIP = o_CLIENTSOCK.RemoteEndPoint.ToString().Substring(0, o_CLIENTSOCK.RemoteEndPoint.ToString().IndexOf(':')).Trim();
                string sPort = o_CLIENTSOCK.RemoteEndPoint.ToString().Substring(o_CLIENTSOCK.RemoteEndPoint.ToString().IndexOf(':') + 1).Trim();

                // 받는 사람이 호스트일 경우
                if (sPort.Contains(" "))
                    sPort = sPort.Substring(0, sPort.IndexOf(' '));

                Append_SYSTEM_MSG("", "서버에 재연결중입니다. (" + o_CLIENTSOCK.RemoteEndPoint.ToString() + ")");

                // Endpoint 생성
                IPEndPoint o_EndPoint = new IPEndPoint(o_CLIENTIP, int.Parse(sPort));

                // 접속 TCP 소켓 생성
                o_CLIENTSOCK = new Socket(o_CLIENTIP.AddressFamily, SocketType.Stream, ProtocolType.IP);

                // 클라이언트 소켓이 사용중
                if (o_CLIENTSOCK.Connected)
                    return;

                // 소켓 바인드, 클라이언트 접속 수락 비동기 대기
                try
                {
                    o_CLIENTSOCK.Connect(sIP, int.Parse(sPort));
                }
                catch (Exception ex) { }

                // 서버(소켓) 수신상태 객체
                StateObject o_ServerState = new StateObject();

                // 서버에서 오는 메세지 수신 대기           
                o_ServerState.wSocket = o_CLIENTSOCK;
                try
                {
                    o_CLIENTSOCK.BeginReceive(o_ServerState.arrBuffer, 0, StateObject.iBufferSize, 0, DataReceived, o_ServerState);
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
                    StateObject o_State = (StateObject)ar.AsyncState;
                    if (!o_State.wSocket.Connected)
                    {
                        // 버퍼 비우기
                        o_State.ClearBuffer();
                        // 비동기 수신 대기
                        o_State.wSocket.BeginReceive(o_State.arrBuffer, 0, StateObject.iBufferSize, 0, DataReceived, o_State);
                        return;
                    }

                    // 수신 끝                
                    int i_Received = o_State.wSocket.EndReceive(ar);

                    // 받은 데이터가 없으면(연결끊어짐) 끝낸다.
                    if (i_Received <= 0)
                    {
                        o_State.wSocket.Close();
                        return;
                    }

                    // 받은 데이터를 텍스트로 변환
                    string s_Text = Encoding.UTF8.GetString(o_State.arrBuffer);
                    // s_Text = s_Text + '\x01' + o_State.wSocket.RemoteEndPoint.ToString();
                    string[] sMsg = s_Text.Split('\x01');
                    switch (sMsg[0])
                    {
                        //SYSTEM, CONNECT_LIST, DATA, MSG (default)
                        case "SYSTEM":
                            Append_SYSTEM_MSG(o_State.wSocket.LocalEndPoint.ToString(), sMsg[2]);
                            break;
                        case "CONNECT_LIST":
                            Append_NODE(o_State.wSocket.LocalEndPoint.ToString(), sMsg[2]);
                            break;
                        case "DATA":
                            Append_DATA(o_State.wSocket.RemoteEndPoint.ToString(), sMsg);
                            break;
                        case "MSG":
                            Append_Msg(sMsg[1], sMsg[2]);
                            break;
                        default:
                            Append_DataOnly(o_State.arrBuffer);
                            break;
                    }
                    // 버퍼 비우기
                    o_State.ClearBuffer();

                    // 비동기 수신 대기
                    o_State.wSocket.BeginReceive(o_State.arrBuffer, 0, StateObject.iBufferSize, 0, DataReceived, o_State);
                }
                catch (Exception ex)
                {
                    StartClient();
                }
            }

            public void OnSendData(MultiSockets.SocketMSG p_Msg)
            {
                if (o_CLIENTSOCK == null)
                {
                    Append_SYSTEM_MSG("", "서버와 연결되어 있지 않습니다.");
                    return;
                }
                try
                {
                    // 현재 주소 획득
                    IPEndPoint o_IP = (IPEndPoint)o_CLIENTSOCK.LocalEndPoint;
                    string sData = p_Msg.sHeader.ToString() + '\x01' + o_IP.ToString() + '\x01' + p_Msg.sMsg;

                    // 문자열을 바이트로 변환
                    byte[] arr_Data = Encoding.UTF8.GetBytes(sData);

                    // 데이터 전송
                    o_CLIENTSOCK.Send(arr_Data);
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
                    Thread t4 = new Thread(() => Run(p_Msg, p_Packet));
                    t4.Start();
                }
            }

            private void Run(SocketMSG p_Msg, SockFileTransfer.DataPacket p_Packet)
            {
                string sDataDesc = string.Empty;
                p_Msg.sSender = o_CLIENTSOCK.LocalEndPoint.ToString();
                // 헤더/보낸사람/받는사람/파일이름,파일사이즈,[1024]로 나눠 받을 패킷 수
                sDataDesc = p_Msg.sHeader.ToString() + '\x01' + p_Msg.sSender + '\x01' + p_Msg.sReceiver + '\x01' +
                             p_Packet.FileName + "/" + p_Packet.Size + "/" + p_Packet.PacketCnt + "/" + p_Packet.RemainPacket + '\x01';

                byte[] arr_DataDesc = Encoding.UTF8.GetBytes(sDataDesc);

                // 파일 정보 전송
                o_CLIENTSOCK.Send(arr_DataDesc);
            }

            public void CloseClient()
            {
                try
                {
                    o_CLIENTSOCK.Shutdown(SocketShutdown.Both);
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }
                finally
                {
                    o_CLIENTSOCK.Close(0);
                }
            }
        }
    }
}