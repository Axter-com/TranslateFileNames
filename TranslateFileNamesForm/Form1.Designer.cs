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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
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
            this.label1 = new System.Windows.Forms.Label();
            this.TargetedPath = new System.Windows.Forms.TextBox();
            this.groupBoxFilter = new System.Windows.Forms.GroupBox();
            this.FilterText = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.fastObjListView1)).BeginInit();
            this.groupBoxFilter.SuspendLayout();
            this.SuspendLayout();
            // 
            // AddFiles
            // 
            this.AddFiles.Location = new System.Drawing.Point(497, 2);
            this.AddFiles.Name = "AddFiles";
            this.AddFiles.Size = new System.Drawing.Size(43, 34);
            this.AddFiles.TabIndex = 0;
            this.AddFiles.Text = "...";
            this.AddFiles.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.AddFiles.UseVisualStyleBackColor = true;
            this.AddFiles.Click += new System.EventHandler(this.AddFiles_Click);
            // 
            // ClearList
            // 
            this.ClearList.Location = new System.Drawing.Point(868, 1);
            this.ClearList.Name = "ClearList";
            this.ClearList.Size = new System.Drawing.Size(112, 34);
            this.ClearList.TabIndex = 1;
            this.ClearList.Text = "Clear List";
            this.ClearList.UseVisualStyleBackColor = true;
            this.ClearList.Click += new System.EventHandler(this.ClearList_Click);
            // 
            // TestButton
            // 
            this.TestButton.Location = new System.Drawing.Point(868, 36);
            this.TestButton.Name = "TestButton";
            this.TestButton.Size = new System.Drawing.Size(112, 34);
            this.TestButton.TabIndex = 99;
            this.TestButton.TabStop = false;
            this.TestButton.Text = "TestButton";
            this.TestButton.UseVisualStyleBackColor = true;
            this.TestButton.Click += new System.EventHandler(this.TestButton_Click);
            // 
            // RenameAll
            // 
            this.RenameAll.Location = new System.Drawing.Point(554, 1);
            this.RenameAll.Name = "RenameAll";
            this.RenameAll.Size = new System.Drawing.Size(112, 34);
            this.RenameAll.TabIndex = 100;
            this.RenameAll.Text = "Rename All";
            this.RenameAll.UseVisualStyleBackColor = true;
            this.RenameAll.Click += new System.EventHandler(this.RenameAll_Click);
            // 
            // RenameSelected
            // 
            this.RenameSelected.Location = new System.Drawing.Point(672, 1);
            this.RenameSelected.Name = "RenameSelected";
            this.RenameSelected.Size = new System.Drawing.Size(190, 34);
            this.RenameSelected.TabIndex = 101;
            this.RenameSelected.Text = "Rename Selected";
            this.RenameSelected.UseVisualStyleBackColor = true;
            this.RenameSelected.Click += new System.EventHandler(this.RenameSelected_Click);
            // 
            // RenameNotSelected
            // 
            this.RenameNotSelected.Location = new System.Drawing.Point(672, 36);
            this.RenameNotSelected.Name = "RenameNotSelected";
            this.RenameNotSelected.Size = new System.Drawing.Size(190, 34);
            this.RenameNotSelected.TabIndex = 102;
            this.RenameNotSelected.Text = "Rename Unselected";
            this.RenameNotSelected.UseVisualStyleBackColor = true;
            this.RenameNotSelected.Click += new System.EventHandler(this.RenameNotSelected_Click);
            // 
            // pBar1
            // 
            this.pBar1.Location = new System.Drawing.Point(99, 507);
            this.pBar1.Name = "pBar1";
            this.pBar1.Size = new System.Drawing.Size(1112, 34);
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
            this.fastObjListView1.FullRowSelect = true;
            this.fastObjListView1.Location = new System.Drawing.Point(2, 76);
            this.fastObjListView1.Name = "fastObjListView1";
            this.fastObjListView1.ShowGroups = false;
            this.fastObjListView1.ShowImagesOnSubItems = true;
            this.fastObjListView1.ShowItemToolTips = true;
            this.fastObjListView1.Size = new System.Drawing.Size(1310, 969);
            this.fastObjListView1.TabIndex = 105;
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
            this.olv_FilenamesOriginal.Width = 400;
            // 
            // olv_NewFilenames
            // 
            this.olv_NewFilenames.AspectName = "NewFilename";
            this.olv_NewFilenames.HeaderForeColor = System.Drawing.Color.DarkRed;
            this.olv_NewFilenames.Text = "Candidates for Translation";
            this.olv_NewFilenames.Width = 400;
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
            this.olv_FilePath.Width = 200;
            // 
            // olv_FileExt
            // 
            this.olv_FileExt.AspectName = "FileExt";
            this.olv_FileExt.Text = "Ext";
            this.olv_FileExt.Width = 55;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 25);
            this.label1.TabIndex = 106;
            this.label1.Text = "Folder:";
            // 
            // TargetedPath
            // 
            this.TargetedPath.Location = new System.Drawing.Point(94, 4);
            this.TargetedPath.Name = "TargetedPath";
            this.TargetedPath.Size = new System.Drawing.Size(397, 31);
            this.TargetedPath.TabIndex = 107;
            this.TargetedPath.TextChanged += new System.EventHandler(this.TargetedPath_TextChanged);
            // 
            // groupBoxFilter
            // 
            this.groupBoxFilter.Controls.Add(this.FilterText);
            this.groupBoxFilter.Location = new System.Drawing.Point(1000, -3);
            this.groupBoxFilter.Name = "groupBoxFilter";
            this.groupBoxFilter.Size = new System.Drawing.Size(305, 57);
            this.groupBoxFilter.TabIndex = 108;
            this.groupBoxFilter.TabStop = false;
            this.groupBoxFilter.Text = "Filter";
            // 
            // FilterText
            // 
            this.FilterText.Location = new System.Drawing.Point(6, 22);
            this.FilterText.Name = "FilterText";
            this.FilterText.Size = new System.Drawing.Size(294, 31);
            this.FilterText.TabIndex = 0;
            this.FilterText.TextChanged += new System.EventHandler(this.FilterText_TextChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(1311, 1048);
            this.Controls.Add(this.groupBoxFilter);
            this.Controls.Add(this.TargetedPath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pBar1);
            this.Controls.Add(this.RenameNotSelected);
            this.Controls.Add(this.RenameSelected);
            this.Controls.Add(this.RenameAll);
            this.Controls.Add(this.TestButton);
            this.Controls.Add(this.ClearList);
            this.Controls.Add(this.AddFiles);
            this.Controls.Add(this.fastObjListView1);
            this.Name = "Form1";
            this.Text = "TranslateFilenames - Utility to Translate Language(s) of Filenames in Bulk";
            ((System.ComponentModel.ISupportInitialize)(this.fastObjListView1)).EndInit();
            this.groupBoxFilter.ResumeLayout(false);
            this.groupBoxFilter.PerformLayout();
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
        private ProgressBar pBar1;
        private FastObjectListView fastObjListView1;
        private OLVColumn olv_FilenamesOriginal;
        private OLVColumn olv_NewFilenames;
        private OLVColumn olv_SrcLanguage;
        private OLVColumn olv_FilePath;
        private OLVColumn olv_FileExt;
        private Label label1;
        private TextBox TargetedPath;
        private GroupBox groupBoxFilter;
        private TextBox FilterText;
    }
}