using GTranslate.Translators;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using SearchOption = System.IO.SearchOption;

namespace TranslateFilenamesCore
{
    public abstract class TranslateFilenames
    {
        #region class variables
        public const string _LONGPATHPREFIX = "\\\\?\\";
        private readonly object _lock = new object();
        private bool _exitAfterHelp = false;
        protected TranslateFilenames_Options _options { get; } = new TranslateFilenames_Options();
        private int _workerThreadCount = 0;
        private bool _initializeSuccess = false;
        private bool _isConsoleMode = false;
        protected string _undoListText;
        protected ConsoleProgressBar _progressBar;
        #endregion

        public TranslateFilenames(string[] args, bool exitAfterHelp = false, bool isConsoleMode = false)
        {
            _isConsoleMode = isConsoleMode;
            _initializeSuccess = Initialize(args, exitAfterHelp);
        }

        public TranslateFilenames(TranslateFilenames_Options options)
        {
            if (options != null)
                _options = options;
            _options._progressBarOutput = false;
            _initializeSuccess = Initialize();
        }

        public bool TranslatePath(string DataToTranslate = "")
        {
            if (_initializeSuccess == false)
            {
                OutPut("Class TranslateFilenames failed initialization.", OutPutLevel.ErrorLvl);
                return false;
            }
            if (DataToTranslate != null && DataToTranslate.Length > 0)
                _options._path = new string(DataToTranslate);
            if (_options._path == null || _options._path.Length == 0)
                _options._path = Directory.GetCurrentDirectory();

            Stopwatch sw = new Stopwatch();
            sw.Start();
            _options._filesToRename = Directory.GetFiles(_options._path, _options._fileType, _options._searchOption).ToList();
            if (_options._filesToRename.Count == 0)
            {
                OutPut("No files exist. Exiting.");
                OutPut("Early exit because no files exist in path \"" + _options._path + "\".", OutPutLevel.ErrorLvl);
                return false;
            }

            string RecursiveModeText = (_options._searchOption == SearchOption.AllDirectories) ? " recursively" : "";
            string FromLang = (_options._fromLang == null) ? "AutoDetect" : _options._fromLang;
            OutPut("Scanning path \"" + _options._path + "\"" + RecursiveModeText + " for files to translate.\n" +
                "[To-Lang:\"" + _options._targetLang + "\"] [From-Lang:\"" + FromLang + "\"] [Ext:\"" + _options._fileType + "\"] [AppOrg:" + _options._appendOrgName + "\"] [AppLang:\" + _options._appendLangName + \"\\\"] [Threads:" + _options._maxWorkerThreads + "\"] [Recursive:" + (_options._searchOption == SearchOption.AllDirectories) + "]\n"
                , OutPutLevel.PreProgressBar);
            InitializeProgressBar(_options._filesToRename.Count);
            if (_options._filesToRename.Count < ((_options._maxWorkerThreads * 4) + 11) && _options._filesPerTransReq == FilesPerTransReq.Auto)
                _options._filesPerTransReq = FilesPerTransReq.OnePerFile;
            string sourceText = "";
            List<ItemDetails> itemDetailsList = new List<ItemDetails>();
            List<Task> tasks = new List<Task>();
            int subCount = 0;
            foreach (var file in _options._filesToRename)
            {
                OutPut("Parsing file: " + file);
                var fileDetails = new ItemDetails()
                {
                    Name = Path.GetFileNameWithoutExtension(file),
                    Extension = Path.GetExtension(file),
                    ParrentPath = Path.GetDirectoryName(file)
                };

                if (fileDetails.Name.Length == 0)
                {
                    OutPut("Failed to get file name for  \"" + file + "\".", OutPutLevel.ErrorLvl);
                    continue;
                }

                if ((_options._filesPerTransReq == FilesPerTransReq.OnePerFile && subCount > 0)
                    ||
                    (sourceText.Length + fileDetails.Name.Length + 4 > _options._maximumTranslateDataLength))
                {
                    if (_options._progressBarOutput)
                        _progressBar.ShowProgress(_options._totalCount);
                    while (Volatile.Read(ref _workerThreadCount) > _options._maxWorkerThreads)
                        Task.WaitAny(tasks.ToArray());
                    OutPut("** Sending " + subCount.ToString() + " file names to translator.");
                    Task task = TranslateSourceText(sourceText, itemDetailsList);
                    tasks.Add(task);
                    subCount = 0;
                    sourceText = "";
                    itemDetailsList = new List<ItemDetails>();
                }

                itemDetailsList.Add(fileDetails);
                sourceText += fileDetails.Name;
                ++subCount;
                if (_options._filesToRename.Last() != file)
                    sourceText += "\n";
            }

            tasks.Add(TranslateSourceText(sourceText, itemDetailsList));
            Task.WaitAll(tasks.ToArray());
            if (_options._createUndoList)
                _options._writer.Close();
            sw.Stop();
            TaskComplete(_options._renameCount);
            OutPut("\nFound " + _options._renameCount + " files to rename out of " + _options._filesToRename.Count + ".", OutPutLevel.PostProgressBar);
            if (_options._renameErrorCount > 0)
                OutPut(_options._renameErrorCount + " Rename errors occurred and " + _options._renameSubErrorCount + " sub rename errors.", OutPutLevel.PostProgressBar);
            if (_options._errorCount > 0)
                OutPut(_options._errorCount + " unknown errors occurred.", OutPutLevel.PostProgressBar);
            OutPut("Translation completed. Elapse time = " + sw.Elapsed, OutPutLevel.PostProgressBar);
            if (_options._waitKeyPress && _isConsoleMode)
            {
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
            return true;
        }

        public class TranslateFilenames_Options
        {
            public Assembly _assembly { get; set; } = Assembly.GetExecutingAssembly();
            public string _fromLang { get; set; } = null;
            public string _targetLang { get; set; } = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            public string _path { get; set; } = Directory.GetCurrentDirectory();
            public bool _appendOrgName { get; set; } = false;
            public int _appendLangName { get; set; } = 0;
            public bool _noRename { get; set; } = false;
            public bool _verbose { get; set; } = false;
            public bool _dottedOutput { get; set; } = false;
            public bool _progressBarOutput { get; set; } = true;
            public bool _silent { get; set; } = false;
            public FilesPerTransReq _filesPerTransReq { get; set; } = FilesPerTransReq.Auto;
            public bool _waitKeyPress { get; set; } = false;
            public SearchOption _searchOption { get; set; } = SearchOption.TopDirectoryOnly;
            public string _fileType { get; set; } = "*";
            public bool _createUndoList { get; set; } = false;
            public string _undoListFileName { get; set; } = "";
            public string _longPathPrefix { get; set; } = "";
            public string _programCommandLineName { get; set; } = "";
            public string _programVersion { get; set; } = "";
            public string _programFileVersion { get; set; } = "";
            public string _programDescription { get; set; } = "";
            public StreamWriter _writer { get; set; } = null;
            public int _postProgressBarPos { get; set; } = 8;
            public int _totalCount { get; set; } = 0;
            public int _renameCount { get; set; } = 0;
            public int _renameErrorCount { get; set; } = 0;
            public int _renameSubErrorCount { get; set; } = 0;
            public int _errorCount { get; set; } = 0;
            public int _maximumTranslateDataLength { get; set; } = 10000; // Fails at 10500
            public int _maxWorkerThreads { get; set; } = Math.Max(Environment.ProcessorCount, 4);
            public List<string> _filesToRename { get; set; } = new List<string>();
        }

        public class ItemDetails
        {
            public string Name { get; set; }
            public string Extension { get; set; }
            public string Translation { get; set; }
            public string ParrentPath { get; set; }
            public string SourceLang { get; set; }
        }
        public enum OutPutLevel
        {
            VerboseIfNotSilent,
            NormalLvl,
            PreProgressBar,
            PostProgressBar,
            ErrorLvl
        }

        public enum FilesPerTransReq
        {
            Auto,
            OnePerFile,
            Many
        }

        /// <summary>
        /// Parses commandline arguments.
        /// </summary>
        /// <param name="args">Commandline argument list</param>
        public void ParseArgs(string[] args)
        {
            if (args == null) return;
            bool AdvanceHelpPage = false;
            int StartAt = 0;
            if (args.Length != 0 && !args[0].StartsWith("-") && !args[0].Equals("/?") && !args[0].Equals("?")) _options._path = args[StartAt++];

            for (int i = StartAt; i < args.Length; i++)
            {
                string arg = args[i].StartsWith("--") ? args[i].Substring(1).ToLower() : args[i].ToLower();
                switch (arg)
                {
                    case "-fromlang":
                    case "-f":
                        if ((i + 1) < args.Length && !args[i + 1].StartsWith("-") && args[i + 1].Length == 2 && Char.IsLetter(args[i + 1][0]) && Char.IsLetter(args[i + 1][1])) _options._fromLang = args[++i].ToLower();
                        break;
                    case "-tolang":
                    case "-t":
                        if ((i + 1) < args.Length && !args[i + 1].StartsWith("-") && args[i + 1].Length == 2 && Char.IsLetter(args[i + 1][0]) && Char.IsLetter(args[i + 1][1])) _options._targetLang = args[++i].ToLower();
                        break;
                    case "-lentrans":
                    case "-n":
                        if ((i + 1) < args.Length && !args[i + 1].StartsWith("-") && Int32.Parse(args[i + 1]) > 250) _options._maximumTranslateDataLength = Int32.Parse(args[++i]);
                        break;
                    case "-maxthrds":
                    case "-m":
                        if ((i + 1) < args.Length && !args[i + 1].StartsWith("-") && Int32.Parse(args[i + 1]) > 1) _options._maxWorkerThreads = Int32.Parse(args[++i]);
                        break;
                    case "-createlist":
                    case "-c":
                        _options._createUndoList = true;
                        if ((i + 1) < args.Length && !args[i + 1].StartsWith("-")) _options._undoListFileName = args[++i];
                        break;
                    //case "-undo":
                    //case "-u":
                    //    _options._undoFileRename = true;
                    //    if ((i + 1) < args.Length && !args[i + 1].StartsWith("-")) _options._undoListFileName = args[++i];
                    //    break;
                    case "-ext":
                    case "-e":
                        if ((i + 1) < args.Length && !args[i + 1].StartsWith("-")) _options._fileType = args[++i].ToLower();
                        break;
                    case "-appendorigname":
                    case "-a":
                        _options._appendOrgName = true;
                        break;
                    case "-lang":
                    case "-l":
                        if ((i + 1) < args.Length && !args[i + 1].StartsWith("-") && Int32.Parse(args[i + 1]) > 1) _options._appendLangName = Int32.Parse(args[++i]);
                        else
                            _options._appendLangName = 9;
                        break;
                    case "-norename":
                    case "-x":
                        _options._noRename = true;
                        break;
                    case "-recursive":
                    case "-r":
                        _options._searchOption = SearchOption.AllDirectories;
                        break;
                    case "-one":
                    case "-0":
                    case "-1":
                    case "-o":
                        _options._filesPerTransReq = FilesPerTransReq.OnePerFile;
                        break;
                    case "-longpath":
                    case "-g":
                        _options._longPathPrefix = _LONGPATHPREFIX;
                        break;
                    case "-verbose":
                    case "-v":
                        _options._verbose = true;
                        break;
                    case "-itemize":
                    case "-i":
                        _options._progressBarOutput = false;
                        break;
                    case "-dotoutput":
                    case "-d":
                        _options._dottedOutput = true;
                        break;
                    case "-silent":
                    case "-s":
                        _options._silent = true;
                        break;
                    case "-presskey":
                    case "-p":
                        _options._waitKeyPress = true;
                        break;
                    case "-?":
                    case "/?":
                    case "?":
                        AdvanceHelpPage = true;
                        goto case "-h";
                    case "-help":
                    case "-h":
                        PrintHelp(AdvanceHelpPage);
                        if (_exitAfterHelp)
                            Environment.Exit(0);
                        return;
                    default:
                        break;
                }
            }

            if (_options._dottedOutput) _options._progressBarOutput = _options._verbose = _options._silent = false;
            if (_options._verbose) _options._progressBarOutput = _options._silent = false;
            if (_options._silent) _options._progressBarOutput = false;
            if (_options._appendLangName > 0) _options._filesPerTransReq = FilesPerTransReq.OnePerFile;
        }


        /// <summary>
        /// Initialize 
        /// </summary>
        /// <param name="args">The list of commandline arguments</param>
        public bool Initialize(string[] args = null, bool exitAfterHelp = false)
        {
            if (_isConsoleMode)
            {
                _options._programCommandLineName = _options._assembly.GetCustomAttribute<AssemblyTitleAttribute>().Title;
                _options._programVersion = _options._assembly.GetName().Version.ToString();
                _options._programFileVersion = _options._assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version.ToString();
                var Description = _options._assembly.GetCustomAttribute<AssemblyDescriptionAttribute>();
                if (Description != null)
                    _options._programDescription = Description.Description;
                else
                    _options._programDescription = new string("File name language translator");
                Console.OutputEncoding = System.Text.Encoding.Unicode;
            }

            ParseArgs(args);
            OutPut("Opening path: " + _options._path, OutPutLevel.VerboseIfNotSilent);
            if (!Directory.Exists(_options._path))
            {
                OutPut("Path does not exist. Exiting.", OutPutLevel.ErrorLvl);
                return false;
            }
            if (_options._undoListFileName.Length == 0)
                _options._undoListFileName = Path.Combine(_options._path, "undo_rename.bat");
            if (_options._createUndoList)
            {
                if (File.Exists(_options._undoListFileName))
                    File.Delete(_options._undoListFileName);
                try
                {
                    _options._writer = new StreamWriter(_options._undoListFileName);
                }
                catch (Exception e)
                {
                    OutPut("Early exit because could not create undo file at path \"" + _options._undoListFileName + "\".\nErr Msg: \"" + e.Message + "\"", OutPutLevel.ErrorLvl);
                    ++_options._errorCount;
                    return false;
                }
            }
            return true;
        }

        public string GetAppendedName(ItemDetails fileDetail)
        {
            return (_options._appendOrgName ? " (" + fileDetail.Name + ")" : "") + ((_options._appendLangName > 0 && fileDetail.SourceLang != null && fileDetail.SourceLang.Length > 0) ? fileDetail.SourceLang : "");
        }

        /// <summary>
        /// Translator Wrapper
        /// </summary>
        /// <param name="sourceText">Consolidated source text to translate</param>
        /// <param name="itemDetailsList">List of files to translate</param>
        public async Task TranslateSourceText(string sourceText, List<ItemDetails> itemDetailsList)
        {
            try
            {
                try
                {
                    Interlocked.Increment(ref _workerThreadCount);
                    await GTranslate_TranslateSourceText(sourceText, itemDetailsList);
                }
                catch (Exception ee)
                {
                    OutPut(ee.Message, OutPutLevel.ErrorLvl);
                    ++_options._errorCount;
                    throw new Exception(ee.Message);
                }
                finally
                {
                    Interlocked.Decrement(ref _workerThreadCount);
                }

                _undoListText = new string("");
                foreach (ItemDetails fileDetail in itemDetailsList)
                {
                    ++_options._totalCount;
                    if (fileDetail.Name == null || fileDetail.Name.Length == 0 || fileDetail.Translation == null || fileDetail.Translation.Length == 0)
                        continue;
                    try
                    {
                        string oldFileName = Path.Combine(fileDetail.ParrentPath, fileDetail.Name + fileDetail.Extension);
                        string appendedName = GetAppendedName(fileDetail);
                        fileDetail.Translation = FileDetailsReplaceIllegalFileNameChar(fileDetail);// Replaces illegal windows file name characters with alternative characters
                        string newFileName = Path.Combine(fileDetail.ParrentPath, fileDetail.Translation + appendedName + fileDetail.Extension);

                        CheckIfRenameNeeded(fileDetail, oldFileName, newFileName);
                    }
                    catch (Exception eeeee)
                    {
                        OutPut(eeeee.Message, OutPutLevel.ErrorLvl);
                        ++_options._errorCount;
                        throw new Exception(eeeee.Message);
                    }
                }

                if (_undoListText.Length > 0)
                {
                    if (_options._createUndoList)
                    {
                        OutPut("Creating undo list file at \"" + _options._undoListFileName + "\"");
                        Monitor.Enter(_lock);
                        try
                        {
                            Task write_result = _options._writer.WriteAsync(_undoListText);
                            write_result.Wait();
                            //while (!write_result.IsCompleted)
                            //    Thread.Sleep(10);
                            Monitor.Pulse(_lock);
                        }
                        finally
                        {
                            Monitor.Exit(_lock);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                OutPut(e.Message, OutPutLevel.ErrorLvl);
                ++_options._errorCount;
            }
        }

        /// <summary>
        /// Sets Translation variable in each ItemDetails
        /// </summary>
        /// <param name="oldFileName">Original file name</param>
        /// <param name="newFileName">Translated file name</param>
        /// <param name="Ext">file name extension</param>
        public string RenameFileWithAppendedIndex(string oldFileName, string newFileName, string Ext)
        {
            for (int i = 0; i < 199; ++i)
            {
                string newIndexFileName = newFileName + " (" + i + ")" + Ext;
                if (File.Exists(newIndexFileName))
                    continue;
                try
                {
                    File.Move(oldFileName, newIndexFileName);
                    return newIndexFileName;
                }
                catch (Exception e)
                {
                    int lastErrCode = Marshal.GetLastWin32Error();
                    ++_options._renameSubErrorCount;
                    OutPut("Could not rename file \"" + oldFileName + "\" to \"" + newIndexFileName + "\"\nErr Msg: " + e.Message + "\nErrCode: " + lastErrCode, OutPutLevel.VerboseIfNotSilent);
                }
            }
            return null;
        }

        /// <summary>
        /// Rename file 
        /// </summary>
        /// <param name="itemDetailsList">File detail to rename</param>
        /// <param name="oldFileName">Original file name</param>
        /// <param name="newFileName">Translated file name</param>
        /// <param name="CheckIfOrgFileExist">If true, checks if the original file still exists before rename</param>
        /// <param name="UseIndexIfNeeded">If true, and if file with same target name exists, then rename using appended index number</param>
        public void RenameFile(ItemDetails fileDetail, string oldFileName, string newFileName, bool CheckIfOrgFileExist = false, bool UseIndexIfNeeded = true)
        {
            try
            {
                if (CheckIfOrgFileExist && !File.Exists(_options._longPathPrefix + oldFileName))
                    return;
                File.Move(_options._longPathPrefix + oldFileName, _options._longPathPrefix + newFileName);
                _undoListText += newFileName + "|" + oldFileName + "\n";
                OutPut("Renamed file \"" + oldFileName + "\" to \"" + newFileName + "\"");
            }
            catch (Exception exc)
            {
                int lastErrCode = Marshal.GetLastWin32Error();
                string appendedName = GetAppendedName(fileDetail);
                string newIndexFileName = (UseIndexIfNeeded && (lastErrCode == 183 || (exc != null && exc.Message != null && exc.Message.StartsWith("Cannot create a file when that file already exists")))) ? RenameFileWithAppendedIndex(_options._longPathPrefix + oldFileName, _options._longPathPrefix + Path.Combine(fileDetail.ParrentPath, fileDetail.Translation + appendedName), fileDetail.Extension) : null;
                if (newIndexFileName != null)
                {
                    _undoListText += newIndexFileName + "|" + oldFileName + "\n";
                    OutPut("Renamed file \"" + oldFileName + "\" to \"" + newIndexFileName + "\"");
                }
                else
                {
                    --_options._renameCount;
                    ++_options._renameErrorCount;
                    OutPut("Could not rename file \"" + oldFileName + "\" to \"" + newIndexFileName + "\"\nErr Msg: " + exc.Message + "\nErrNo: " + lastErrCode, OutPutLevel.ErrorLvl);
                }
            }
        }

        protected virtual void RenameRequired(ItemDetails fileDetail, string oldFileName, string newFileName)
        {
            RenameFile(fileDetail, oldFileName, newFileName);
        }
        protected virtual void CheckIfRenameNeeded(ItemDetails fileDetail, string oldFileName, string newFileName)
        {
            if (fileDetail.Translation.Equals(fileDetail.Name))
                OutPut("Skipping file rename \"" + fileDetail.Name + "\" to \"" + fileDetail.Translation + "\"");
            else
            {
                ++_options._renameCount;
                if (_options._noRename)
                {
                    _undoListText += newFileName + "\n" + oldFileName + "\n";
                    OutPut("Rename candidate \"" + oldFileName + "\" to-> \"" + newFileName + "\"");
                }
                else
                    RenameRequired(fileDetail, oldFileName, newFileName);
            }
        }


        /// <summary>
        /// Sets Translation variable in each ItemDetails
        /// </summary>
        /// <param name="itemDetailsList">List of files to translate</param>
        /// <param name="TranslatedFileNames_str">Translated text</param>
        /// <param name="SourceLanguage">2 letter source language code</param>
        private void SetTranslation(List<ItemDetails> itemDetailsList, string TranslatedFileNames_str, string SourceLanguage)
        {
            string[] TranslatedFileNames = TranslatedFileNames_str.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            int fileDetailsIndex = 0;
            foreach (string fileName in TranslatedFileNames)
            {
                itemDetailsList[fileDetailsIndex].Translation = fileName.Contains("\\n") ? fileName.Substring(0, fileName.Length - 2) : fileName;
                fileDetailsIndex++;
            }

            if (fileDetailsIndex == 1 && itemDetailsList.Count == 1 && SourceLanguage.Length > 0)
                itemDetailsList[0].SourceLang = SourceLanguage;
        }


        /// <summary>
        /// Translate via GTranslate
        /// </summary>
        /// <param name="sourceText">Source text to translate</param>
        /// <param name="itemDetailsList">List of files to translate</param>
        private async Task GTranslate_TranslateSourceText(string sourceText, List<ItemDetails> itemDetailsList)
        {
            var translator = new AggregateTranslator(); // GoogleTranslator2();// AggregateTranslator(); //
            try
            {
                GTranslate.Results.ITranslationResult result = await translator.TranslateAsync(sourceText, _options._targetLang, _options._fromLang);
                //GTranslate.Language SourceLanguage = new GTranslate.Language(result.SourceLanguage.Name);
                OutPut($"Translated text: {result.Translation}");
                OutPut($"Source Language: {result.SourceLanguage}");
                OutPut($"Target Language: {result.TargetLanguage}");
                OutPut($"Service: {result.Service}");
                string SrcLang = "";
                if (result.SourceLanguage.Name != null && result.SourceLanguage.Name.Length > 0)
                {
                    switch (_options._appendLangName)
                    {
                        case 2:
                            SrcLang = "_options._[" + result.SourceLanguage.ISO6391 + "]";
                            break;
                        case 20:
                            SrcLang = "_(" + result.SourceLanguage.ISO6391 + ")";
                            break;
                        case 21:
                            SrcLang = "_" + result.SourceLanguage.ISO6391;
                            break;
                        case 22:
                            SrcLang = result.SourceLanguage.ISO6391;
                            break;
                        case 3:
                            SrcLang = "_[" + result.SourceLanguage.ISO6393 + "]";
                            break;
                        case 31:
                            SrcLang = "_(" + result.SourceLanguage.ISO6393 + ")";
                            break;
                        case 32:
                            SrcLang = "_" + result.SourceLanguage.ISO6393;
                            break;
                        case 33:
                            SrcLang = result.SourceLanguage.ISO6393;
                            break;
                        //case 8:
                        //    SrcLang = "_[" + SourceLanguage.NativeName + "]";
                        //    break;
                        //case 81:
                        //    SrcLang = "_(" + SourceLanguage.NativeName + ")";
                        //    break;
                        //case 82:
                        //    SrcLang = "_" + SourceLanguage.NativeName;
                        //    break;
                        //case 83:
                        //    SrcLang = SourceLanguage.NativeName;
                        //    break;
                        case 9:
                            SrcLang = "_[" + result.SourceLanguage.Name + "]";
                            break;
                        case 91:
                            SrcLang = "_(" + result.SourceLanguage.Name + ")";
                            break;
                        case 92:
                            SrcLang = "_" + result.SourceLanguage.Name;
                            break;
                        case 93:
                            SrcLang = result.SourceLanguage.Name;
                            break;
                        default:
                            SrcLang = "_[" + result.SourceLanguage.Name + "]";
                            break;
                    }
                }
                SetTranslation(itemDetailsList, result.Translation, SrcLang);

            }
            catch (Exception e)
            {
                OutPut(e.Message, OutPutLevel.ErrorLvl);
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// replaces illegal filename characters with alternatives
        /// </summary>
        /// <param name="itemDetailsList">File detail to translate</param>
        public string FileDetailsReplaceIllegalFileNameChar(ItemDetails fileDetail)
        {
            try
            {
                // replaces illegal filename characters with alternatives
                fileDetail.Translation = fileDetail.Translation.Replace('\\', '-');
                fileDetail.Translation = fileDetail.Translation.Replace('/', '-');
                fileDetail.Translation = fileDetail.Translation.Replace(':', '-');
                fileDetail.Translation = fileDetail.Translation.Replace('*', ' ');
                fileDetail.Translation = fileDetail.Translation.Replace('?', ' ');
                fileDetail.Translation = fileDetail.Translation.Replace('\"', '\'');
                fileDetail.Translation = fileDetail.Translation.Replace('<', '[');
                fileDetail.Translation = fileDetail.Translation.Replace('>', ']');
                fileDetail.Translation = fileDetail.Translation.Replace('|', '-');
                fileDetail.Translation = fileDetail.Translation.Replace('\t', ' ');
                fileDetail.Translation = fileDetail.Translation.Replace('\n', ' ');
            }
            catch (Exception e)
            {
                OutPut(e.Message, OutPutLevel.ErrorLvl);
                throw new Exception(e.Message);
            }

            return fileDetail.Translation;
        }

        /// <summary>
        /// Allows derived class to get details on how many files will be checked for language translation candidates
        /// </summary>
        /// <param name="QtyFilesToCheck">Quantity of files to check</param>
        protected virtual void InitializeProgressBar(int QtyFilesToCheck)
        {
            _progressBar = (_options._progressBarOutput && _isConsoleMode) ? new ConsoleProgressBar(QtyFilesToCheck, "Files processed") : null;
        }

        /// <summary>
        /// Allows derived class to get details on how many files will be checked for language translation candidates
        /// </summary>
        /// <param name="QtyFilesRenameCandidate">Quantity of files found that are rename candidates</param>
        protected virtual void TaskComplete(int QtyFilesRenameCandidate)
        {
            if (_options._progressBarOutput && _isConsoleMode)
            {
                _progressBar.ShowProgress(_options._filesToRename.Count);
                Console.SetCursorPosition(0, 6);
            }
        }

        /// <summary>
        /// Sends logging or status data to derived function
        /// </summary>
        /// <param name="message">String to print</param>
        protected abstract void OutPut(string message, OutPutLevel outputlevel = OutPutLevel.NormalLvl);

        /// <summary>
        /// Have derived method display help
        /// </summary>
        protected abstract void PrintHelp(bool AdvanceHelpPage);
    }


    public class TranslateFilenamesSimple : TranslateFilenames
    {
        public TranslateFilenamesSimple(string[] args, bool exitAfterHelp = false, bool isConsoleMode = false) : base(args, exitAfterHelp, isConsoleMode)
        {
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