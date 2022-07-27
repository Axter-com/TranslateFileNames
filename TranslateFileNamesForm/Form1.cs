using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using TranslateFilenamesCore;
using FileDetails = TranslateFilenamesCore.TranslateFilenames.FileDetails;
using Microsoft.VisualBasic.ApplicationServices;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.Design.AxImporter;
using static TranslateFilenamesCore.TranslateFilenames;
using System.Runtime.InteropServices;

namespace TranslateFileNamesForm
{
    public partial class Form1 : Form
    {
        private static string _path = Directory.GetCurrentDirectory();
        private static List<FileDetails> filesDetails = new List<FileDetails>();
        private static readonly object _lock = new object();

        public delegate void UpdateList();
        public UpdateList UpdateListDelegate;

        public delegate void SetupProgressBarDelegateType();
        public SetupProgressBarDelegateType SetupProgressBarDelegate;
        public int QtyFilesToCheck = 0;
        public int QtyFilesRenameCandidate = 0;
        TranslateFilenamesFrm _translateFilenames;

        public Form1()
        {
            InitializeComponent();
            UpdateListDelegate = new UpdateList(UpdateListMethod);
            SetupProgressBarDelegate = new SetupProgressBarDelegateType(UpdateProgressBar);
            listViewFiles.FullRowSelect = true;
            ClearItemsOnList();
            TranslateFilenamesCore.TranslateFilenames.TranslateFilenames_Options options = new TranslateFilenamesCore.TranslateFilenames.TranslateFilenames_Options();
            //options._maxWorkerThreads = 0;
            _translateFilenames = new TranslateFilenamesFrm(this, options);

        }

        private static FileDetails Get_filesDetails()
        {
            FileDetails fileDetails = null;
            Monitor.Enter(_lock);
            try
            {
                if (filesDetails.Count > 0)
                {
                    fileDetails = filesDetails[0];
                    filesDetails.RemoveAt(0);
                }
                Monitor.Pulse(_lock);
            }
            finally
            {
                Monitor.Exit(_lock);
            }
            return fileDetails;
        }

        public static void AddTo_filesDetails(FileDetails fileDetails)
        {
            if (fileDetails == null)
                return;
            Monitor.Enter(_lock);
            try
            {
                filesDetails.Add(fileDetails);
                Monitor.Pulse(_lock);
            }
            finally
            {
                Monitor.Exit(_lock);
            }
        }

        public void UpdateListMethod()
        {
            FileDetails fileDetails = Get_filesDetails();
            AddToList(fileDetails);
        }
        public void SetupProgressBar(int QtyFilesToCheck)
        {
            //this.FilterOption_CmboBx.Enabled = true;
            pBar1.Visible = true;
            pBar1.Minimum = 1;
            pBar1.Value = 1;
            pBar1.Step = 1;
            pBar1.Maximum = QtyFilesToCheck;
        }

        public void UpdateProgressBar()
        {
            if (QtyFilesRenameCandidate > -1)
            {
                pBar1.Value = pBar1.Maximum;
                pBar1.Update();
                this.UseWaitCursor = false;
                pBar1.Visible = false;
                this.listViewFiles.Enabled = true ;
            }
            else
                SetupProgressBar(QtyFilesToCheck);
        }

        private void ClearItemsOnList()
        {
            while (listViewFiles.Items.Count > 0)
                listViewFiles.Items.RemoveAt(0);
            this.FilterOption_CmboBx.Enabled = false;
            QtyFilesToCheck = 0;
            QtyFilesRenameCandidate = -1;
            pBar1.Visible = false;
        }

        private void ClearList_Click(object sender, EventArgs e)
        {
            ClearItemsOnList();
        }

