using System.Windows.Forms;
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("");
            this.FilenamesOriginal = new System.Windows.Forms.ColumnHeader();
            this.NewFilenames = new System.Windows.Forms.ColumnHeader();
            this.FilePath = new System.Windows.Forms.ColumnHeader();
            this.SrcLanguage = new System.Windows.Forms.ColumnHeader();
            this.AddFiles = new System.Windows.Forms.Button();
            this.ClearList = new System.Windows.Forms.Button();
            this.listViewFiles = new System.Windows.Forms.ListView();
            this.FileExt = new System.Windows.Forms.ColumnHeader();
            this.TestButton = new System.Windows.Forms.Button();
            this.RenameAll = new System.Windows.Forms.Button();
            this.RenameSelected = new System.Windows.Forms.Button();
            this.RenameNotSelected = new System.Windows.Forms.Button();
            this.FilterOption_CmboBx = new System.Windows.Forms.ComboBox();
            this.pBar1 = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // FilenamesOriginal
            // 
            this.FilenamesOriginal.Text = "Filename (Original)";
            this.FilenamesOriginal.Width = 400;
            // 
            // NewFilenames
            // 
            this.NewFilenames.Text = "Translated Filename";
            this.NewFilenames.Width = 400;
            // 
            // SrcLanguage
            // 
            this.SrcLanguage.Text = "Original Language";
            this.SrcLanguage.Width = 158;
            // 
            // FilePath
            // 
            this.FilePath.Text = "Parrent Path";
            this.FilePath.Width = 200;
            // 
            // FileExt
            // 
            this.FileExt.Text = "Ext";
            this.FileExt.Width = 55;
            // 
            // AddFiles
            // 
            this.AddFiles.Location = new System.Drawing.Point(2, 2);
            this.AddFiles.Name = "AddFiles";
            this.AddFiles.Size = new System.Drawing.Size(112, 34);
            this.AddFiles.TabIndex = 0;
            this.AddFiles.Text = "Add Files";
            this.AddFiles.UseVisualStyleBackColor = true;
            this.AddFiles.Click += new System.EventHandler(this.AddFiles_Click);
            // 
            // ClearList
            // 
            this.ClearList.Location = new System.Drawing.Point(1200, 2);
            this.ClearList.Name = "ClearList";
            this.ClearList.Size = new System.Drawing.Size(112, 34);
            this.ClearList.TabIndex = 1;
            this.ClearList.Text = "Clear List";
            this.ClearList.UseVisualStyleBackColor = true;
            this.ClearList.Click += new System.EventHandler(this.ClearList_Click);
            // 
            // listViewFiles
            // 
            this.listViewFiles.BackColor = System.Drawing.SystemColors.Control;
            this.listViewFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.FilenamesOriginal,
            this.NewFilenames,
            this.SrcLanguage,
            this.FilePath,
            this.FileExt});
            this.listViewFiles.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
            this.listViewFiles.Location = new System.Drawing.Point(2, 37);
            this.listViewFiles.Name = "listViewFiles";
            this.listViewFiles.Size = new System.Drawing.Size(1310, 649);
            this.listViewFiles.TabIndex = 4;
            this.listViewFiles.UseCompatibleStateImageBehavior = false;
            this.listViewFiles.View = System.Windows.Forms.View.Details;
            // 
            // TestButton
            // 
            this.TestButton.Location = new System.Drawing.Point(1083, 2);
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
            this.RenameAll.Location = new System.Drawing.Point(122, 2);
            this.RenameAll.Name = "RenameAll";
            this.RenameAll.Size = new System.Drawing.Size(112, 34);
            this.RenameAll.TabIndex = 100;
            this.RenameAll.Text = "Rename All";
            this.RenameAll.UseVisualStyleBackColor = true;
            this.RenameAll.Click += new System.EventHandler(this.RenameAll_Click);
            // 
            // RenameSelected
            // 
            this.RenameSelected.Location = new System.Drawing.Point(239, 2);
            this.RenameSelected.Name = "RenameSelected";
            this.RenameSelected.Size = new System.Drawing.Size(190, 34);
            this.RenameSelected.TabIndex = 101;
            this.RenameSelected.Text = "Rename Selected";
            this.RenameSelected.UseVisualStyleBackColor = true;
            this.RenameSelected.Click += new System.EventHandler(this.RenameSelected_Click);
            // 
            // RenameNotSelected
            // 
            this.RenameNotSelected.Location = new System.Drawing.Point(434, 2);
            this.RenameNotSelected.Name = "RenameNotSelected";
            this.RenameNotSelected.Size = new System.Drawing.Size(190, 34);
            this.RenameNotSelected.TabIndex = 102;
            this.RenameNotSelected.Text = "Rename Unselected";
            this.RenameNotSelected.UseVisualStyleBackColor = true;
            this.RenameNotSelected.Click += new System.EventHandler(this.RenameNotSelected_Click);
            // 
            // FilterOption_CmboBx
            // 
            this.FilterOption_CmboBx.FormattingEnabled = true;
            this.FilterOption_CmboBx.Items.AddRange(new object[] {
            "Clear All Filters",
            "Filter Translation Candidates",
            "Filter Candidates for Specific Language",
            "Filter Specific File Types",
            "Filter files with specific word in source language",
            "Filter files with specifc word in target language"});
            this.FilterOption_CmboBx.Location = new System.Drawing.Point(629, 2);
            this.FilterOption_CmboBx.Name = "FilterOption_CmboBx";
            this.FilterOption_CmboBx.Size = new System.Drawing.Size(449, 33);
            this.FilterOption_CmboBx.TabIndex = 103;
            this.FilterOption_CmboBx.Text = "Clear All Filters";
            // 
            // pBar1
            // 
            this.pBar1.Location = new System.Drawing.Point(99, 327);
            this.pBar1.Name = "pBar1";
            this.pBar1.Size = new System.Drawing.Size(1112, 34);
            this.pBar1.TabIndex = 104;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(1311, 688);
            this.Controls.Add(this.pBar1);
            this.Controls.Add(this.FilterOption_CmboBx);
            this.Controls.Add(this.RenameNotSelected);
            this.Controls.Add(this.RenameSelected);
            this.Controls.Add(this.RenameAll);
            this.Controls.Add(this.TestButton);
            this.Controls.Add(this.ClearList);
            this.Controls.Add(this.AddFiles);
            this.Controls.Add(this.listViewFiles);
            this.Name = "Form1";
            this.Text = "TranslateFilenames - Utility to Translate Language of Filenames in Bulk";
            this.ResumeLayout(false);

        }

        #endregion

        //private ListView listViewFiles;
        private ListView listViewFiles;
        private Button AddFiles;
        private ColumnHeader FilenamesOriginal;
        private ColumnHeader NewFilenames;
        private ColumnHeader FilePath;
        private Button ClearList;
        private ColumnHeader SrcLanguage;
        private Button TestButton;
        private Button RenameAll;
        private Button RenameSelected;
        private Button RenameNotSelected;
        private ComboBox FilterOption_CmboBx;
        private ProgressBar pBar1;
        private ColumnHeader FileExt;
    }
}