using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WCFSampleTesing;

public class Config : Form
{
	private string SAPPLANT = "";

	private string LINENAME = "";

	private string GROUPNAME = "";

	private string CUSTCODE = "";

	private string PROCESS = "";

	private string FACTORYSN = "";

	private string MOUNTSIDE = "";

	private string EQUIPTYPE = "";

	private string OPEUSER = "";

	private string LOGFILEPATH = "";

	private string SENDMSG2SERVER = "";

	private string APPSTOP = "";

	private IContainer components;

	private BackgroundWorker backgroundWorker1;

	private Label label1;

	private Label label2;

	private Label label3;

	private Label label4;

	private Label label5;

	private TextBox textBoxSAPPLANT;

	private TextBox textBoxLINENAME;

	private TextBox textBoxGROUPNAME;

	private TextBox textBoxCUSTCODE;

	private TextBox textBoxPROCESS;

	private Label label6;

	private Label label7;

	private Label label8;

	private Label label9;

	private Label label10;

	private TextBox textBoxMOUNTSIDE;

	private TextBox textBoxEQUIPTYPE;

	private TextBox textBoxOPEUSER;

	private TextBox textBoxFACTORYSN;

	private TextBox textBoxLOGFILEPATH;

	private Button buttonModify;

	private GroupBox groupBox1;

	private Label label11;

	private TextBox textBoxSENDMSG;

	public Config()
	{
		InitializeComponent();
		base.FormClosing += Form1_FormClosing;
	}

	private void Config_Load(object sender, EventArgs e)
	{
		base.Location = Cursor.Position;
		SAPPLANT = AppConfig.Get("SAPPLANT");
		LINENAME = AppConfig.Get("LINENAME");
		GROUPNAME = AppConfig.Get("GROUPNAME");
		CUSTCODE = AppConfig.Get("CUSTCODE");
		PROCESS = AppConfig.Get("PROCESS");
		MOUNTSIDE = AppConfig.Get("MOUNTSIDE");
		EQUIPTYPE = AppConfig.Get("EQUIPTYPE");
		OPEUSER = AppConfig.Get("OPEUSER");
		LOGFILEPATH = AppConfig.Get("LOGFILEPATH");
		SENDMSG2SERVER = AppConfig.Get("SENDMSG2SERVER");
		FACTORYSN = AppConfig.Get("FACTORYSN");
		APPSTOP = AppConfig.Get("APPSTOP");
		textBoxSAPPLANT.Text = SAPPLANT;
		textBoxLINENAME.Text = LINENAME;
		textBoxGROUPNAME.Text = GROUPNAME;
		textBoxCUSTCODE.Text = CUSTCODE;
		textBoxPROCESS.Text = PROCESS;
		textBoxEQUIPTYPE.Text = EQUIPTYPE;
		textBoxMOUNTSIDE.Text = MOUNTSIDE;
		textBoxOPEUSER.Text = OPEUSER;
		textBoxLOGFILEPATH.Text = LOGFILEPATH;
		textBoxFACTORYSN.Text = FACTORYSN;
		textBoxSENDMSG.Text = SENDMSG2SERVER;
	}

	private void Form1_FormClosing(object sender, FormClosingEventArgs e)
	{
		AppConfig.Set("APPSTOP", "0");
	}

