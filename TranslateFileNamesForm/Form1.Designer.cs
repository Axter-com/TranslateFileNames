using System.Windows.Forms;
using BrightIdeasSoftware;
using static System.Windows.Forms.ListView;
using static TranslateFilenamesCore.TranslateFilenames;
using FileDetails = TranslateFilenamesCore.TranslateFilenames.FileDetails;

namespace TranslateFileNamesForm
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.AddFiles = new System.Windows.Forms.Button();
            this.ClearList = new System.Windows.Forms.Button();
            this.TestButton = new System.Windows.Forms.Button();
            this.RenameAll = new System.Windows.Forms.Button();
            this.RenameSelected = new System.Windows.Forms.Button();
            this.RenameNotSelected = new System.Windows.Forms.Button();
            this.pBar1 = new System.Windows.Forms.ProgressBar();
            this.fastObjListView1 = new BrightIdeasSoftware.FastObjectListView();
            this.olv_FilenamesOriginal = new BrightIdeasSoftware.OLVColumn();
            this.olv_NewFilenames = new BrightIdeasSoftware.OLVColumn();
            this.olv_SrcLanguage = new BrightIdeasSoftware.OLVColumn();
            this.olv_FilePath = new BrightIdeasSoftware.OLVColumn();
            this.olv_FileExt = new BrightIdeasSoftware.OLVColumn();
            this.labelFolder = new System.Windows.Forms.Label();
            this.TargetedPath = new System.Windows.Forms.TextBox();
            this.FilterText = new System.Windows.Forms.TextBox();
            this.labelFilter = new System.Windows.Forms.Label();
            this.RenameUnchecked = new System.Windows.Forms.Button();
            this.RenameChecked = new System.Windows.Forms.Button();
            this.labelTargetLang = new System.Windows.Forms.Label();
            this.TargetLanguage = new System.Windows.Forms.TextBox();
            this.labelSourceLanguage = new System.Windows.Forms.Label();
            this.SourceLanguage = new System.Windows.Forms.TextBox();
            this.checkBoxRecursive = new System.Windows.Forms.CheckBox();
            this.checkBoxLongPathSupport = new System.Windows.Forms.CheckBox();
            this.labelMaxThread = new System.Windows.Forms.Label();
            this.MaxThread = new System.Windows.Forms.TextBox();
            this.labelMaxTranslateLen = new System.Windows.Forms.Label();
            this.MaxTranslateLen = new System.Windows.Forms.TextBox();
            this.checkBoxAppendLanguageName = new System.Windows.Forms.CheckBox();
            this.checkBoxAppendOrginalName = new System.Windows.Forms.CheckBox();
            this.labelFileType = new System.Windows.Forms.Label();
            this.FileType = new System.Windows.Forms.TextBox();
            this.comboBoxFilesPerTransReq = new System.Windows.Forms.ComboBox();
            this.labelFilesPerTransReq = new System.Windows.Forms.Label();
            this.RefreshList = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.fastObjListView1)).BeginInit();
            this.SuspendLayout();
            // 
            // AddFiles
            // 
            this.AddFiles.Location = new System.Drawing.Point(437, 3);
            this.AddFiles.Name = "AddFiles";
            this.AddFiles.Size = new System.Drawing.Size(43, 34);
            this.AddFiles.TabIndex = 1;
            this.AddFiles.Text = "...";
            this.AddFiles.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.AddFiles.UseVisualStyleBackColor = true;
            this.AddFiles.Click += new System.EventHandler(this.AddFiles_Click);
            // 
            // ClearList
            // 
            this.ClearList.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.ClearList.Location = new System.Drawing.Point(484, 3);
            this.ClearList.Name = "ClearList";
            this.ClearList.Size = new System.Drawing.Size(64, 34);
            this.ClearList.TabIndex = 9;
            this.ClearList.Text = "Clear";
            this.ClearList.UseVisualStyleBackColor = true;
            this.ClearList.Click += new System.EventHandler(this.ClearList_Click);
            // 
            // TestButton
            // 
            this.TestButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.TestButton.Location = new System.Drawing.Point(2, 500);
            this.TestButton.Name = "TestButton";
            this.TestButton.Size = new System.Drawing.Size(60, 34);
            this.TestButton.TabIndex = 99;
            this.TestButton.TabStop = false;
            this.TestButton.Text = "Test";
            this.TestButton.UseVisualStyleBackColor = true;
            this.TestButton.Click += new System.EventHandler(this.TestButton_Click);
            // 
            // RenameAll
            // 
            this.RenameAll.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.RenameAll.Location = new System.Drawing.Point(68, 500);
            this.RenameAll.Name = "RenameAll";
            this.RenameAll.Size = new System.Drawing.Size(112, 34);
            this.RenameAll.TabIndex = 4;
            this.RenameAll.Text = "Rename All";
            this.RenameAll.UseVisualStyleBackColor = true;
            this.RenameAll.Click += new System.EventHandler(this.RenameAll_Click);
            // 
            // RenameSelected
            // 
            this.RenameSelected.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.RenameSelected.Location = new System.Drawing.Point(183, 500);
            this.RenameSelected.Name = "RenameSelected";
            this.RenameSelected.Size = new System.Drawing.Size(171, 34);
            this.RenameSelected.TabIndex = 5;
            this.RenameSelected.Text = "Rename Selected";
            this.RenameSelected.UseVisualStyleBackColor = true;
            this.RenameSelected.Click += new System.EventHandler(this.RenameSelected_Click);
            // 
            // RenameNotSelected
            // 
            this.RenameNotSelected.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.RenameNotSelected.Location = new System.Drawing.Point(357, 500);
            this.RenameNotSelected.Name = "RenameNotSelected";
            this.RenameNotSelected.Size = new System.Drawing.Size(180, 34);
            this.RenameNotSelected.TabIndex = 6;
            this.RenameNotSelected.Text = "Rename Unselected";
            this.RenameNotSelected.UseVisualStyleBackColor = true;
            this.RenameNotSelected.Click += new System.EventHandler(this.RenameNotSelected_Click);
            // 
            // pBar1
            // 
            this.pBar1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.pBar1.Location = new System.Drawing.Point(48, 252);
            this.pBar1.Name = "pBar1";
            this.pBar1.Size = new System.Drawing.Size(872, 34);
            this.pBar1.TabIndex = 104;
            // 
            // fastObjListView1
            // 
            this.fastObjListView1.AllowColumnReorder = true;
            this.fastObjListView1.AlternateRowBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.fastObjListView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fastObjListView1.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.SingleClick;
            this.fastObjListView1.CellEditEnterChangesRows = true;
            this.fastObjListView1.CellEditTabChangesRows = true;
            this.fastObjListView1.CellEditUseWholeCell = false;
            this.fastObjListView1.CheckBoxes = true;
            this.fastObjListView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olv_FilenamesOriginal,
            this.olv_NewFilenames,
            this.olv_SrcLanguage,
            this.olv_FilePath,
            this.olv_FileExt});
            this.fastObjListView1.ColumnsNotEditable = null;
            this.fastObjListView1.FullRowSelect = true;
            this.fastObjListView1.Location = new System.Drawing.Point(2, 40);
            this.fastObjListView1.Name = "fastObjListView1";
            this.fastObjListView1.ShowGroups = false;
            this.fastObjListView1.ShowImagesOnSubItems = true;
            this.fastObjListView1.ShowItemToolTips = true;
            this.fastObjListView1.Size = new System.Drawing.Size(967, 450);
            this.fastObjListView1.TabIndex = 3;
            this.fastObjListView1.TintSortColumn = true;
            this.fastObjListView1.UseAlternatingBackColors = true;
            this.fastObjListView1.UseCellFormatEvents = true;
            this.fastObjListView1.UseCompatibleStateImageBehavior = false;
            this.fastObjListView1.UseFilterIndicator = true;
            this.fastObjListView1.UseFiltering = true;
            this.fastObjListView1.View = System.Windows.Forms.View.Details;
            this.fastObjListView1.VirtualMode = true;
            // 
            // olv_FilenamesOriginal
            // 
            this.olv_FilenamesOriginal.AspectName = "Name";
            this.olv_FilenamesOriginal.Text = "Filenames (Original)";
            this.olv_FilenamesOriginal.Width = 280;
            // 
            // olv_NewFilenames
            // 
            this.olv_NewFilenames.AspectName = "NewFilename";
            this.olv_NewFilenames.HeaderForeColor = System.Drawing.Color.DarkRed;
            this.olv_NewFilenames.Text = "Candidates to Rename";
            this.olv_NewFilenames.Width = 320;
            // 
            // olv_SrcLanguage
            // 
            this.olv_SrcLanguage.AspectName = "SrcLanguage";
            this.olv_SrcLanguage.Text = "Source Language";
            this.olv_SrcLanguage.Width = 158;
            // 
            // olv_FilePath
            // 
            this.olv_FilePath.AspectName = "ParentPath";
            this.olv_FilePath.Text = "Parent Path";
            this.olv_FilePath.Width = 150;
            // 
            // olv_FileExt
            // 
            this.olv_FileExt.AspectName = "FileExt";
            this.olv_FileExt.Text = "Ext";
            this.olv_FileExt.Width = 55;
            // 
            // labelFolder
            // 
            this.labelFolder.AutoSize = true;
            this.labelFolder.Location = new System.Drawing.Point(4, 3);
            this.labelFolder.Name = "labelFolder";
            this.labelFolder.Size = new System.Drawing.Size(66, 25);
            this.labelFolder.TabIndex = 106;
            this.labelFolder.Text = "Folder:";
            // 
            // TargetedPath
            // 
            this.TargetedPath.Location = new System.Drawing.Point(74, 3);
            this.TargetedPath.Name = "TargetedPath";
            this.TargetedPath.Size = new System.Drawing.Size(357, 31);
            this.TargetedPath.TabIndex = 0;
            this.TargetedPath.WordWrap = false;
            this.TargetedPath.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.TargetedPath_PreviewKeyDown);
            // 
            // FilterText
            // 
            this.FilterText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FilterText.Location = new System.Drawing.Point(694, 3);
            this.FilterText.Name = "FilterText";
            this.FilterText.Size = new System.Drawing.Size(272, 31);
            this.FilterText.TabIndex = 2;
            this.FilterText.TextChanged += new System.EventHandler(this.FilterText_TextChanged);
            // 
            // labelFilter
            // 
            this.labelFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelFilter.AutoSize = true;
            this.labelFilter.Location = new System.Drawing.Point(638, 4);
            this.labelFilter.Name = "labelFilter";
            this.labelFilter.Size = new System.Drawing.Size(54, 25);
            this.labelFilter.TabIndex = 108;
            this.labelFilter.Text = "Filter:";
            // 
            // RenameUnchecked
            // 
            this.RenameUnchecked.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.RenameUnchecked.Location = new System.Drawing.Point(714, 500);
            this.RenameUnchecked.Name = "RenameUnchecked";
            this.RenameUnchecked.Size = new System.Drawing.Size(187, 34);
            this.RenameUnchecked.TabIndex = 8;
            this.RenameUnchecked.Text = "Rename Unchecked";
            this.RenameUnchecked.UseVisualStyleBackColor = true;
            this.RenameUnchecked.Click += new System.EventHandler(this.RenameUnchecked_Click);
            // 
            // RenameChecked
            // 
            this.RenameChecked.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.RenameChecked.Location = new System.Drawing.Point(540, 500);
            this.RenameChecked.Name = "RenameChecked";
            this.RenameChecked.Size = new System.Drawing.Size(171, 34);
            this.RenameChecked.TabIndex = 7;
            this.RenameChecked.Text = "Rename Checked";
            this.RenameChecked.UseVisualStyleBackColor = true;
            this.RenameChecked.Click += new System.EventHandler(this.RenameChecked_Click);
            // 
            // labelTargetLang
            // 
            this.labelTargetLang.AutoSize = true;
            this.labelTargetLang.Location = new System.Drawing.Point(2, 140);
            this.labelTargetLang.Name = "labelTargetLang";
            this.labelTargetLang.Size = new System.Drawing.Size(280, 25);
            this.labelTargetLang.TabIndex = 110;
            this.labelTargetLang.Text = "2 Letter ISO6391 Target Language";
            // 
            // TargetLanguage
            // 
            this.TargetLanguage.Location = new System.Drawing.Point(288, 140);
            this.TargetLanguage.MaxLength = 2;
            this.TargetLanguage.Name = "TargetLanguage";
            this.TargetLanguage.Size = new System.Drawing.Size(37, 31);
            this.TargetLanguage.TabIndex = 109;
            // 
            // labelSourceLanguage
            // 
            this.labelSourceLanguage.AutoSize = true;
            this.labelSourceLanguage.Location = new System.Drawing.Point(2, 180);
            this.labelSourceLanguage.Name = "labelSourceLanguage";
            this.labelSourceLanguage.Size = new System.Drawing.Size(286, 25);
            this.labelSourceLanguage.TabIndex = 112;
            this.labelSourceLanguage.Text = "2 Letter ISO6391 Source Language";
            // 
            // SourceLanguage
            // 
            this.SourceLanguage.Location = new System.Drawing.Point(288, 174);
            this.SourceLanguage.MaxLength = 2;
            this.SourceLanguage.Name = "SourceLanguage";
            this.SourceLanguage.Size = new System.Drawing.Size(37, 31);
            this.SourceLanguage.TabIndex = 111;
            // 
            // checkBoxRecursive
            // 
            this.checkBoxRecursive.AutoSize = true;
            this.checkBoxRecursive.Location = new System.Drawing.Point(2, 56);
            this.checkBoxRecursive.Name = "checkBoxRecursive";
            this.checkBoxRecursive.Size = new System.Drawing.Size(181, 29);
            this.checkBoxRecursive.TabIndex = 113;
            this.checkBoxRecursive.Text = "Search Recursively";
            this.checkBoxRecursive.UseVisualStyleBackColor = true;
            // 
            // checkBoxLongPathSupport
            // 
            this.checkBoxLongPathSupport.AutoSize = true;
            this.checkBoxLongPathSupport.Location = new System.Drawing.Point(2, 93);
            this.checkBoxLongPathSupport.Name = "checkBoxLongPathSupport";
            this.checkBoxLongPathSupport.Size = new System.Drawing.Size(187, 29);
            this.checkBoxLongPathSupport.TabIndex = 114;
            this.checkBoxLongPathSupport.Text = "Long Path Support";
            this.checkBoxLongPathSupport.UseVisualStyleBackColor = true;
            // 
            // labelMaxThread
            // 
            this.labelMaxThread.AutoSize = true;
            this.labelMaxThread.Location = new System.Drawing.Point(784, 60);
            this.labelMaxThread.Name = "labelMaxThread";
            this.labelMaxThread.Size = new System.Drawing.Size(112, 25);
            this.labelMaxThread.TabIndex = 120;
            this.labelMaxThread.Text = "Max Threads";
            // 
            // MaxThread
            // 
            this.MaxThread.Location = new System.Drawing.Point(902, 54);
            this.MaxThread.MaxLength = 2;
            this.MaxThread.Name = "MaxThread";
            this.MaxThread.Size = new System.Drawing.Size(62, 31);
            this.MaxThread.TabIndex = 119;
            // 
            // labelMaxTranslateLen
            // 
            this.labelMaxTranslateLen.AutoSize = true;
            this.labelMaxTranslateLen.Location = new System.Drawing.Point(733, 97);
            this.labelMaxTranslateLen.Name = "labelMaxTranslateLen";
            this.labelMaxTranslateLen.Size = new System.Drawing.Size(166, 25);
            this.labelMaxTranslateLen.TabIndex = 122;
            this.labelMaxTranslateLen.Text = "Max Translation Len";
            // 
            // MaxTranslateLen
            // 
            this.MaxTranslateLen.Location = new System.Drawing.Point(902, 91);
            this.MaxTranslateLen.MaxLength = 5;
            this.MaxTranslateLen.Name = "MaxTranslateLen";
            this.MaxTranslateLen.Size = new System.Drawing.Size(62, 31);
            this.MaxTranslateLen.TabIndex = 121;
            // 
            // checkBoxAppendLanguageName
            // 
            this.checkBoxAppendLanguageName.AutoSize = true;
            this.checkBoxAppendLanguageName.Location = new System.Drawing.Point(267, 56);
            this.checkBoxAppendLanguageName.Name = "checkBoxAppendLanguageName";
            this.checkBoxAppendLanguageName.Size = new System.Drawing.Size(236, 29);
            this.checkBoxAppendLanguageName.TabIndex = 124;
            this.checkBoxAppendLanguageName.Text = "Append Language Name";
            this.checkBoxAppendLanguageName.UseVisualStyleBackColor = true;
            // 
            // checkBoxAppendOrginalName
            // 
            this.checkBoxAppendOrginalName.AutoSize = true;
            this.checkBoxAppendOrginalName.Location = new System.Drawing.Point(267, 93);
            this.checkBoxAppendOrginalName.Name = "checkBoxAppendOrginalName";
            this.checkBoxAppendOrginalName.Size = new System.Drawing.Size(217, 29);
            this.checkBoxAppendOrginalName.TabIndex = 123;
            this.checkBoxAppendOrginalName.Text = "Append Orginal Name";
            this.checkBoxAppendOrginalName.UseVisualStyleBackColor = true;
            // 
            // labelFileType
            // 
            this.labelFileType.AutoSize = true;
            this.labelFileType.Location = new System.Drawing.Point(582, 56);
            this.labelFileType.Name = "labelFileType";
            this.labelFileType.Size = new System.Drawing.Size(80, 25);
            this.labelFileType.TabIndex = 126;
            this.labelFileType.Text = "File Type";
            // 
            // FileType
            // 
            this.FileType.Location = new System.Drawing.Point(668, 56);
            this.FileType.MaxLength = 128;
            this.FileType.Name = "FileType";
            this.FileType.Size = new System.Drawing.Size(45, 31);
            this.FileType.TabIndex = 125;
            // 
            // comboBoxFilesPerTransReq
            // 
            this.comboBoxFilesPerTransReq.FormattingEnabled = true;
            this.comboBoxFilesPerTransReq.Items.AddRange(new object[] {
            "Auto",
            "OnePerFile",
            "Many"});
            this.comboBoxFilesPerTransReq.Location = new System.Drawing.Point(556, 140);
            this.comboBoxFilesPerTransReq.Name = "comboBoxFilesPerTransReq";
            this.comboBoxFilesPerTransReq.Size = new System.Drawing.Size(155, 33);
            this.comboBoxFilesPerTransReq.TabIndex = 127;
            this.comboBoxFilesPerTransReq.Text = "Auto";
            // 
            // labelFilesPerTransReq
            // 
            this.labelFilesPerTransReq.AutoSize = true;
            this.labelFilesPerTransReq.Location = new System.Drawing.Point(345, 142);
            this.labelFilesPerTransReq.Name = "labelFilesPerTransReq";
            this.labelFilesPerTransReq.Size = new System.Drawing.Size(205, 25);
            this.labelFilesPerTransReq.TabIndex = 128;
            this.labelFilesPerTransReq.Text = "Files-Per-Translation-Req";
            // 
            // RefreshList
            // 
            this.RefreshList.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.RefreshList.Location = new System.Drawing.Point(552, 3);
            this.RefreshList.Name = "RefreshList";
            this.RefreshList.Size = new System.Drawing.Size(82, 34);
            this.RefreshList.TabIndex = 129;
            this.RefreshList.Text = "Refresh";
            this.RefreshList.UseVisualStyleBackColor = true;
            this.RefreshList.Click += new System.EventHandler(this.RefreshList_Click);
            // 
            // Form1
            // 
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(968, 538);
            this.Controls.Add(this.RefreshList);
            this.Controls.Add(this.RenameUnchecked);
            this.Controls.Add(this.RenameChecked);
            this.Controls.Add(this.labelFilter);
            this.Controls.Add(this.FilterText);
            this.Controls.Add(this.TargetedPath);
            this.Controls.Add(this.labelFolder);
            this.Controls.Add(this.pBar1);
            this.Controls.Add(this.RenameNotSelected);
            this.Controls.Add(this.RenameSelected);
            this.Controls.Add(this.RenameAll);
            this.Controls.Add(this.TestButton);
            this.Controls.Add(this.ClearList);
            this.Controls.Add(this.AddFiles);
            this.Controls.Add(this.fastObjListView1);
            this.Controls.Add(this.labelFilesPerTransReq);
            this.Controls.Add(this.comboBoxFilesPerTransReq);
            this.Controls.Add(this.labelFileType);
            this.Controls.Add(this.FileType);
            this.Controls.Add(this.checkBoxAppendLanguageName);
            this.Controls.Add(this.checkBoxAppendOrginalName);
            this.Controls.Add(this.labelMaxTranslateLen);
            this.Controls.Add(this.MaxTranslateLen);
            this.Controls.Add(this.labelMaxThread);
            this.Controls.Add(this.MaxThread);
            this.Controls.Add(this.checkBoxLongPathSupport);
            this.Controls.Add(this.checkBoxRecursive);
            this.Controls.Add(this.labelSourceLanguage);
            this.Controls.Add(this.SourceLanguage);
            this.Controls.Add(this.TargetLanguage);
            this.Controls.Add(this.labelTargetLang);
            this.MinimumSize = new System.Drawing.Size(980, 250);
            this.Name = "Form1";
            this.Text = "TranslateFilenames - Utility to Translate Language(s) of Filenames in Bulk";
            ((System.ComponentModel.ISupportInitialize)(this.fastObjListView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Button AddFiles;
        private Button ClearList;
        private Button TestButton;
        private Button RenameAll;
        private Button RenameSelected;
        private Button RenameNotSelected;
        private Button RenameChecked;
        private Button RenameUnchecked;
        private ProgressBar pBar1;
        private FastObjectListView fastObjListView1;
        private OLVColumn olv_FilenamesOriginal;
        private OLVColumn olv_NewFilenames;
        private OLVColumn olv_SrcLanguage;
        private OLVColumn olv_FilePath;
        private OLVColumn olv_FileExt;
        private Label labelFolder;
        private TextBox TargetedPath;
        private TextBox FilterText;
        private Label labelFilter;
        private Label labelTargetLang;
        private TextBox TargetLanguage;
        private Label labelSourceLanguage;
        private TextBox SourceLanguage;
        private CheckBox checkBoxRecursive;
        private CheckBox checkBoxLongPathSupport;
        private Label labelMaxThread;
        private TextBox MaxThread;
        private Label labelMaxTranslateLen;
        private TextBox MaxTranslateLen;
        private CheckBox checkBoxAppendLanguageName;
        private CheckBox checkBoxAppendOrginalName;
        private Label labelFileType;
        private TextBox FileType;
        private ComboBox comboBoxFilesPerTransReq;
        private Label labelFilesPerTransReq;
        private Button RefreshList;
    }
}