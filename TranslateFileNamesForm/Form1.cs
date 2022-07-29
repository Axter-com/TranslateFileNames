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
using BrightIdeasSoftware;
using System.ComponentModel;
using System.Collections;

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
            fastObjListView1.FormatCell += FormatCell;
            ClearItemsOnList();
            TranslateFilenamesCore.TranslateFilenames.TranslateFilenames_Options options = new TranslateFilenamesCore.TranslateFilenames.TranslateFilenames_Options();
            _translateFilenames = new TranslateFilenamesFrm(this, options);
#if !DEBUG
            this.TestButton.Enabled = false;
            this.TestButton.Visible = false;
#endif
        }

        private static FileDetails? Get_filesDetails()
        {
            FileDetails? fileDetails = null;
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
                this.fastObjListView1.Enabled = true;
                ClearList.Enabled = true;
                RenameAll.Enabled = true;
                RenameSelected.Enabled = true;
                RenameNotSelected.Enabled = true;
            }
            else
                SetupProgressBar(QtyFilesToCheck);
        }

        private void ClearItemsOnList()
        {
            this.fastObjListView1.ClearObjects();
            QtyFilesToCheck = 0;
            QtyFilesRenameCandidate = -1;
            pBar1.Visible = false;
            //ClearList.di
            ClearList.Enabled = false;
            RenameAll.Enabled = false;
            RenameSelected.Enabled = false;
            RenameNotSelected.Enabled = false;
            this.Controls.Add(RenameAll);
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

        private void AddFilesToList(string targetPath)
        {
            Debug.WriteLine("Info: AddFilesToList Entering.");
            _path = targetPath;
            ClearItemsOnList();
            SetupProgressBar(3);
            pBar1.PerformStep();
            pBar1.PerformStep();
            this.UseWaitCursor = true;
            this.fastObjListView1.Enabled = false;
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
                try
                {
                    Debug.WriteLine("Info: AddToList adding infor for file '" + oldFileName + "' trans = '" + newFileName + "'");
                    fileDetails.Translation = _translateFilenames.FileDetailsReplaceIllegalFileNameChar(fileDetails);// Replaces illegal windows file name characters with alternative characters
                    string SourceLang = fileDetails.SourceLang;
                    SourceLang = SourceLang.TrimStart(new string("_[").ToCharArray());
                    SourceLang = SourceLang.TrimEnd(']').ToString();

                    ArrayList l = new ArrayList();
                    FileRowDetails fileRowDetails = new FileRowDetails(fileDetails.Name)
                    {
                        Name = fileDetails.Name,
                        NewFilename = fileDetails.Translation,
                        SrcLanguage = SourceLang,
                        ParentPath = fileDetails.ParrentPath,
                        FileExt = fileDetails.Extension,
                    };
                    l.Add(fileRowDetails);
                    this.fastObjListView1.AddObjects(l);
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

        public enum WhichItems
        {
            AllItems,
            SelectedItems,
            UnselectedItems,
            CheckedItems,
            UncheckedItems
        }
        private void RenameItems(WhichItems whichItems = WhichItems.AllItems) // bool GetSelected = false, bool GetNotSelected = false, bool GetChecked = false)
        {
            int QtyRenamed = 0;
            string OrgName = "", LastRenameSuccess = "";
            try
            {
                this.UseWaitCursor = true;
                this.fastObjListView1.Enabled = false;
                System.Windows.Forms.ListView.ListViewItemCollection Items = fastObjListView1.Items;
                IList s_arrayList = fastObjListView1.SelectedObjects;
                IList c_arrayList = fastObjListView1.CheckedObjects;
                int sCount = s_arrayList.Count;
                int cCount = c_arrayList.Count;
                int iCount = fastObjListView1.GetItemCount();
                if (whichItems == WhichItems.SelectedItems && cCount > 0 && (sCount == 0 || (sCount == 1 && cCount > sCount)))
                    whichItems = WhichItems.CheckedItems;
                else if (whichItems == WhichItems.UnselectedItems && cCount > 0 && (sCount == 0 || (sCount == 1 && cCount > sCount)))
                    whichItems = WhichItems.UncheckedItems;
                int PrgBarCount = 0;
                int Count = 0;
                switch(whichItems)
                {
                    case WhichItems.AllItems:
                        PrgBarCount = iCount;
                        Count = iCount;
                        break;
                    case WhichItems.SelectedItems:
                        PrgBarCount = sCount;
                        Count = sCount;
                        break;
                    case WhichItems.UnselectedItems:
                        PrgBarCount = iCount - sCount;
                        Count = iCount;
                        break;
                    case WhichItems.CheckedItems:
                        PrgBarCount = cCount;
                        Count = cCount;
                        break;
                    case WhichItems.UncheckedItems:
                        PrgBarCount = iCount - cCount;
                        Count = iCount;
                        break;
                }

                SetupProgressBar(PrgBarCount);
                for (int i = 0; i < Count; i++)
                {
                    if ((whichItems == WhichItems.UnselectedItems && s_arrayList.Contains(fastObjListView1.GetModelObject(i))) ||
                        (whichItems == WhichItems.UncheckedItems && c_arrayList.Contains(fastObjListView1.GetModelObject(i))))
                        continue;
                    if (whichItems == WhichItems.SelectedItems || whichItems == WhichItems.CheckedItems)
                    {
                        FileRowDetails fileRowDetails = whichItems == WhichItems.SelectedItems ? (FileRowDetails)s_arrayList[i] : (FileRowDetails)c_arrayList[i];
                        OrgName = fileRowDetails.Name;
                        RenameItem(OrgName, fileRowDetails.NewFilename, fileRowDetails.ParentPath, fileRowDetails.SrcLanguage, fileRowDetails.FileExt);
                    }
                    else
                    {
                        OrgName = Items[i].SubItems[0].Text;
                        string NewName = Items[i].SubItems[1].Text;
                        string OrgLang = Items[i].SubItems[2].Text;
                        string ParentPath = Items[i].SubItems[3].Text;
                        string Extension = Items[i].SubItems[4].Text;
                        RenameItem(OrgName, NewName, ParentPath, OrgLang, Extension);
                    }
                    pBar1.PerformStep();
                    ++QtyRenamed;
                    LastRenameSuccess = OrgName;
                }
            } catch (Exception e) 
            {
                string LastRenameInfo = (OrgName.Length > 0) ? "; LastFile='" + OrgName + "'; " : "; ";
                if (LastRenameSuccess.Length > 0)
                    LastRenameInfo += "LastSuccess='" + LastRenameSuccess + "'; ";
                Debug.WriteLine("ERROR: - RenameItems: Exception thrown. Rename-Count=" + QtyRenamed + LastRenameInfo + "Exception MSG=" + e.Message);
            } finally 
            { 
                this.UseWaitCursor = false;
                pBar1.Visible = false;
                this.fastObjListView1.Enabled = true;
            }
        }
        private void RenameAll_Click(object sender, EventArgs e)
        {
            RenameItems();
        }

        private void RenameSelected_Click(object sender, EventArgs e)
        {
            RenameItems(WhichItems.SelectedItems);
        }

        private void RenameNotSelected_Click(object sender, EventArgs e)
        {
            RenameItems(WhichItems.UnselectedItems);
        }

        private void FormatCell(object sender, FormatCellEventArgs e)
        {
            if (e.ColumnIndex == 1) 
            {
                e.SubItem.Font = new Font(fastObjListView1.Font, FontStyle.Bold);
                e.SubItem.ForeColor = Color.DarkRed;
            }
        }

        private void FilterText_TextChanged(object sender, EventArgs e)
        {
            TimedFilter(this.fastObjListView1, ((System.Windows.Forms.TextBox)sender).Text, 0);
        }

        public void TimedFilter(BrightIdeasSoftware.ObjectListView olv, string txt)
        {
            TimedFilter(olv, txt, 0);
        }

        public void TimedFilter(BrightIdeasSoftware.ObjectListView olv, string txt, int matchKind)
        {
            TextMatchFilter filter = null;
            if (!String.IsNullOrEmpty(txt))
            {
                switch (matchKind)
                {
                    case 0:
                    default:
                        filter = TextMatchFilter.Contains(olv, txt);
                        break;
                    case 1:
                        filter = TextMatchFilter.Prefix(olv, txt);
                        break;
                    case 2:
                        filter = TextMatchFilter.Regex(olv, txt);
                        break;
                }
            }

            // Text highlighting requires at least a default renderer
            if (olv.DefaultRenderer == null)
                olv.DefaultRenderer = new HighlightTextRenderer(filter);

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            olv.AdditionalFilter = filter;
            //olv.Invalidate();
            stopWatch.Stop();

            IList objects = olv.Objects as IList;
            //if (objects == null)
            //    this.ToolStripStatus1 = prefixForNextSelectionMessage =
            //        String.Format("Filtered in {0}ms", stopWatch.ElapsedMilliseconds);
            //else
            //    this.ToolStripStatus1 = prefixForNextSelectionMessage =
            //        String.Format("Filtered {0} items down to {1} items in {2}ms",
            //                      objects.Count,
            //                      olv.Items.Count,
            //                      stopWatch.ElapsedMilliseconds);
        }

    }

    public class TranslateFilenamesFrm : TranslateFilenames
    {
        private Form1 _form1 = null;
        public TranslateFilenamesFrm(Form1 form1, TranslateFilenames_Options? options = null) : base(options)
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
            if (_options._silent)
                return;
            else if (outputlevel == OutPutLevel.ErrorLvl || outputlevel == OutPutLevel.PreProgressBar || outputlevel == OutPutLevel.PostProgressBar)
                Debug.WriteLine("ERROR: - " + message);
            else if (_options._verbose)
                Debug.WriteLine("VERBOSE: - " + message);
            else if (outputlevel == OutPutLevel.VerboseIfNotSilent)
                Debug.WriteLine("INFO: " + message);
        }

        /// <summary>
        /// Prints the help text to console.
        /// </summary>
        protected override void PrintHelp(bool AdvanceHelpPage)
        {
        }
    }
    public class FileRowDetails : INotifyPropertyChanged
    {
        public bool IsActive = true;

        public FileRowDetails(string fileName)
        {
            this._fileName = fileName;
        }

        public FileRowDetails(string fileName, string NewFilename, string SrcLanguage, 
            string ParentPath, string FileExt)
        {
            this.Name = fileName;
            this.NewFilename = NewFilename;
            this.SrcLanguage = SrcLanguage;
            this.ParentPath = ParentPath;
            this.FileExt = FileExt;
        }

        public FileRowDetails(FileRowDetails other)
        {
            this.Name = other.Name;
            this.NewFilename = other.NewFilename;
            this.SrcLanguage = other.SrcLanguage;
            this.ParentPath = other.ParentPath;
            this.FileExt = other.FileExt;
        }

        [OLVIgnore]
        public string ImageName
        {
            get
            {
                return "user";
            }
        }

        // Allows tests for properties.
        [OLVColumn(ImageAspectName = "ImageName")]
        public string Name
        {
            get { return this._fileName; }
            set
            {
                if (this._fileName == value) return;
                this._fileName = value;
                this.OnPropertyChanged("Name");
            }
        }
        private string _fileName;

        public string NewFilename
        {
            get { return this._newFileName; }
            set
            {
                if (this._newFileName == value) return;
                this._newFileName = value;
                this.OnPropertyChanged("NewFilename");
            }
        }
        private string _newFileName;


        public string SrcLanguage
        {
            get { return this._srcLanguage; }
            set
            {
                if (this._srcLanguage == value) return;
                this._srcLanguage = value;
                this.OnPropertyChanged("SrcLanguage");
            }
        }
        private string _srcLanguage;

        public string ParentPath
        {
            get { return this._filePath; }
            set
            {
                if (this._filePath == value) return;
                this._filePath = value;
                this.OnPropertyChanged("ParentPath");
            }
        }
        private string _filePath;


        public string FileExt
        {
            get { return this._fileExt; }
            set
            {
                if (this._fileExt == value) return;
                this._fileExt = value;
                this.OnPropertyChanged("FileExt");
            }
        }
        private string _fileExt;

        #region Implementation of INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

}