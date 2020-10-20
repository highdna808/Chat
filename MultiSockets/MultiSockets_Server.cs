using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace MultiSockets
{
    public class MultiSockets_Server
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

        public class ServerNode
        {
            private List<Socket> lst_ConnectedClients = new List<Socket>();     // 접속된 클라이언트 리스트
            private static Socket o_SERVERSOCK;                                 // 서버 소켓
            private static IPAddress o_SERVERIP;                                // 서버 IP

            public delegate void AppendServerDelegate(string p_Sender, string p_Msg);            // 서버 델리게이트
            public event AppendServerDelegate BoardCast;                        // 클라이언트 추가 메세지
            public event AppendServerDelegate AppendNodeMsg;                    // 클라이언트로부터 받은 대화 메세지
            public event AppendServerDelegate ExitNode;                         // 클라이언트로부터 받은 시스템 메세지

            public delegate void AppendDataDelegate(string p_Sender, string[] p_Msg);   // 파일 정보 델리게이트
            public delegate void AppendDataOnly(byte[] p_Data);                         // 파일 델리게이트
            public event AppendDataOnly Append_DataOnly;
            public event AppendDataDelegate Append_DATA;                                // 폼->폼간 데이터

            /// <summary>
            /// 서버 객체 생성
            /// </summary>
            /// <param name="p_IPaddress"></param> 서버 IP
            /// <param name="p_Port"></param> 서버 Port
            public void StartServer(string p_IPaddress, int p_Port)
            {
                // 서버 IP, Port, Endpoint 생성
                o_SERVERIP = IPAddress.Parse(p_IPaddress);
                IPEndPoint o_EndPoint = new IPEndPoint(o_SERVERIP, p_Port);

                // 접속 TCP 소켓 생성
                o_SERVERSOCK = new Socket(o_SERVERIP.AddressFamily, SocketType.Stream, ProtocolType.IP);

                // 소켓 바인드 (10 노드)
                o_SERVERSOCK.Bind(o_EndPoint);
                o_SERVERSOCK.Listen(10);

                // 클라이언트 접속 수락 비동기 대기
                o_SERVERSOCK.BeginAccept(AcceptCallback, null);
            }

            public void CloseServer()
            {
                try
                {
                    o_SERVERSOCK.Shutdown(SocketShutdown.Both);
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }
                finally
                {
                    o_SERVERSOCK.Close(0);
                }
            }
            /// <summary>
            /// 접속 요청 수신
            /// </summary>
            /// <param name="ar"></param>
            private void AcceptCallback(IAsyncResult ar)
            {
                try
                {
                    // 클라이언트 접속 요청 수락
                    Socket o_SockClient = o_SERVERSOCK.EndAccept(ar);
                    // 다른 클라이언트 접속 비동기 수락
                    o_SERVERSOCK.BeginAccept(AcceptCallback, null);

                    // 현재 클라이언트 상태 객체
                    StateObject o_ClientState = new StateObject();
                    o_ClientState.wSocket = o_SockClient;

                    // 클라이언트를 정보를 리스트에 추가
                    lst_ConnectedClients.Add(o_SockClient);

                    // 클라이언트 접속 브로드캐스트 메세지 20200828
                    BoardCast("", o_SockClient.RemoteEndPoint.ToString());

                    // 클라이언트로부터 비전송 데이터 수신 대기
                    o_SockClient.BeginReceive(o_ClientState.arrBuffer, 0, StateObject.iBufferSize, 0, ReadCallback, o_ClientState);
                    o_SockClient.SendFile("");
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }
            }

            /// <summary>
            /// 메세지 수신
            /// </summary>
            /// <param name="ar"></param>
            private void ReadCallback(IAsyncResult ar)
            {
                // BeginReceive에서 추가적으로 넘어온 데이터를 AsyncObject 형식으로 변환한다.
                StateObject o_ClientState = (StateObject)ar.AsyncState;
                int received;

                // 데이터 수신을 끝낸다.
                if (o_ClientState.arrBuffer[0] > 1)
                {
                    received = o_ClientState.wSocket.EndReceive(ar);
                }
                else
                {
                    o_ClientState.wSocket.Close();
                    return;
                }

                // 데이터가 없으면 연결 종료
                if (received <= 0)
                {
                    o_ClientState.wSocket.Close();
                    return;
                }

                // 텍스트로 변환
                string sText = Encoding.UTF8.GetString(o_ClientState.arrBuffer);
                string[] s_Msg = sText.Split('\x01');

                switch (s_Msg[0])
                {
                    //SYSTEM, CONNECT_LIST, DATA, MSG (default)
                    case "SYSTEM":
                        //Append_SYSTEM_MSG(o_State.wSocket.RemoteEndPoint.ToString(), sMsg[2]);
                        break;
                    case "DATA":
                        Append_DATA(o_ClientState.wSocket.RemoteEndPoint.ToString(), s_Msg);
                        MsgCallBackOnly(ar);
                        return;
                    case "MSG":
                        AppendNodeMsg(s_Msg[1], s_Msg[2]);
                        MsgCallBacktoAll(ar);
                        break;
                    case "EXIT":
                        ExitNode(s_Msg[1], s_Msg[2]);
                        return;
                    default:
                        Append_DataOnly(o_ClientState.arrBuffer);
                        MsgCallBackOnly(ar);
                        break;
                }
            }
            private void MsgCallBackOnly(IAsyncResult ar)
            {
                // BeginReceive에서 추가적으로 넘어온 데이터를 AsyncObject 형식으로 변환한다.
                StateObject o_ClientState = (StateObject)ar.AsyncState;
                // 데이터를 받은 후엔 다시 버퍼를 비워주고 같은 방법으로 수신을 대기한다.
                o_ClientState.ClearBuffer();
                // 현재 클라이언트 재수신 대기
                o_ClientState.wSocket.BeginReceive(o_ClientState.arrBuffer, 0, StateObject.iBufferSize, 0, ReadCallback, o_ClientState);
            }
            private void MsgCallBacktoAll(IAsyncResult ar)
            {
                // BeginReceive에서 추가적으로 넘어온 데이터를 AsyncObject 형식으로 변환한다.
                StateObject o_ClientState = (StateObject)ar.AsyncState;

                // for을 통해 "역순"으로 클라이언트에게 데이터를 보낸다.
                for (int i = lst_ConnectedClients.Count - 1; i >= 0; i--)
                {
                    Socket socket = lst_ConnectedClients[i];
                    if (socket != o_ClientState.wSocket)
                    {
                        try
                        {
                            System.Threading.Thread.Sleep(50);
                            socket.Send(o_ClientState.arrBuffer);
                        }
                        catch
                        {
                            // 오류 발생하면 전송 취소하고 리스트에서 삭제한다.
                            try { socket.Dispose(); }
                            catch { }
                            lst_ConnectedClients.RemoveAt(i);
                        }
                    }
                }

                // 데이터를 받은 후엔 다시 버퍼를 비워주고 같은 방법으로 수신을 대기한다.
                o_ClientState.ClearBuffer();

                // 현재 클라이언트 재수신 대기
                o_ClientState.wSocket.BeginReceive(o_ClientState.arrBuffer, 0, StateObject.iBufferSize, 0, ReadCallback, o_ClientState);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="p_Msg"></param>    데이터 정보 sHeader/sSender/sReceiver/sMsg
            /// <param name="p_Packet"></param> 보내는 데이터 FileName/PacketCnt/Size(length)/Data
            public void FileSend(SocketMSG p_Msg, SockFileTransfer.DataPacket p_Packet)
            {
                Socket socket;
                string sDataDesc = string.Empty;
                // 헤더/보낸사람/받는사람/파일이름,파일사이즈,[1024]로 나눠 받을 패킷 수
                sDataDesc = p_Msg.sHeader.ToString() + '\x01' + p_Msg.sSender + '\x01' + p_Msg.sReceiver + '\x01' +
                             p_Packet.FileName + "/" + p_Packet.Size + "/" + p_Packet.PacketCnt + "/" + p_Packet.RemainPacket + '\x01';
                for (int i = 0; i < lst_ConnectedClients.Count; i++)
                {
                    if (lst_ConnectedClients[i].RemoteEndPoint.ToString() == p_Msg.sReceiver)
                    {
                        socket = lst_ConnectedClients[i];
                        byte[] arr_DataDesc = Encoding.UTF8.GetBytes(sDataDesc);
                        // 파일 정보 전송
                        socket.Send(arr_DataDesc);
                        // 데이터 전송
                        foreach (byte[] buff in p_Packet.Data.Values)
                        {
                            socket.Send(buff);
                        }
                    }
                }
            }

            public void MessageSend(MultiSockets.SocketMSG p_Msg)
            {
                string sData = string.Empty;
                // 현재 주소 획득
                IPEndPoint o_IP = (IPEndPoint)o_SERVERSOCK.LocalEndPoint;
                p_Msg.sSender = o_IP.ToString();
                sData = p_Msg.sHeader.ToString() + '\x01' + p_Msg.sSender + '\x01' + p_Msg.sMsg;
                // 입력받은 문자열을 utf8 형식의 바이트로 변환
                byte[] arr_Message = Encoding.UTF8.GetBytes(sData);

                // 연결된 모든 클라이언트에게 전송한다.
                for (int i = lst_ConnectedClients.Count - 1; i >= 0; i--)
                {
                    Socket socket = lst_ConnectedClients[i];
                    try
                    {
                        System.Threading.Thread.Sleep(50);
                        socket.Send(arr_Message);
                    }
                    catch
                    {
                        // 오류 발생하면 전송 취소하고 리스트에서 삭제한다.
                        try { socket.Dispose(); }
                        catch { }
                        lst_ConnectedClients.RemoveAt(i);
                    }
                }
            }
        }
    }
}
