namespace SocketChatandFile
{
    partial class frm_Main
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_Connect = new System.Windows.Forms.Button();
            this.btn_Create = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_Port = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_IPaddr = new System.Windows.Forms.TextBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btn_FileTransfer = new System.Windows.Forms.Button();
            this.btn_Disconnect = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.txt_TextBox = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.txt_Message = new System.Windows.Forms.TextBox();
            this.btn_Send = new System.Windows.Forms.Button();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_Connect
            // 
            this.btn_Connect.Font = new System.Drawing.Font("210 앱굴림 R", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btn_Connect.Location = new System.Drawing.Point(314, 3);
            this.btn_Connect.Name = "btn_Connect";
            this.btn_Connect.Size = new System.Drawing.Size(105, 48);
            this.btn_Connect.TabIndex = 5;
            this.btn_Connect.Text = "서버 연결 \r\n(클라이언트)";
            this.btn_Connect.UseVisualStyleBackColor = true;
            this.btn_Connect.Click += new System.EventHandler(this.btn_Connect_Click);
            // 
            // btn_Create
            // 
            this.btn_Create.Font = new System.Drawing.Font("210 앱굴림 R", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btn_Create.Location = new System.Drawing.Point(231, 3);
            this.btn_Create.Name = "btn_Create";
            this.btn_Create.Size = new System.Drawing.Size(77, 48);
            this.btn_Create.TabIndex = 4;
            this.btn_Create.Text = "채팅 생성";
            this.btn_Create.UseVisualStyleBackColor = true;
            this.btn_Create.Click += new System.EventHandler(this.btn_Create_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("210 앱굴림 R", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(12, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "PORT";
            // 
            // txt_Port
            // 
            this.txt_Port.Location = new System.Drawing.Point(81, 31);
            this.txt_Port.Name = "txt_Port";
            this.txt_Port.Size = new System.Drawing.Size(132, 20);
            this.txt_Port.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("210 앱굴림 R", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(12, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "SERVER";
            // 
            // txt_IPaddr
            // 
            this.txt_IPaddr.Location = new System.Drawing.Point(81, 5);
            this.txt_IPaddr.Name = "txt_IPaddr";
            this.txt_IPaddr.Size = new System.Drawing.Size(132, 20);
            this.txt_IPaddr.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.txt_IPaddr);
            this.panel3.Controls.Add(this.txt_Port);
            this.panel3.Controls.Add(this.btn_FileTransfer);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.btn_Disconnect);
            this.panel3.Controls.Add(this.btn_Create);
            this.panel3.Controls.Add(this.btn_Connect);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(659, 61);
            this.panel3.TabIndex = 1;
            // 
            // btn_FileTransfer
            // 
            this.btn_FileTransfer.Font = new System.Drawing.Font("210 앱굴림 R", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btn_FileTransfer.Location = new System.Drawing.Point(579, 3);
            this.btn_FileTransfer.Name = "btn_FileTransfer";
            this.btn_FileTransfer.Size = new System.Drawing.Size(77, 48);
            this.btn_FileTransfer.TabIndex = 5;
            this.btn_FileTransfer.Text = "파일 전송";
            this.btn_FileTransfer.UseVisualStyleBackColor = true;
            this.btn_FileTransfer.Click += new System.EventHandler(this.btn_FileTransfer_Click);
            // 
            // btn_Disconnect
            // 
            this.btn_Disconnect.Font = new System.Drawing.Font("210 앱굴림 R", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btn_Disconnect.Location = new System.Drawing.Point(425, 3);
            this.btn_Disconnect.Name = "btn_Disconnect";
            this.btn_Disconnect.Size = new System.Drawing.Size(105, 48);
            this.btn_Disconnect.TabIndex = 5;
            this.btn_Disconnect.Text = "연결 해제\r\n(클라이언트)";
            this.btn_Disconnect.UseVisualStyleBackColor = true;
            this.btn_Disconnect.Click += new System.EventHandler(this.btn_Disconnect_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.Color.White;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 61);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listBox1);
            this.splitContainer1.Panel1MinSize = 215;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel1);
            this.splitContainer1.Size = new System.Drawing.Size(659, 408);
            this.splitContainer1.SplitterDistance = 217;
            this.splitContainer1.TabIndex = 2;
            // 
            // listBox1
            // 
            this.listBox1.BackColor = System.Drawing.Color.White;
            this.listBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.Font = new System.Drawing.Font("210 앱굴림 R", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 16;
            this.listBox1.Location = new System.Drawing.Point(0, 0);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(217, 408);
            this.listBox1.TabIndex = 4;
            this.listBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listBox1_MouseDown);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.txt_TextBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(438, 408);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // txt_TextBox
            // 
            this.txt_TextBox.BackColor = System.Drawing.Color.White;
            this.txt_TextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_TextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txt_TextBox.Font = new System.Drawing.Font("210 앱굴림 R", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.txt_TextBox.ForeColor = System.Drawing.Color.Black;
            this.txt_TextBox.Location = new System.Drawing.Point(3, 3);
            this.txt_TextBox.Multiline = true;
            this.txt_TextBox.Name = "txt_TextBox";
            this.txt_TextBox.ReadOnly = true;
            this.txt_TextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_TextBox.Size = new System.Drawing.Size(432, 347);
            this.txt_TextBox.TabIndex = 0;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 103F));
            this.tableLayoutPanel3.Controls.Add(this.txt_Message, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.btn_Send, 1, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 353);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(438, 55);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // txt_Message
            // 
            this.txt_Message.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_Message.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txt_Message.Font = new System.Drawing.Font("210 앱굴림 R", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.txt_Message.Location = new System.Drawing.Point(3, 3);
            this.txt_Message.Multiline = true;
            this.txt_Message.Name = "txt_Message";
            this.txt_Message.Size = new System.Drawing.Size(329, 49);
            this.txt_Message.TabIndex = 0;
            this.txt_Message.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_Message_KeyPress);
            // 
            // btn_Send
            // 
            this.btn_Send.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_Send.Font = new System.Drawing.Font("210 앱굴림 R", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btn_Send.Location = new System.Drawing.Point(338, 3);
            this.btn_Send.Name = "btn_Send";
            this.btn_Send.Size = new System.Drawing.Size(97, 49);
            this.btn_Send.TabIndex = 1;
            this.btn_Send.Text = "전송";
            this.btn_Send.UseVisualStyleBackColor = true;
            this.btn_Send.Click += new System.EventHandler(this.btn_Send_Click);
            // 
            // frm_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(659, 469);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel3);
            this.MinimumSize = new System.Drawing.Size(675, 508);
            this.Name = "frm_Main";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frm_Main_FormClosed);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txt_IPaddr;
        private System.Windows.Forms.Button btn_Connect;
        private System.Windows.Forms.Button btn_Create;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_Port;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox txt_TextBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TextBox txt_Message;
        private System.Windows.Forms.Button btn_Send;
        private System.Windows.Forms.Button btn_Disconnect;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button btn_FileTransfer;
    }
}

