namespace ToolKit.DecryptTool;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        btnSelectFolder = new System.Windows.Forms.Button();
        txtSourceFolder = new System.Windows.Forms.TextBox();
        label1 = new System.Windows.Forms.Label();
        txtFilter = new System.Windows.Forms.TextBox();
        tvFiles = new System.Windows.Forms.TreeView();
        label2 = new System.Windows.Forms.Label();
        txtProjectName = new System.Windows.Forms.TextBox();
        rtbLog = new System.Windows.Forms.RichTextBox();
        groupBox1 = new System.Windows.Forms.GroupBox();
        groupBox2 = new System.Windows.Forms.GroupBox();
        btnGenerate = new System.Windows.Forms.Button();
        groupBox3 = new System.Windows.Forms.GroupBox();
        btnRestore = new System.Windows.Forms.Button();
        tt = new System.Windows.Forms.TextBox();
        btnSelectDll = new System.Windows.Forms.Button();
        folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
        groupBox4 = new System.Windows.Forms.GroupBox();
        groupBox5 = new System.Windows.Forms.GroupBox();
        btnFormater = new System.Windows.Forms.Button();
        btnBak = new System.Windows.Forms.Button();
        txtFormater = new System.Windows.Forms.TextBox();
        btnLoadFormater = new System.Windows.Forms.Button();
        txtBak = new System.Windows.Forms.TextBox();
        btnLoadBak = new System.Windows.Forms.Button();
        groupBox1.SuspendLayout();
        groupBox2.SuspendLayout();
        groupBox3.SuspendLayout();
        groupBox4.SuspendLayout();
        groupBox5.SuspendLayout();
        SuspendLayout();
        // 
        // btnSelectFolder
        // 
        btnSelectFolder.Location = new System.Drawing.Point(6, 25);
        btnSelectFolder.Name = "btnSelectFolder";
        btnSelectFolder.Size = new System.Drawing.Size(130, 26);
        btnSelectFolder.TabIndex = 0;
        btnSelectFolder.Text = "选择源文件夹...";
        btnSelectFolder.UseVisualStyleBackColor = true;
        btnSelectFolder.Click += btnSelectFolder_Click;
        // 
        // txtSourceFolder
        // 
        txtSourceFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
        txtSourceFolder.Location = new System.Drawing.Point(163, 25);
        txtSourceFolder.Name = "txtSourceFolder";
        txtSourceFolder.ReadOnly = true;
        txtSourceFolder.Size = new System.Drawing.Size(234, 23);
        txtSourceFolder.TabIndex = 1;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new System.Drawing.Point(6, 61);
        label1.Name = "label1";
        label1.Size = new System.Drawing.Size(151, 17);
        label1.TabIndex = 2;
        label1.Text = "过滤条件 (例如: *.txt;*.jpg)";
        // 
        // txtFilter
        // 
        txtFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
        txtFilter.Location = new System.Drawing.Point(163, 58);
        txtFilter.Name = "txtFilter";
        txtFilter.Size = new System.Drawing.Size(234, 23);
        txtFilter.TabIndex = 3;
        txtFilter.TextChanged += txtFilter_TextChanged;
        // 
        // tvFiles
        // 
        tvFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
        tvFiles.CheckBoxes = true;
        tvFiles.Location = new System.Drawing.Point(6, 25);
        tvFiles.Name = "tvFiles";
        tvFiles.Size = new System.Drawing.Size(565, 333);
        tvFiles.TabIndex = 4;
        tvFiles.AfterCheck += tvFiles_AfterCheck;
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Location = new System.Drawing.Point(6, 94);
        label2.Name = "label2";
        label2.Size = new System.Drawing.Size(106, 17);
        label2.TabIndex = 5;
        label2.Text = "输出项目/DLL名称";
        // 
        // txtProjectName
        // 
        txtProjectName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
        txtProjectName.Location = new System.Drawing.Point(163, 91);
        txtProjectName.Name = "txtProjectName";
        txtProjectName.ReadOnly = true;
        txtProjectName.Size = new System.Drawing.Size(234, 23);
        txtProjectName.TabIndex = 6;
        // 
        // rtbLog
        // 
        rtbLog.Dock = System.Windows.Forms.DockStyle.Fill;
        rtbLog.Location = new System.Drawing.Point(3, 19);
        rtbLog.Name = "rtbLog";
        rtbLog.ReadOnly = true;
        rtbLog.Size = new System.Drawing.Size(978, 243);
        rtbLog.TabIndex = 8;
        rtbLog.Text = "";
        // 
        // groupBox1
        // 
        groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
        groupBox1.Controls.Add(btnSelectFolder);
        groupBox1.Controls.Add(txtSourceFolder);
        groupBox1.Controls.Add(label1);
        groupBox1.Controls.Add(txtFilter);
        groupBox1.Controls.Add(label2);
        groupBox1.Controls.Add(txtProjectName);
        groupBox1.Location = new System.Drawing.Point(12, 14);
        groupBox1.Name = "groupBox1";
        groupBox1.Size = new System.Drawing.Size(403, 133);
        groupBox1.TabIndex = 10;
        groupBox1.TabStop = false;
        groupBox1.Text = "1. 设置";
        // 
        // groupBox2
        // 
        groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
        groupBox2.Controls.Add(btnGenerate);
        groupBox2.Controls.Add(tvFiles);
        groupBox2.Location = new System.Drawing.Point(419, 14);
        groupBox2.Name = "groupBox2";
        groupBox2.Size = new System.Drawing.Size(577, 417);
        groupBox2.TabIndex = 11;
        groupBox2.TabStop = false;
        groupBox2.Text = "2. 选择要包含的资源";
        // 
        // btnGenerate
        // 
        btnGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
        btnGenerate.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
        btnGenerate.Location = new System.Drawing.Point(6, 363);
        btnGenerate.Name = "btnGenerate";
        btnGenerate.Size = new System.Drawing.Size(565, 45);
        btnGenerate.TabIndex = 8;
        btnGenerate.Text = "生成资源 DLL";
        btnGenerate.UseVisualStyleBackColor = true;
        btnGenerate.Click += btnGenerate_Click;
        // 
        // groupBox3
        // 
        groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
        groupBox3.Controls.Add(btnRestore);
        groupBox3.Controls.Add(tt);
        groupBox3.Controls.Add(btnSelectDll);
        groupBox3.Location = new System.Drawing.Point(12, 127);
        groupBox3.Name = "groupBox3";
        groupBox3.Size = new System.Drawing.Size(403, 87);
        groupBox3.TabIndex = 12;
        groupBox3.TabStop = false;
        groupBox3.Text = "3. 从 DLL 还原文件";
        // 
        // btnRestore
        // 
        btnRestore.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right));
        btnRestore.Location = new System.Drawing.Point(316, 54);
        btnRestore.Name = "btnRestore";
        btnRestore.Size = new System.Drawing.Size(81, 23);
        btnRestore.TabIndex = 2;
        btnRestore.Text = "开始还原...";
        btnRestore.UseVisualStyleBackColor = true;
        btnRestore.Click += btnRestore_Click;
        // 
        // tt
        // 
        tt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
        tt.Location = new System.Drawing.Point(6, 22);
        tt.Multiline = true;
        tt.Name = "tt";
        tt.ReadOnly = true;
        tt.Size = new System.Drawing.Size(293, 55);
        tt.TabIndex = 1;
        // 
        // btnSelectDll
        // 
        btnSelectDll.Location = new System.Drawing.Point(316, 22);
        btnSelectDll.Name = "btnSelectDll";
        btnSelectDll.Size = new System.Drawing.Size(81, 23);
        btnSelectDll.TabIndex = 0;
        btnSelectDll.Text = "选择 DLL...";
        btnSelectDll.UseVisualStyleBackColor = true;
        btnSelectDll.Click += btnSelectDll_Click;
        // 
        // groupBox4
        // 
        groupBox4.Controls.Add(rtbLog);
        groupBox4.Location = new System.Drawing.Point(12, 465);
        groupBox4.Name = "groupBox4";
        groupBox4.Size = new System.Drawing.Size(984, 265);
        groupBox4.TabIndex = 13;
        groupBox4.TabStop = false;
        groupBox4.Text = "日志输出";
        // 
        // groupBox5
        // 
        groupBox5.Controls.Add(btnFormater);
        groupBox5.Controls.Add(btnBak);
        groupBox5.Controls.Add(txtFormater);
        groupBox5.Controls.Add(btnLoadFormater);
        groupBox5.Controls.Add(txtBak);
        groupBox5.Controls.Add(btnLoadBak);
        groupBox5.Location = new System.Drawing.Point(12, 248);
        groupBox5.Name = "groupBox5";
        groupBox5.Size = new System.Drawing.Size(402, 211);
        groupBox5.TabIndex = 14;
        groupBox5.TabStop = false;
        groupBox5.Text = "4. 代码备份和格式化";
        // 
        // btnFormater
        // 
        btnFormater.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
        btnFormater.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
        btnFormater.Location = new System.Drawing.Point(6, 153);
        btnFormater.Name = "btnFormater";
        btnFormater.Size = new System.Drawing.Size(390, 45);
        btnFormater.TabIndex = 11;
        btnFormater.Text = "格式化";
        btnFormater.UseVisualStyleBackColor = true;
        btnFormater.Click += btnFormater_Click;
        // 
        // btnBak
        // 
        btnBak.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
        btnBak.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
        btnBak.Location = new System.Drawing.Point(6, 102);
        btnBak.Name = "btnBak";
        btnBak.Size = new System.Drawing.Size(390, 45);
        btnBak.TabIndex = 10;
        btnBak.Text = "备份";
        btnBak.UseVisualStyleBackColor = true;
        btnBak.Click += btnBak_Click;
        // 
        // txtFormater
        // 
        txtFormater.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
        txtFormater.Location = new System.Drawing.Point(163, 61);
        txtFormater.Name = "txtFormater";
        txtFormater.ReadOnly = true;
        txtFormater.Size = new System.Drawing.Size(233, 23);
        txtFormater.TabIndex = 9;
        // 
        // btnLoadFormater
        // 
        btnLoadFormater.Location = new System.Drawing.Point(6, 61);
        btnLoadFormater.Name = "btnLoadFormater";
        btnLoadFormater.Size = new System.Drawing.Size(130, 26);
        btnLoadFormater.TabIndex = 8;
        btnLoadFormater.Text = "选择格式化文件夹";
        btnLoadFormater.UseVisualStyleBackColor = true;
        btnLoadFormater.Click += btnLoad_Click;
        // 
        // txtBak
        // 
        txtBak.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
        txtBak.Location = new System.Drawing.Point(163, 22);
        txtBak.Name = "txtBak";
        txtBak.ReadOnly = true;
        txtBak.Size = new System.Drawing.Size(233, 23);
        txtBak.TabIndex = 7;
        // 
        // btnLoadBak
        // 
        btnLoadBak.Location = new System.Drawing.Point(6, 22);
        btnLoadBak.Name = "btnLoadBak";
        btnLoadBak.Size = new System.Drawing.Size(130, 26);
        btnLoadBak.TabIndex = 1;
        btnLoadBak.Text = "选择备份文件夹";
        btnLoadBak.UseVisualStyleBackColor = true;
        btnLoadBak.Click += btnLoad_Click;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(1008, 733);
        Controls.Add(groupBox5);
        Controls.Add(groupBox4);
        Controls.Add(groupBox2);
        Controls.Add(groupBox1);
        Controls.Add(groupBox3);
        MinimumSize = new System.Drawing.Size(600, 675);
        Text = "资源DLL生成工具 (.NET 8 + NLog)";
        groupBox1.ResumeLayout(false);
        groupBox1.PerformLayout();
        groupBox2.ResumeLayout(false);
        groupBox3.ResumeLayout(false);
        groupBox3.PerformLayout();
        groupBox4.ResumeLayout(false);
        groupBox5.ResumeLayout(false);
        groupBox5.PerformLayout();
        ResumeLayout(false);
    }

    private System.Windows.Forms.Button btnBak;
    private System.Windows.Forms.Button btnFormater;

    private System.Windows.Forms.Button btnLoadBak;
    private System.Windows.Forms.TextBox txtBak;
    private System.Windows.Forms.TextBox txtFormater;
    private System.Windows.Forms.Button btnLoadFormater;

    private System.Windows.Forms.GroupBox groupBox5;

    private System.Windows.Forms.GroupBox groupBox4;

    #endregion

    private System.Windows.Forms.Button btnSelectFolder;
    private System.Windows.Forms.TextBox txtSourceFolder;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox txtFilter;
    private System.Windows.Forms.TreeView tvFiles;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox txtProjectName;
    private System.Windows.Forms.Button btnGenerate;
    private System.Windows.Forms.RichTextBox rtbLog;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.GroupBox groupBox3;
    private System.Windows.Forms.Button btnSelectDll;
    private System.Windows.Forms.TextBox tt;
    private System.Windows.Forms.Button btnRestore;
    private FolderBrowserDialog folderBrowserDialog1;
}