	private void buttonModify_Click(object sender, EventArgs e)
	{
		AppConfig.Set("SAPPLANT", textBoxSAPPLANT.Text);
		AppConfig.Set("LINENAME", textBoxLINENAME.Text);
		AppConfig.Set("GROUPNAME", textBoxGROUPNAME.Text);
		AppConfig.Set("CUSTCODE", textBoxCUSTCODE.Text);
		AppConfig.Set("PROCESS", textBoxPROCESS.Text);
		AppConfig.Set("MOUNTSIDE", textBoxMOUNTSIDE.Text);
		AppConfig.Set("EQUIPTYPE", textBoxEQUIPTYPE.Text);
		AppConfig.Set("OPEUSER", textBoxOPEUSER.Text);
		AppConfig.Set("LOGFILEPATH", textBoxLOGFILEPATH.Text);
		AppConfig.Set("FACTORYSN", textBoxFACTORYSN.Text);
		AppConfig.Set("SENDMSG2SERVER", textBoxSENDMSG.Text);
		AppConfig.Set("APPSTOP", "0");
		MessageBox.Show("Modify Parameters Successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
		Close();
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
		this.label1 = new System.Windows.Forms.Label();
		this.label2 = new System.Windows.Forms.Label();
		this.label3 = new System.Windows.Forms.Label();
		this.label4 = new System.Windows.Forms.Label();
		this.label5 = new System.Windows.Forms.Label();
		this.textBoxSAPPLANT = new System.Windows.Forms.TextBox();
		this.textBoxLINENAME = new System.Windows.Forms.TextBox();
		this.textBoxGROUPNAME = new System.Windows.Forms.TextBox();
		this.textBoxCUSTCODE = new System.Windows.Forms.TextBox();
		this.textBoxPROCESS = new System.Windows.Forms.TextBox();
		this.label6 = new System.Windows.Forms.Label();
		this.label7 = new System.Windows.Forms.Label();
		this.label8 = new System.Windows.Forms.Label();
		this.label9 = new System.Windows.Forms.Label();
		this.label10 = new System.Windows.Forms.Label();
		this.textBoxMOUNTSIDE = new System.Windows.Forms.TextBox();
		this.textBoxEQUIPTYPE = new System.Windows.Forms.TextBox();
		this.textBoxOPEUSER = new System.Windows.Forms.TextBox();
		this.textBoxFACTORYSN = new System.Windows.Forms.TextBox();
		this.textBoxLOGFILEPATH = new System.Windows.Forms.TextBox();
		this.buttonModify = new System.Windows.Forms.Button();
		this.groupBox1 = new System.Windows.Forms.GroupBox();
		this.label11 = new System.Windows.Forms.Label();
		this.textBoxSENDMSG = new System.Windows.Forms.TextBox();
		this.groupBox1.SuspendLayout();
		base.SuspendLayout();
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(18, 34);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(66, 13);
		this.label1.TabIndex = 0;
		this.label1.Text = "SAPPLANT:";
		this.label2.AutoSize = true;
		this.label2.Location = new System.Drawing.Point(18, 71);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(65, 13);
		this.label2.TabIndex = 1;
		this.label2.Text = "LINENAME:";
		this.label3.AutoSize = true;
		this.label3.Location = new System.Drawing.Point(6, 113);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(80, 13);
		this.label3.TabIndex = 2;
		this.label3.Text = "GROUPNAME:";
		this.label4.AutoSize = true;
		this.label4.Location = new System.Drawing.Point(14, 152);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(69, 13);
		this.label4.TabIndex = 3;
		this.label4.Text = "CUSTCODE:";
		this.label5.AutoSize = true;
		this.label5.Location = new System.Drawing.Point(22, 196);
		this.label5.Name = "label5";
		this.label5.Size = new System.Drawing.Size(61, 13);
		this.label5.TabIndex = 4;
		this.label5.Text = "PROCESS:";
		this.textBoxSAPPLANT.Location = new System.Drawing.Point(89, 27);
		this.textBoxSAPPLANT.Name = "textBoxSAPPLANT";
		this.textBoxSAPPLANT.Size = new System.Drawing.Size(158, 20);
		this.textBoxSAPPLANT.TabIndex = 5;
		this.textBoxLINENAME.Location = new System.Drawing.Point(89, 64);
		this.textBoxLINENAME.Name = "textBoxLINENAME";
		this.textBoxLINENAME.Size = new System.Drawing.Size(158, 20);
		this.textBoxLINENAME.TabIndex = 6;
		this.textBoxGROUPNAME.Location = new System.Drawing.Point(89, 106);
		this.textBoxGROUPNAME.Name = "textBoxGROUPNAME";
		this.textBoxGROUPNAME.Size = new System.Drawing.Size(158, 20);
		this.textBoxGROUPNAME.TabIndex = 7;
		this.textBoxCUSTCODE.Location = new System.Drawing.Point(89, 145);
		this.textBoxCUSTCODE.Name = "textBoxCUSTCODE";
		this.textBoxCUSTCODE.Size = new System.Drawing.Size(158, 20);
		this.textBoxCUSTCODE.TabIndex = 8;
		this.textBoxPROCESS.Location = new System.Drawing.Point(89, 189);
		this.textBoxPROCESS.Name = "textBoxPROCESS";
		this.textBoxPROCESS.Size = new System.Drawing.Size(158, 20);
		this.textBoxPROCESS.TabIndex = 9;
		this.label6.AutoSize = true;
		this.label6.Location = new System.Drawing.Point(282, 34);
		this.label6.Name = "label6";
		this.label6.Size = new System.Drawing.Size(75, 13);
		this.label6.TabIndex = 10;
		this.label6.Text = "MOUNTSIDE:";
		this.label7.AutoSize = true;
		this.label7.Location = new System.Drawing.Point(286, 70);
		this.label7.Name = "label7";
		this.label7.Size = new System.Drawing.Size(71, 13);
		this.label7.TabIndex = 11;
		this.label7.Text = "EQUIPTYPE:";
		this.label8.AutoSize = true;
		this.label8.Location = new System.Drawing.Point(295, 113);
		this.label8.Name = "label8";
		this.label8.Size = new System.Drawing.Size(62, 13);
		this.label8.TabIndex = 12;
		this.label8.Text = "OPEUSER:";
		this.label9.AutoSize = true;
		this.label9.Location = new System.Drawing.Point(282, 152);
		this.label9.Name = "label9";
		this.label9.Size = new System.Drawing.Size(75, 13);
		this.label9.TabIndex = 13;
		this.label9.Text = "FACTORYSN:";
		this.label10.AutoSize = true;
		this.label10.Location = new System.Drawing.Point(3, 235);
		this.label10.Name = "label10";
		this.label10.Size = new System.Drawing.Size(83, 13);
		this.label10.TabIndex = 14;
		this.label10.Text = "LOGFILEPATH:";
		this.textBoxMOUNTSIDE.Location = new System.Drawing.Point(364, 27);
		this.textBoxMOUNTSIDE.Name = "textBoxMOUNTSIDE";
		this.textBoxMOUNTSIDE.Size = new System.Drawing.Size(158, 20);
		this.textBoxMOUNTSIDE.TabIndex = 15;
		this.textBoxEQUIPTYPE.Location = new System.Drawing.Point(364, 68);
		this.textBoxEQUIPTYPE.Name = "textBoxEQUIPTYPE";
		this.textBoxEQUIPTYPE.Size = new System.Drawing.Size(158, 20);
		this.textBoxEQUIPTYPE.TabIndex = 16;
		this.textBoxOPEUSER.Location = new System.Drawing.Point(364, 106);
		this.textBoxOPEUSER.Name = "textBoxOPEUSER";
		this.textBoxOPEUSER.Size = new System.Drawing.Size(158, 20);
		this.textBoxOPEUSER.TabIndex = 17;
		this.textBoxFACTORYSN.Location = new System.Drawing.Point(364, 145);
		this.textBoxFACTORYSN.Name = "textBoxFACTORYSN";
		this.textBoxFACTORYSN.Size = new System.Drawing.Size(158, 20);
		this.textBoxFACTORYSN.TabIndex = 18;
		this.textBoxLOGFILEPATH.Location = new System.Drawing.Point(89, 228);
		this.textBoxLOGFILEPATH.Name = "textBoxLOGFILEPATH";
		this.textBoxLOGFILEPATH.Size = new System.Drawing.Size(433, 20);
		this.textBoxLOGFILEPATH.TabIndex = 19;
		this.buttonModify.Location = new System.Drawing.Point(426, 264);
		this.buttonModify.Name = "buttonModify";
		this.buttonModify.Size = new System.Drawing.Size(96, 35);
		this.buttonModify.TabIndex = 20;
		this.buttonModify.Text = "Modify";
		this.buttonModify.UseVisualStyleBackColor = true;
		this.buttonModify.Click += new System.EventHandler(buttonModify_Click);
		this.groupBox1.Controls.Add(this.textBoxSENDMSG);
		this.groupBox1.Controls.Add(this.label11);
		this.groupBox1.Controls.Add(this.buttonModify);
		this.groupBox1.Controls.Add(this.textBoxLOGFILEPATH);
		this.groupBox1.Controls.Add(this.textBoxFACTORYSN);
		this.groupBox1.Controls.Add(this.textBoxOPEUSER);
		this.groupBox1.Controls.Add(this.textBoxEQUIPTYPE);
		this.groupBox1.Controls.Add(this.textBoxMOUNTSIDE);
		this.groupBox1.Controls.Add(this.label10);
		this.groupBox1.Controls.Add(this.label9);
		this.groupBox1.Controls.Add(this.label8);
		this.groupBox1.Controls.Add(this.label7);
		this.groupBox1.Controls.Add(this.label6);
		this.groupBox1.Controls.Add(this.textBoxPROCESS);
		this.groupBox1.Controls.Add(this.textBoxCUSTCODE);
		this.groupBox1.Controls.Add(this.textBoxGROUPNAME);
		this.groupBox1.Controls.Add(this.textBoxLINENAME);
		this.groupBox1.Controls.Add(this.textBoxSAPPLANT);
		this.groupBox1.Controls.Add(this.label5);
		this.groupBox1.Controls.Add(this.label4);
		this.groupBox1.Controls.Add(this.label3);
		this.groupBox1.Controls.Add(this.label2);
		this.groupBox1.Controls.Add(this.label1);
		this.groupBox1.Location = new System.Drawing.Point(12, 14);
		this.groupBox1.Name = "groupBox1";
		this.groupBox1.Size = new System.Drawing.Size(539, 316);
		this.groupBox1.TabIndex = 21;
		this.groupBox1.TabStop = false;
		this.groupBox1.Text = "Setting";
		this.label11.AutoSize = true;
		this.label11.Location = new System.Drawing.Point(253, 196);
		this.label11.Name = "label11";
		this.label11.Size = new System.Drawing.Size(105, 13);
		this.label11.TabIndex = 21;
		this.label11.Text = "SEND TO SERVER:";
		this.textBoxSENDMSG.Location = new System.Drawing.Point(364, 189);
		this.textBoxSENDMSG.Name = "textBoxSENDMSG";
		this.textBoxSENDMSG.Size = new System.Drawing.Size(158, 20);
		this.textBoxSENDMSG.TabIndex = 22;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(561, 342);
		base.Controls.Add(this.groupBox1);
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "Config";
		this.Text = "Config";
		base.Load += new System.EventHandler(Config_Load);
		this.groupBox1.ResumeLayout(false);
		this.groupBox1.PerformLayout();
		base.ResumeLayout(false);
	}
}
