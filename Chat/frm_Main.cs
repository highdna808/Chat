using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

using MultiSockets;
using System.Runtime.InteropServices;
using System.IO;


namespace SocketChatandFile
{
    public partial class frm_Main : Form
    {
        [DllImport("User32.dll")]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, uint dwDetail);
        private const uint L_down = 0x0002;

        private static MultiSockets_FileTransfer.FileSocketServer clsFileServer = new MultiSockets_FileTransfer.FileSocketServer();
        private static MultiSockets_Server.ServerNode clsServer = new MultiSockets_Server.ServerNode();  // 서버 객체
        private static MultiSockets_Client.ClientNode clsClient = new MultiSockets_Client.ClientNode();  // 클라이언트 객체        
        private static SocketMSG clsMSG = new SocketMSG();       // 메세지
        private static bool isServer = false;                 // 서버/클라이언트 구분 true : Server, False : Client
        private static bool isConnect = false;                // 서버 연결 확인 플래그      
        private static string clientIP = string.Empty;
        private static string myIP = string.Empty;
        private int packetCnt = 0;

        public frm_Main()
        {
            InitializeComponent();
            FindIPv4();
            clsServer.AppendNodeMsg += new MultiSockets_Server.ServerNode.AppendServerDelegate(this.AppendNodeMsg);
        }

