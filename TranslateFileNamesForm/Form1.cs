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
        #region class variables
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
        #endregion

        public Form1()
        {
            InitializeComponent();
            UpdateListDelegate = new UpdateList(UpdateListMethod);
            SetupProgressBarDelegate = new SetupProgressBarDelegateType(UpdateProgressBar);
            fastObjListView1.FormatCell += FormatCell;
            ClearItemsOnList();
            fastObjListView1.ColumnsNotEditable = new List<int>();
            fastObjListView1.ColumnsNotEditable.Add(0);
            fastObjListView1.ColumnsNotEditable.Add(2);
            fastObjListView1.ColumnsNotEditable.Add(3);
            fastObjListView1.ColumnsNotEditable.Add(4);
            TranslateFilenames_Options options = new TranslateFilenames_Options();
            MaxThread.Text = options._maxWorkerThreads.ToString();
            MaxTranslateLen.Text    = options._maximumTranslateDataLength.ToString();
            TargetLanguage.Text = options._targetLang.ToString();
#if !DEBUG
            TestButton.Enabled = false;
            TestButton.Visible = false;
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
                UseWaitCursor = false;
                pBar1.Visible = false;
                fastObjListView1.Enabled = true;
                ClearList.Visible = true;
                RenameAll.Visible = true;
                RenameSelected.Visible = true;
                RenameNotSelected.Visible = true;
                RenameChecked.Visible = true;
                RenameUnchecked.Visible = true;
                FilterText.Visible = true;
                labelFilter.Visible = true;
                RefreshList.Visible = true;
            }
            else
                SetupProgressBar(QtyFilesToCheck);
        }

        private void ClearItemsOnList()
        {
            fastObjListView1.ClearObjects();
            QtyFilesToCheck = 0;
            QtyFilesRenameCandidate = -1;
            pBar1.Visible = false;
            //ClearList.di
            fastObjListView1.Visible = false;
            ClearList.Visible = false;
            RenameAll.Visible = false;
            RenameSelected.Visible = false;
            RenameNotSelected.Visible = false;
            RenameChecked.Visible = false;
            RenameUnchecked.Visible = false;
            FilterText.Visible = false;
            labelFilter.Visible = false;
            RefreshList.Visible = false;
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
            if (TargetedPath.Text.Length > 0)
                folderBrowserDialog1.InitialDirectory = TargetedPath.Text;
            // Default to the My Documents folder.
            folderBrowserDialog1.RootFolder = Environment.SpecialFolder.Personal;
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (!AddFilesToList(folderBrowserDialog1.SelectedPath))
                    MessageBox.Show("Error: Folder does NOT exist!\n" + folderBrowserDialog1.SelectedPath,
                                  "Folder NOT Exist!",
                                  MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
            }
            else
                ClearItemsOnList();
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

        private bool AddFilesToList(string targetPath)
        {
            Debug.WriteLine("Info: AddFilesToList Entering.");
            if (!Directory.Exists(targetPath))
                return false;
            _path = targetPath;
            TargetedPath.Text = targetPath;
            TargetedPath.Update();
            TranslateFilenames_Options options = new TranslateFilenames_Options();
            if (TargetLanguage.Text.Length == 2)
                options._targetLang = TargetLanguage.Text;
            if (SourceLanguage.Text.Length == 2)
                options._targetLang = SourceLanguage.Text;
            if (checkBoxRecursive.Checked)
                options._searchOption = SearchOption.AllDirectories;
            if (checkBoxLongPathSupport.Checked)
                options._longPathPrefix = TranslateFilenames._LONGPATHPREFIX;
            if (MaxThread.Text.Length > 0 && Convert.ToInt32(MaxThread.Text) > 0)
                options._maxWorkerThreads = Convert.ToInt32(MaxThread.Text);
            if (MaxTranslateLen.Text.Length > 0 && Convert.ToInt32(MaxTranslateLen.Text) > 0)
                options._maximumTranslateDataLength = Convert.ToInt32(MaxTranslateLen.Text);
            if (FileType.Text.Length > 0)
                options._fileType = FileType.Text;
            if (checkBoxAppendLanguageName.Checked)  // ToDo: Change this to a combo box listing items in GTranslate_TranslateSourceText **switch**
                options._appendLangName = 9;
            if (checkBoxAppendOrginalName.Checked)
                options._appendOrgName = true;
            if (comboBoxFilesPerTransReq.SelectedIndex > 0)
            {
                if (comboBoxFilesPerTransReq.SelectedIndex == 1)
                    options._filesPerTransReq = FilesPerTransReq.OnePerFile;
                else if (comboBoxFilesPerTransReq.SelectedIndex == 2)
                    options._filesPerTransReq = FilesPerTransReq.Many;
            }
            _translateFilenames = new TranslateFilenamesFrm(this, options);

            ClearItemsOnList();
            fastObjListView1.Visible = true;
            SetupProgressBar(3);
            pBar1.PerformStep();
            pBar1.PerformStep();
            UseWaitCursor = true;
            fastObjListView1.Enabled = false;
            Task.Run(() => AddFilesToList_Task(targetPath));
            Debug.WriteLine("Info: AddFilesToList Exiting.");
            return true;
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
                    fastObjListView1.AddObjects(l);
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
            SelectedItemsAlways,
            UnselectedItemsAlways,
            CheckedItems,
            UncheckedItems
        }
        private void RenameItems(WhichItems whichItems = WhichItems.AllItems)
        {
            int QtyRenamed = 0;
            string OrgName = "", LastRenameSuccess = "";
            UseWaitCursor = true;
            fastObjListView1.Enabled = false;
            try
            {
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
                else if (whichItems == WhichItems.SelectedItemsAlways)
                    whichItems = WhichItems.SelectedItems;
                else if (whichItems == WhichItems.UnselectedItemsAlways)
                    whichItems = WhichItems.UnselectedItems;
                int PrgBarCount;
                int Count;
                switch(whichItems)
                {
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
                    case WhichItems.AllItems:
                    default:
                        PrgBarCount = iCount;
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
                UseWaitCursor = false;
                pBar1.Visible = false;
                fastObjListView1.Enabled = true;
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

        private void RenameChecked_Click(object sender, EventArgs e)
        {
            RenameItems(WhichItems.CheckedItems);
        }

        private void RenameUnchecked_Click(object sender, EventArgs e)
        {
            RenameItems(WhichItems.UncheckedItems);
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
            TimedFilter(fastObjListView1, ((System.Windows.Forms.TextBox)sender).Text, 0);
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
            stopWatch.Stop();
        }

        private void TargetedPath_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            // Check if User Presses Enter
            if (e.KeyCode == System.Windows.Forms.Keys.Return)
                AddFilesToList(((System.Windows.Forms.TextBox)sender).Text);
        }

        private void RefreshList_Click(object sender, EventArgs e)
        {
            AddFilesToList(_path);
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
            _fileName = fileName;
        }

        public FileRowDetails(string fileName, string NewFilename, string SrcLanguage, 
            string ParentPath, string FileExt)
        {
            Name = fileName;
            NewFilename = NewFilename;
            SrcLanguage = SrcLanguage;
            ParentPath = ParentPath;
            FileExt = FileExt;
        }

        public FileRowDetails(FileRowDetails other)
        {
            Name = other.Name;
            NewFilename = other.NewFilename;
            SrcLanguage = other.SrcLanguage;
            ParentPath = other.ParentPath;
            FileExt = other.FileExt;
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
            get { return _fileName; }
            set
            {
                if (_fileName == value) return;
                _fileName = value;
                OnPropertyChanged("Name");
            }
        }
        private string _fileName;

        public string NewFilename
        {
            get { return _newFileName; }
            set
            {
                if (_newFileName == value) return;
                _newFileName = value;
                OnPropertyChanged("NewFilename");
            }
        }
        private string _newFileName;


        public string SrcLanguage
        {
            get { return _srcLanguage; }
            set
            {
                if (_srcLanguage == value) return;
                _srcLanguage = value;
                OnPropertyChanged("SrcLanguage");
            }
        }
        private string _srcLanguage;

        public string ParentPath
        {
            get { return _filePath; }
            set
            {
                if (_filePath == value) return;
                _filePath = value;
                OnPropertyChanged("ParentPath");
            }
        }
        private string _filePath;


        public string FileExt
        {
            get { return _fileExt; }
            set
            {
                if (_fileExt == value) return;
                _fileExt = value;
                OnPropertyChanged("FileExt");
            }
        }
        private string _fileExt;

        #region Implementation of INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

}