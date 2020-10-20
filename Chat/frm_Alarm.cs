using MultiSockets;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;
using static MultiSockets.SockFileTransfer;

namespace SocketChatandFile
{
    public partial class frm_Alarm : Form
    {
        public frm_Alarm(string p_sender, string p_fileName)
        {
            InitializeComponent();
            this.TopMost = true;
            this.StartPosition = FormStartPosition.CenterScreen;

            string sMsg = string.Empty;
            sMsg = " " + p_sender + " 님이 파일을 전송 " + Environment.NewLine +
                   " 했습니다." + Environment.NewLine + " 수신 하시겠습니까?";
            textBox1.Text = sMsg;
            textBox2.Text = " " + p_fileName;
        }

        private bool AcceptFile()
        {
            SaveFileDialog savePanel = new SaveFileDialog();
            savePanel.InitialDirectory = System.Windows.Forms.Application.StartupPath;
            savePanel.FileName = ReceivedFile._FileName;

            if (savePanel.ShowDialog() == DialogResult.OK)
            {
                FileStream fileStr = new FileStream(savePanel.FileName, FileMode.Create, FileAccess.Write);
                BinaryWriter writer = new BinaryWriter(fileStr);
                writer.Write(ReceivedFile._Data, 0, ReceivedFile._Size);
                fileStr.Close();

                return true;
            }
            else
            {
                MessageBox.Show("취소되었습니다.");
                return false;
            }
        }
        private void btn_Accept_Click(object sender, EventArgs e)
        {
            bool isDone = AcceptFile();
            if (isDone)
            {
                this.Close();
            }
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