        private void btn_Create_Click(object sender, EventArgs e)
        {
            bool isIPValid = ValidateIPv4(txt_IPaddr.Text);
            bool isPortValid = CheckPort();
            clsServer.BoardCast += new MultiSockets_Server.ServerNode.AppendServerDelegate(this.BroadCastMsg);
            clsServer.ExitNode += new MultiSockets_Server.ServerNode.AppendServerDelegate(this.ExitNodeMsg);
            clsServer.Append_DATA += new MultiSockets_Server.ServerNode.AppendDataDelegate(this.Received_DATA);
            clsServer.Append_DataOnly += new MultiSockets_Server.ServerNode.AppendDataOnly(this.Received_DataOnly);
            clsFileServer.Listen();

            //  아이피, 포트 확인
            if (!isIPValid || !isPortValid)
            {
                MessageBox.Show("IP주소 또는 Port를 확인해주세요.");
                return;
            }

            // 서버 시작
            try
            {
                string currentTime = "[" + DateTime.Now.ToString("yy/MM/dd HH:mm:ss") + "] ";
                clsServer.StartServer(txt_IPaddr.Text, int.Parse(txt_Port.Text));
                txt_TextBox.Text = txt_TextBox.Text + Environment.NewLine + currentTime + "Server Open";
                btn_Connect.Enabled = false;
                btn_Disconnect.Enabled = false;
                btn_Create.BackColor = Color.Lime;
                isServer = true;
                myIP = txt_IPaddr.Text + ":" + txt_Port.Text;
                listBox1.Items.Add(txt_IPaddr.Text + ":" + txt_Port.Text + " (Host)");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // 리스트에 담기 -> 타이머체커로 메세지 보내기(서버,클라)
        private void CheckConnectiList()
        {
            string msg = string.Empty;
            for (int i = 1; i <= listBox1.Items.Count; i++)
            {
                if (listBox1.Items.Count == i)
                {
                    msg += listBox1.Items[i - 1].ToString();
                }
                else
                    msg += listBox1.Items[i - 1] + "/";
            }
            clsMSG.sHeader = SocketMSG.MESSAGE_TYPE.CONNECT_LIST;
            clsMSG.sMsg = msg;
            clsServer.MessageSend(clsMSG);
        }

        private void btn_Connect_Click(object sender, EventArgs e)
        {
            bool isIPValid = ValidateIPv4(txt_IPaddr.Text);
            bool isPortValid;

            if (txt_Port.Text == "")
            {
                MessageBox.Show("IP주소 또는 Port를 확인해주세요.");
                return;
            }
            // 포트 범위 이외의 포트번호 확인
            if ((double.Parse(txt_Port.Text) < 65536 && int.Parse(txt_Port.Text) > 0))
                isPortValid = true;
            else
                isPortValid = false;

            if (!isIPValid || !isPortValid)
            {
                MessageBox.Show("IP주소 또는 Port를 확인해주세요.");
                return;
            }

            // 서버에 접속
            try
            {
                clsClient.Append_Msg += new MultiSockets_Client.ClientNode.AppendClientDelegate(this.AppendNodeMsg);
                clsClient.Append_SYSTEM_MSG += new MultiSockets_Client.ClientNode.AppendClientDelegate(this.SYSTEM_MSG);
                clsClient.Append_NODE += new MultiSockets_Client.ClientNode.AppendClientDelegate(this.CONNECTED_LIST);
                clsClient.Append_DATA += new MultiSockets_Client.ClientNode.AppendDataDelegate(this.Received_DATA);
                clsClient.Append_DataOnly += new MultiSockets_Client.ClientNode.AppendDataOnly(this.Received_DataOnly);
                clsClient.StartClient(txt_IPaddr.Text, int.Parse(txt_Port.Text));
                btn_Create.Enabled = false;
                btn_Connect.Enabled = false;
                isConnect = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("연결에 실패 했습니다. \n" + ex.Message);
                clsClient.Append_Msg -= new MultiSockets_Client.ClientNode.AppendClientDelegate(this.AppendNodeMsg);
                clsClient.Append_SYSTEM_MSG -= new MultiSockets_Client.ClientNode.AppendClientDelegate(this.SYSTEM_MSG);
                clsClient.Append_NODE -= new MultiSockets_Client.ClientNode.AppendClientDelegate(this.CONNECTED_LIST);
                clsClient.Append_DATA -= new MultiSockets_Client.ClientNode.AppendDataDelegate(this.Received_DATA);
                clsClient.Append_DataOnly -= new MultiSockets_Client.ClientNode.AppendDataOnly(this.Received_DataOnly);
                btn_Create.Enabled = true;
                btn_Connect.Enabled = true;
                isConnect = false;
            }
        }
        private delegate void ReceivedDataOnly();
        /// <summary>
        /// 파일 수신
        /// </summary>
        /// <param name="pData"></param>
        private void Received_DataOnly(byte[] pData)
        {
            Invoke(new ReceivedDataOnly(delegate
            {
                // 수신받은 데이터(바이트배열) 복사 위치
                int packageLength = 0;
                if (packetCnt > 0)
                    packageLength = (packetCnt * 1024);

                // 마지막 패킷일 경우 마지막으로 받은 패킷 크기 지정
                if (2 > ReceivedFile._PacketCnt)
                    Array.Copy(pData, 0, ReceivedFile._Data, packageLength, ReceivedFile._RemainPacket);
                else
                    Array.Copy(pData, 0, ReceivedFile._Data, packageLength, pData.Length);

                // 받은 패킷 카운트 증가, 남은 패킷 수 계산
                packetCnt += 1;
                ReceivedFile._PacketCnt -= 1;

                // 파일 전송 완료
                if (1 > ReceivedFile._PacketCnt)
                {
                    packetCnt = 0;
                    frm_Alarm frm_Alarm = new frm_Alarm(ReceivedFile._sender, ReceivedFile._FileName);
                    frm_Alarm.Show();
                }
            }));
        }

        private delegate void ReceivedData();
        private void Received_DATA(string sender, string[] msg)
        {
            Invoke(new ReceivedData(delegate
            {
                if (msg[0] == "DATA")
                {
                    var fileInfo = msg[3].Split('/');
                    ReceivedFile._sender = msg[1];
                    ReceivedFile._Receiver = msg[2];
                    ReceivedFile._FileName = fileInfo[0];
                    ReceivedFile._Size = int.Parse(fileInfo[1]);
                    ReceivedFile._PacketCnt = int.Parse(fileInfo[2]);
                    ReceivedFile._Data = new byte[ReceivedFile._Size];
                    ReceivedFile._RemainPacket = int.Parse(fileInfo[3]);

                    // 서버 관리용 파일 관리 정보
                    StoredPackage SPakage = new StoredPackage(ReceivedFile._sender,
                                            ReceivedFile._Receiver, ReceivedFile._FileName, ReceivedFile._Size);

                    if (isServer)
                    {
                        if (ReceivedFile._Receiver == myIP)
                        {
                            // 서버가 수신인
                            frm_Alarm frmAlarm = new frm_Alarm(ReceivedFile._sender, ReceivedFile._FileName);
                            frmAlarm.Show();
                        }
                        else
                        {
                            // 클라이언트 -> 서버 -> 수신인
                            clsMSG.sSender = ReceivedFile._sender;
                            clsMSG.sReceiver = ReceivedFile._Receiver;
                            clsMSG.sHeader = SocketMSG.MESSAGE_TYPE.DATA;
                            FileStream fileStr = new FileStream(ReceivedFile._FileName, FileMode.Create, FileAccess.Write);
                            BinaryWriter writer = new BinaryWriter(fileStr);
                            writer.Write(ReceivedFile._Data, 0, ReceivedFile._Size);
                            fileStr.Close();
                            // 보낼 파일
                            SockFileTransfer sockFileTransfer = new SockFileTransfer();
                            var o_Packet = sockFileTransfer.FileSelect(ReceivedFile._FileName);
                            clsServer.FileSend(clsMSG, o_Packet);
                        }
                    }
                }
            }));
        }

        /// <summary>
        /// 현재 사용중이 IPv4 주소 찾기
        /// </summary>
        private void FindIPv4()
        {
            IPHostEntry o_HostEntry = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress thisAddress = null;

            // 컴퓨터안의 ipv4 주소 찾기
            foreach (IPAddress addr in o_HostEntry.AddressList)
            {
                if (addr.AddressFamily == AddressFamily.InterNetwork)
                {
                    thisAddress = addr;
                    break;
                }
            }

            // IPv4 주소가 없을 경우 루프백 사용
            if (thisAddress == null)
                thisAddress = IPAddress.Loopback;

            txt_IPaddr.Text = thisAddress.ToString();
        }

        /// <summary>
        /// IP Validate 검사
        /// </summary>
        /// <param name="ipString"></param> IP 주소
        /// <returns></returns>
        private bool ValidateIPv4(string ipString)
        {
            if (String.IsNullOrWhiteSpace(ipString))
            {
                return false;
            }

            string[] splitValues = ipString.Split('.');
            if (splitValues.Length != 4)
            {
                return false;
            }

            byte tempForParsing;

            return splitValues.All(r => byte.TryParse(r, out tempForParsing));
        }

        /// <summary>
        /// 입력된 포트 번호 확인
        /// </summary>
        /// <returns></returns>
        private bool CheckPort()
        {
            bool isNotInUse = true;
            int isNumber;
            bool successfullyParsed = int.TryParse(txt_Port.Text, out isNumber);

            if (txt_Port.Text == "" || !successfullyParsed)
                return false;

            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

            // 현재 사용중인 포트와 중복 확인
            foreach (TcpConnectionInformation tcpi in tcpConnInfoArray)
            {
                if (tcpi.LocalEndPoint.Port == int.Parse(txt_Port.Text))
                {
                    isNotInUse = false;
                    break;
                }
            }

            // 포트 범위 이외의 포트번호 확인
            if ((double.Parse(txt_Port.Text) < 65536 && int.Parse(txt_Port.Text) > 0) && isNotInUse)
                return true;
            else
                return false;
        }
        private void frm_Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (!isServer && isConnect)       // 클라이언트 상태 일때
                {
                    clsMSG.sHeader = SocketMSG.MESSAGE_TYPE.EXIT;
                    clsClient.OnSendData(clsMSG);
                }
                else
                    clsFileServer.SockClose();
            }
            catch (Exception ex) { }
        }