        private void AddFiles_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowserDialog1.Description = "Select the directory containing files to rename.";
            // Do not allow the user to create new files via the FolderBrowserDialog.
            folderBrowserDialog1.ShowNewFolderButton = false;
            // Default to the My Documents folder.
            folderBrowserDialog1.RootFolder = Environment.SpecialFolder.Personal;
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                _path = folderBrowserDialog1.SelectedPath;
                AddFilesToList(_path);
            }
        }

        private void TestButton_Click(object sender, EventArgs e)
        {
            var p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "E:\\Temp\\LanguageChangeTest\\renametest\\Reset_TestFiles.cmd";
            p.StartInfo.WorkingDirectory = "E:\\Temp\\LanguageChangeTest\\renametest";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            Thread.Sleep(2000);
            AddFilesToList("C:\\Users\\david\\Pictures\\renametest\\t1");
        }

        private async Task AddFilesToList_Task(string targetPath)
        {
            Debug.WriteLine("Info: AddFilesToList_Task **** Entering.");
            _translateFilenames.TranslatePath(targetPath);
            Debug.WriteLine("Info: AddFilesToList_Task **** Exiting.");
        }
        //private async Task AddFilesToList_sub(string targetPath)
        //{
        //    Debug.WriteLine("Info: AddFilesToList_sub ----------- Entering.");
        //    await AddFilesToList_Task(targetPath);
        //    Debug.WriteLine("Info: AddFilesToList_sub ----------- Exiting.");
        //}

        private void AddFilesToList(string targetPath)
        {
            Debug.WriteLine("Info: AddFilesToList Entering.");
            _path = targetPath;
            ClearItemsOnList();
            SetupProgressBar(3);
            pBar1.PerformStep();
            pBar1.PerformStep();
            this.UseWaitCursor = true;
            this.listViewFiles.Enabled = false;
            Task.Run(() => AddFilesToList_Task(targetPath));
            Debug.WriteLine("Info: AddFilesToList Exiting.");
        }
        public void AddToList(FileDetails fileDetails)
        {
            pBar1.PerformStep();
            string oldFileName = fileDetails.Name.ToString();
            string newFileName = fileDetails.Translation.ToString();
            Debug.WriteLine("Info: AddToList Entering with file '" + oldFileName + "'.");
            if (fileDetails.Name.Length != 0)
            {
                var it = new ListViewItem(fileDetails.Name);
                try
                {
                    it.UseItemStyleForSubItems = false;
                    Debug.WriteLine("Info: AddToList adding infor for file '" + oldFileName + "' trans = '" + newFileName + "'");
                    listViewFiles.Items.Add(it);
                    fileDetails.Translation = _translateFilenames.FileDetailsReplaceIllegalFileNameChar(fileDetails);// Replaces illegal windows file name characters with alternative characters
                    it.SubItems.Add(fileDetails.Translation, Color.DarkRed, Color.WhiteSmoke, new Font(listViewFiles.Font, FontStyle.Bold));
                    string SourceLang = fileDetails.SourceLang;
                    SourceLang = SourceLang.TrimStart(new string("_[").ToCharArray());
                    SourceLang = SourceLang.TrimEnd(']').ToString();
                    it.SubItems.Add(SourceLang);
                    it.SubItems.Add(fileDetails.ParrentPath);
                    it.SubItems.Add(fileDetails.Extension);
                    Debug.WriteLine("Info: AddToList added row for file '" + oldFileName + "'.");
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error: AddToList EXCEPTION thrown with file '" + oldFileName + "'.\nErrMsg\n" + e.Message);
                }
            }
            Debug.WriteLine("Info: AddToList Exiting.");
        }

        private void RenameItem(string OrgName, string NewName, string ParentPath, string OrgLang, string Extension)
        {
            FileDetails fileDetail = new FileDetails { Name = OrgName, Extension = Extension, Translation = NewName, ParrentPath = ParentPath, SourceLang = OrgLang };
            string oldFileName = Path.Combine(fileDetail.ParrentPath, fileDetail.Name + fileDetail.Extension);
            string appendedName = _translateFilenames.GetAppendedName(fileDetail);
            string newFileName = Path.Combine(fileDetail.ParrentPath, fileDetail.Translation + appendedName + fileDetail.Extension);
            _translateFilenames.RenameFile(fileDetail, oldFileName, newFileName, true);
        }
        private void RenameItems(bool GetSelected = false, bool GetNotSelected = false)
        {
            this.UseWaitCursor = true;
            this.listViewFiles.Enabled = false;
            System.Windows.Forms.ListView.ListViewItemCollection Items = listViewFiles.Items;
            System.Windows.Forms.ListView.SelectedListViewItemCollection sItems = listViewFiles.SelectedItems;
            int PrgBarCount = GetSelected ? sItems.Count : (GetNotSelected ? Items.Count - sItems.Count : Items.Count);
            int Count = GetSelected ? sItems.Count : Items.Count;
            SetupProgressBar(PrgBarCount);
            for (int i = 0; i < Count; i++)
            {
                if (GetNotSelected && sItems.Contains(Items[i]))
                    continue;
                string OrgName = GetSelected ? sItems[i].SubItems[0].Text : Items[i].SubItems[0].Text;
                string NewName = GetSelected ? sItems[i].SubItems[1].Text : Items[i].SubItems[1].Text;
                string OrgLang = GetSelected ? sItems[i].SubItems[2].Text : Items[i].SubItems[2].Text;
                string ParentPath = GetSelected ? sItems[i].SubItems[3].Text : Items[i].SubItems[3].Text;
                string Extension = GetSelected ? sItems[i].SubItems[4].Text : Items[i].SubItems[4].Text;
                RenameItem(OrgName, NewName, ParentPath, OrgLang, Extension);
                pBar1.PerformStep();
            }
            this.UseWaitCursor = false;
            pBar1.Visible = false;
            this.listViewFiles.Enabled = true;
        }
        private void RenameAll_Click(object sender, EventArgs e)
        {
            RenameItems();
        }

        private void RenameSelected_Click(object sender, EventArgs e)
        {
            RenameItems(true);
        }

        private void RenameNotSelected_Click(object sender, EventArgs e)
        {
            RenameItems(false, true);
        }
    }

    public class TranslateFilenamesFrm : TranslateFilenames
    {
        private Form1 _form1 = null;
        public TranslateFilenamesFrm(Form1 form1, TranslateFilenames_Options options = null) : base(options)
        {
            _form1 = form1;
        }
        public TranslateFilenamesFrm(string[] args, Form1 form1) : base(args, true, true)
        {
            _form1 = form1;
        }

        protected override void RenameRequired(FileDetails fileDetail, string oldFileName, string newFileName)
        {
            Form1.AddTo_filesDetails(fileDetail);
            _form1.Invoke(_form1.UpdateListDelegate);
        }

        protected override void InitializeProgressBar(int QtyFilesToCheck)
        {
            _form1.QtyFilesToCheck = QtyFilesToCheck;
            _form1.Invoke(_form1.SetupProgressBarDelegate);
        }

        protected override void TaskComplete(int QtyFilesRenameCandidate)
        {
            _form1.QtyFilesRenameCandidate = QtyFilesRenameCandidate;
            _form1.Invoke(_form1.SetupProgressBarDelegate);
        }

        /// <summary>
        /// Prints message to the console depending on settings for verbosity, silent, dotOutput, &  outputlevel
        /// </summary>
        /// <param name="message">String to print</param>
        protected override void OutPut(string message, OutPutLevel outputlevel = OutPutLevel.NormalLvl)
        {

        }

        /// <summary>
        /// Prints the help text to console.
        /// </summary>
        protected override void PrintHelp(bool AdvanceHelpPage)
        {
        }
    }


}