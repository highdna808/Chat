using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiSockets
{
    /// <summary>
    /// 수신 메세지
    /// </summary>
    public class SocketMSG
    {
        public enum MESSAGE_TYPE { SYSTEM, MSG, CONNECT_LIST, EXIT, DATA };
        public MESSAGE_TYPE sHeader { get; set; }
        public string sSender { get; set; }
        public string sReceiver { get; set; }
        public string sMsg { get; set; }
    }
    /// <summary>
    /// 리스트 우클릭 컨텍스트 메뉴
    /// </summary>
    public class ContextMenus
    {
        public enum CONTEXT_ITEM { 파일전송, 화면공유 };
    }

    public class StoredPackage
    {
        private static List<STORED_DATA_INFO> lstPackages = new List<STORED_DATA_INFO>();

        public StoredPackage(string sSender, string sReceiver, string sFileName, int iFileLength)
        {
            this.Add_StoreDataInfo(sSender, sReceiver, sFileName, iFileLength);
        }
        public class STORED_DATA_INFO
        {
            public string SENDER = string.Empty;
            public string RECEIVER = string.Empty;
            public string FILE_NAME = string.Empty;
            public int FILE_LENGTH = 0;
        }

        public void Add_StoreDataInfo(string sSender, string sReceiver, string sFileName, int iFileLength)
        {
            STORED_DATA_INFO o_StoreData = new STORED_DATA_INFO();
            o_StoreData.SENDER = sSender;
            o_StoreData.RECEIVER = sReceiver;
            o_StoreData.FILE_NAME = sFileName;
            o_StoreData.FILE_LENGTH = iFileLength;
            lstPackages.Add(o_StoreData);
        }
    }
    /// <summary>
    /// 수신 파일 정보
    /// </summary>
    public static class ReceivedFile
    {
        public static string _sender { get; set; }
        public static string _Receiver { get; set; }
        public static string _FileName { get; set; }
        public static byte[] _Data { get; set; }
        public static int _Size { get; set; }
        public static int _RemainPacket { get; set; }
        public static int _PacketCnt { get; set; }

        public static void ClearInfo()
        {
            _FileName = string.Empty;
            Array.Clear(_Data, 0, _Data.Length);
            _Size = 0;
            _PacketCnt = 0;
        }
    }

    /// <summary>
    /// 송신 파일 정보
    /// </summary>
    public class SockFileTransfer
    {
        [Serializable]
        public class DataPacket
        {
            public Dictionary<int, byte[]> Data
            { get; private set; }
            public string FileName
            { get; private set; }
            public int Size
            { get; private set; }
            public int PacketCnt
            { get; private set; }
            public int RemainPacket
            { get; private set; }

            public DataPacket(string filename, int pFilelength, int pPacketCnt, int pPacketRemain, Dictionary<int, byte[]> buf)
            {
                FileName = filename;
                Data = buf;
                PacketCnt = pPacketCnt;
                Size = pFilelength;
                RemainPacket = pPacketRemain;
            }

            public void DataPacketClear()
            {
                FileName = string.Empty;
                Data = null;
                PacketCnt = 0;
                Size = 0;
                RemainPacket = 0;
            }
        }

        /// <summary>
        /// 전송할 파일 선택, 파일을 패킷화
        /// </summary>
        /// <returns></returns>
        public DataPacket FileSelect()
        {
            OpenFileDialog oOfd = new OpenFileDialog();
            try
            {
                if (oOfd.ShowDialog() == DialogResult.OK) ;
                {
                    string sFilepath = oOfd.FileName;
                    string sFilename = oOfd.SafeFileName;
                    FileStream fs = new FileStream(sFilepath, FileMode.Open, FileAccess.Read);
                    int iFileLength = (int)fs.Length;
                    int iPacketCnt = 0;
                    var buffData = new Dictionary<int, byte[]>();
                    byte[] buffer = BitConverter.GetBytes(iFileLength);
                    BinaryReader reader = new BinaryReader(fs);
                    //reader.ReadBytes(1024);
                    // 헤더 4바이트 + 데이터 1024바이트
                    int iRemainPacket = iFileLength % 1024;
                    if (iRemainPacket > 0)
                        iPacketCnt = (iFileLength / 1024) + 1;
                    else
                        iPacketCnt = (iFileLength / 1024);

                    for (int i = 0; i < iPacketCnt; i++)
                    {
                        if (iPacketCnt - 1 == i)
                        {   // 데이터 (나머지)
                            buffData.Add(i, reader.ReadBytes(iRemainPacket));
                        }
                        else
                        {   // 데이터 (1024)
                            buffData.Add(i, reader.ReadBytes(1024));
                        }
                    }
                    fs.Close();

                    DataPacket oPacket = new DataPacket(sFilename, iFileLength, iPacketCnt, iRemainPacket, buffData);
                    return oPacket;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 서버에서 수신 후 재송신용
        /// </summary>
        /// <param name="pFileName"></param>
        /// <returns></returns>
        public DataPacket FileSelect(string pFileName)
        {
            try
            {
                string sFilepath = System.Windows.Forms.Application.StartupPath;
                string sFilename = sFilepath + "\\" + pFileName;
                FileStream fs = new FileStream(sFilename, FileMode.Open, FileAccess.Read);
                int iFileLength = (int)fs.Length;
                int iPacketCnt = 0;
                var buffData = new Dictionary<int, byte[]>();
                byte[] buffer = BitConverter.GetBytes(iFileLength);
                BinaryReader reader = new BinaryReader(fs);
                //reader.ReadBytes(1024);
                // 헤더 4바이트 + 데이터 1024바이트
                int iRemainPacket = iFileLength % 1024;
                if (iRemainPacket > 0)
                    iPacketCnt = (iFileLength / 1024) + 1;
                else
                    iPacketCnt = (iFileLength / 1024);

                for (int i = 0; i < iPacketCnt; i++)
                {
                    if (iPacketCnt - 1 == i)
                    {   // 데이터 (나머지)
                        buffData.Add(i, reader.ReadBytes(iRemainPacket));
                    }
                    else
                    {   // 데이터 (1024)
                        buffData.Add(i, reader.ReadBytes(1024));
                    }
                }
                fs.Close();

                DataPacket oPacket = new DataPacket(pFileName, iFileLength, iPacketCnt, iRemainPacket, buffData);
                return oPacket;
            }
            catch (Exception ex)
            {
                MessageBox.Show("파일 선택에 실패했습니다." + "\n" + ex.Message);
                return null;
            }
        }
    }
}