        private void txt_Message_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                if (txt_Message.Text.Length < 1)
                    return;
                btn_Send_Click(null, null);
            }
        }

        #region 델리게이트
        private delegate void BroadcastMsg();
        private void BroadCastMsg(string p_Sender, string p_Msg)
        {
            Invoke(new BroadcastMsg(delegate
            {
                List<string> lst_Connected = new List<string>();
                listBox1.Items.Add(p_Msg);

                foreach (string s1 in listBox1.Items)
                {
                    lst_Connected.Add(s1 + "/");
                }

                string s_Connectedlist = string.Empty;
                for (int i = 0; i < lst_Connected.Count; i++)
                {
                    s_Connectedlist = s_Connectedlist + lst_Connected[i];
                }
                clsMSG.sHeader = SocketMSG.MESSAGE_TYPE.SYSTEM;
                string s_DATETIME = "[" + DateTime.Now.ToString("yy/MM/dd HH:mm:ss") + "] ";
                clsMSG.sMsg = s_DATETIME + "(" + p_Msg + ")가 서버에 접속했습니다.";
                txt_TextBox.AppendText(Environment.NewLine + clsMSG.sMsg);

                clsServer.MessageSend(clsMSG);
                CheckConnectiList();
            }));
        }
        private delegate void ExitNode();
        private void ExitNodeMsg(string p_Sender, string p_Msg)
        {
            Invoke(new ExitNode(delegate
        {
            listBox1.Items.Remove(p_Sender);
            listBox1.Refresh();
            string sConnectedNode = string.Empty;
            for (int i = 1; i <= listBox1.Items.Count; i++)
            {
                if (listBox1.Items.Count == i)
                {
                    sConnectedNode += listBox1.Items[i - 1].ToString();
                }
                else
                    sConnectedNode += listBox1.Items[i - 1] + "/";
            }

            clsMSG.sHeader = SocketMSG.MESSAGE_TYPE.SYSTEM;
            string s_DATETIME = "[" + DateTime.Now.ToString("yy/MM/dd HH:mm:ss") + "] ";
            clsMSG.sMsg = s_DATETIME + "(" + p_Sender + ")가 종료 했습니다.";
            txt_TextBox.AppendText(Environment.NewLine + clsMSG.sMsg);

            clsServer.MessageSend(clsMSG);
            CheckConnectiList();
        }));
        }
        private delegate void AppendMsg();
        private void AppendNodeMsg(string p_Sender, string p_Msg)
        {
            Invoke(new AppendMsg(delegate
            {
                string[] arr_Msg = p_Msg.Split('\x01');
                txt_TextBox.AppendText(Environment.NewLine + p_Sender + " : " + p_Msg.Trim());
            }));
        }
        private delegate void AppendSYSTEM_MSG();
        private void SYSTEM_MSG(string p_Sender, string p_Msg)
        {
            Invoke(new AppendSYSTEM_MSG(delegate
            {
                clientIP = p_Sender;
                txt_TextBox.AppendText(Environment.NewLine + p_Msg);
            }));
        }
        /// <summary>
        /// 소켓 접속자 확인
        /// </summary>
        private delegate void Append_Node();
        private void CONNECTED_LIST(string p_Sender, string p_Msg)
        {
            myIP = p_Sender; // 자신 IP,Port
            Invoke(new Append_Node(delegate
            {
                if (listBox1.Items.Count > 0)
                    this.listBox1.Items.Clear();

                string[] lstConn = p_Msg.Split('/');
                for (int i = 0; i < lstConn.Length; i++)
                {
                    var cleaned = lstConn[i].Replace("\0", string.Empty);
                    listBox1.Items.Add(cleaned.Trim());
                }
            }));
        }
        #endregion

        private void btn_Send_Click(object sender, EventArgs e)
        {
            if (txt_Message.Text.Trim().Length < 1)
                return;

            if (listBox1.Items.Count < 1)
            {
                txt_TextBox.AppendText(Environment.NewLine + "서버와 연결되어 있지 않습니다.");
                txt_Message.Text = "";
                txt_Message.Focus();
                return;
            }
            clsMSG.sHeader = SocketMSG.MESSAGE_TYPE.MSG;
            clsMSG.sMsg = txt_Message.Text;

            // 메세지 전송
            try
            {
                if (isServer)     // 서버 상태
                    clsServer.MessageSend(clsMSG);
                else               // 클라이언트 상태
                    clsClient.OnSendData(clsMSG);

                // 보낸 메세지를 창에 표시
                txt_TextBox.AppendText(Environment.NewLine + "[나] : " + txt_Message.Text.Trim());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                txt_Message.Focus();
                txt_Message.Clear();
            }
        }

        private void btn_Disconnect_Click(object sender, EventArgs e)
        {
            // 접속 해제
            if (btn_Connect.Enabled == false)
            {
                try
                {
                    clsMSG.sHeader = SocketMSG.MESSAGE_TYPE.EXIT;
                    clsClient.OnSendData(clsMSG);
                    clsClient.CloseClient();
                    listBox1.Items.Clear();
                    txt_TextBox.Text += Environment.NewLine + "접속을 종료 했습니다.";
                    btn_Connect.Enabled = true;
                    btn_Create.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        // 쓰레드에서 Dialog를 사용하기위해 필요
        [STAThreadAttribute]
        private void btn_FileTransfer_Click(object sender, EventArgs e)
        {
            // 접속 되어 있지 않을 경우 취소
            if (listBox1.Items.Count == 0)
            {
                MessageBox.Show("서버와 연결되어 있지 않습니다.");
                return;
            }

            // 수신자가 선택되어 있지 않음
            if (listBox1.SelectedIndex < 0)
            {
                MessageBox.Show("파일을 전송할 유저를 선택해 주세요.");
                return;
            }

            string sIP = listBox1.SelectedItem.ToString().Substring(0, listBox1.SelectedItem.ToString().IndexOf(':')).Trim();
            string sPort = listBox1.SelectedItem.ToString().Substring(listBox1.SelectedItem.ToString().IndexOf(':') + 1).Trim();
            // 받는 사람이 호스트일 경우
            if (sPort.Contains(' '))
                sPort = sPort.Substring(0, sPort.IndexOf(' '));

            if (myIP.Equals(sIP + ":" + sPort))
            {
                MessageBox.Show("본인이 선택되었습니다.");
                return;
            }

            // 파일 전송 정보
            clsMSG.sHeader = SocketMSG.MESSAGE_TYPE.DATA;
            clsMSG.sReceiver = listBox1.SelectedItem.ToString();
            clsMSG.sReceiver = sIP + ":" + sPort;

            // 보낼 파일
            SockFileTransfer sockFileTransfer = new SockFileTransfer();
            var o_Packet = sockFileTransfer.FileSelect();

            // 선택된 파일이 없음
            if (o_Packet == null)
                return;

            // 파일 전송
            if (isServer)
            {   // 서버
                clsServer.FileSend(clsMSG, o_Packet);
            }
            else
            {   // 클라이언트
                MultiSockets_FileTransfer.FileSocketClient fileSocketClient = new MultiSockets_FileTransfer.FileSocketClient();
                clsClient.OnSendData(clsMSG, o_Packet);
                System.Threading.Thread.Sleep(50);
                fileSocketClient.ClientFileSend(clsMSG, o_Packet, txt_IPaddr.Text);
            }
        }

        private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && listBox1.Items.Count > 0)
            {
                mouse_event(L_down, uint.Parse(e.X.ToString()), uint.Parse(e.Y.ToString()), uint.Parse(e.Delta.ToString()), 0);
            }
            else
                return;

            listItemSelect(e);
        }

        private void listItemSelect(MouseEventArgs e)
        {
            if (listBox1.SelectedIndex < 0)
                return;

            string sSelectedIP = listBox1.SelectedItem.ToString().Trim();
            int iMenuCnt = Enum.GetNames(typeof(ContextMenus.CONTEXT_ITEM)).Length;
            if (sSelectedIP.Length > 0)
            {
                ContextMenu ctms = new ContextMenu();
                //메뉴 스트립 추가  
                Point mousePoint = new Point(e.X, e.Y);
                //마우스가 클릭된 위치 
                foreach (var i in Enum.GetNames(typeof(ContextMenus.CONTEXT_ITEM)))
                {
                    ctms.MenuItems.Add(i);
                }
                //아이템 추가 

                ctms.Show(listBox1, new Point(e.X, e.Y));
                //마우스 클릭된 위치에 스트립 메뉴를 보여준다 
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Text);
        }
    }
}

