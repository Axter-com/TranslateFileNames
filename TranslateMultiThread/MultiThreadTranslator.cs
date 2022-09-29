using GTranslate.Translators;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using SearchOption = System.IO.SearchOption;

namespace TranslateMultiThread
{
    public abstract class MultiThreadTranslator
    {
        #region class variables
        public const string _LONGPATHPREFIX = "\\\\?\\";
        private readonly object _lock = new object();
        private bool _exitAfterHelp = false;
        protected MultiThreadTranslator_Options _options { get; } = new MultiThreadTranslator_Options();
        private int _workerThreadCount = 0;
        private bool _initializeSuccess = false;
        private bool _isConsoleMode = false;
        protected string _undoListText;
        #endregion

        public MultiThreadTranslator(string[] args, bool exitAfterHelp = false, bool isConsoleMode = false)
        {
            _isConsoleMode = isConsoleMode;
            _initializeSuccess = Initialize(args, exitAfterHelp);
        }

        public MultiThreadTranslator(MultiThreadTranslator_Options options)
        {
            if (options != null)
                _options = options;
            _options._progressBarOutput = false;
            _initializeSuccess = Initialize();
        }

        public bool TranslateData(List<string>? DataToTranslate = null)
        {
            if (_initializeSuccess == false)
            {
                OutPut("Class MultiThreadTranslator failed initialization.", OutPutLevel.ErrorLvl);
                return false;
            }
            if (DataToTranslate != null && DataToTranslate.Count > 0)
                _options._itemsToRename = DataToTranslate;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            if (_options._itemsToRename.Count == 0)
            {
                OutPut("No items exist in list. Exiting.");
                OutPut("Early exit because no items exist.", OutPutLevel.ErrorLvl);
                return false;
            }

            string FromLang = (_options._fromLang == null) ? "AutoDetect" : _options._fromLang;
            OutPut("Starting translation with settings:\n" +
                "[To-Lang:\"" + _options._targetLang + "\"] [From-Lang:\"" + FromLang + "\"] [AppOrg:" + _options._appendOrgName + "\"] [AppLang:\" + _options._appendLangName + \"\\\"] [Threads:" + _options._maxWorkerThreads + "\"]\n"
                , OutPutLevel.PreProgressBar);
            InitializeProgressBar(_options._itemsToRename.Count);
            if (_options._itemsToRename.Count < ((_options._maxWorkerThreads * 4) + 11) && _options._itemsPerTransReq == ItemsPerTransReq.Auto)
                _options._itemsPerTransReq = ItemsPerTransReq.OnePerItem;
            string sourceText = "";
            List<ItemDetails> itemDetailsList = new List<ItemDetails>();
            List<Task> tasks = new List<Task>();
            int subCount = 0;
            foreach (var item in _options._itemsToRename)
            {
                OutPut("Parsing item: " + item);
                var itemDetails = new ItemDetails()
                {
                    OriginalText = item
                };

                if (itemDetails.OriginalText.Length == 0)
                {
                    OutPut("Failed to get item string  \"" + item + "\".", OutPutLevel.ErrorLvl);
                    continue;
                }

                if ((_options._itemsPerTransReq == ItemsPerTransReq.OnePerItem && subCount > 0)
                    ||
                    (sourceText.Length + itemDetails.OriginalText.Length + 4 > _options._maximumTranslateDataLength))
                {
                    if (_options._progressBarOutput)
                        ShowProgress(_options._totalCount);
                    while (Volatile.Read(ref _workerThreadCount) > _options._maxWorkerThreads)
                        Task.WaitAny(tasks.ToArray());
                    OutPut("** Sending " + subCount.ToString() + " item names to translator.");
                    Task task = TranslateSourceText(sourceText, itemDetailsList);
                    tasks.Add(task);
                    subCount = 0;
                    sourceText = "";
                    itemDetailsList = new List<ItemDetails>();
                }

                itemDetailsList.Add(itemDetails);
                sourceText += itemDetails.OriginalText;
                ++subCount;
                if (_options._itemsToRename.Last() != item)
                    sourceText += "\n";
            }

            tasks.Add(TranslateSourceText(sourceText, itemDetailsList));
            Task.WaitAll(tasks.ToArray());
            if (_options._createUndoList)
                _options._writer.Close();
            sw.Stop();
            TaskComplete(_options._renameCount);
            OutPut("\nFound " + _options._renameCount + " items to rename out of " + _options._itemsToRename.Count + ".", OutPutLevel.PostProgressBar);
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

        /// <summary>
        /// Translator Wrapper
        /// </summary>
        /// <param name="sourceText">Consolidated source text to translate</param>
        /// <param name="itemDetailsList">List of items to translate</param>
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
                foreach (ItemDetails itemDetails in itemDetailsList)
                {
                    ++_options._totalCount;
                    if (itemDetails.OriginalText == null || itemDetails.OriginalText.Length == 0 || itemDetails.Translation == null || itemDetails.Translation.Length == 0)
                        continue;
                    try
                    {
                        itemDetails.Translation = ItemDetailsReplaceInvalidChar(itemDetails) + GetAppendedName(itemDetails);
                        _options._ItemsTranslated.Add(itemDetails);
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
                        OutPut("Creating undo list item at \"" + _options._undoListFileName + "\"");
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

        public class MultiThreadTranslator_Options
        {
            public Assembly _assembly { get; set; } = Assembly.GetExecutingAssembly();
            public string _fromLang { get; set; } = null;
            public string _targetLang { get; set; } = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            public bool _appendOrgName { get; set; } = false;
            public int _appendLangName { get; set; } = 0;
            public bool _verbose { get; set; } = false;
            public bool _dottedOutput { get; set; } = false;
            public bool _progressBarOutput { get; set; } = true;
            public bool _silent { get; set; } = false;
            public ItemsPerTransReq _itemsPerTransReq { get; set; } = ItemsPerTransReq.Auto;
            public bool _waitKeyPress { get; set; } = false;
            public bool _createUndoList { get; set; } = false;
            public string _undoListFileName { get; set; } = "";
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
            public List<string> _itemsToRename { get; set; } = new List<string>();
            public List<ItemDetails> _ItemsTranslated { get; set; } = new List<ItemDetails>();
        }
        public enum OutPutLevel
        {
            VerboseIfNotSilent,
            NormalLvl,
            PreProgressBar,
            PostProgressBar,
            ErrorLvl
        }

        public enum ItemsPerTransReq
        {
            Auto,
            OnePerItem,
            Many
        }
        /// <summary>
        /// Parses command line arguments.
        /// </summary>
        /// <param name="args">Command line argument list</param>
        public void ParseArgs(string[] args)
        {
            if (args == null) return;
            bool AdvanceHelpPage = false;
            int StartAt = 0;

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
                    case "-one":
                    case "-0":
                    case "-1":
                    case "-o":
                        _options._itemsPerTransReq = ItemsPerTransReq.OnePerItem;
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
            if (_options._appendLangName > 0) _options._itemsPerTransReq = ItemsPerTransReq.OnePerItem;
        }


        /// <summary>
        /// Initialize 
        /// </summary>
        /// <param name="args">The list of command line arguments</param>
        public bool Initialize(string[]? args = null, bool exitAfterHelp = false)
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
                    _options._programDescription = new string("Language translator");
                Console.OutputEncoding = System.Text.Encoding.Unicode;
            }

            ParseArgs(args);
            if (_options._undoListFileName.Length == 0)
                _options._undoListFileName = Path.Combine(Directory.GetCurrentDirectory(), "undo_rename.bat");
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
                    OutPut("Early exit because could not create undo item at path \"" + _options._undoListFileName + "\".\nErr Msg: \"" + e.Message + "\"", OutPutLevel.ErrorLvl);
                    ++_options._errorCount;
                    return false;
                }
            }
            return true;
        }


        public class ItemDetails
        {
            public string OriginalText { get; set; }
            public string Translation { get; set; }
            public string SourceLang { get; set; }
        }
        public string GetAppendedName(ItemDetails itemDetails)
        {
            return (_options._appendOrgName ? " (" + itemDetails.OriginalText + ")" : "") + ((_options._appendLangName > 0 && itemDetails.SourceLang != null && itemDetails.SourceLang.Length > 0) ? itemDetails.SourceLang : "");
        }

        /// <summary>
        /// Sets Translation variable in each ItemDetails
        /// </summary>
        /// <param name="itemDetailsList">List of items to translate</param>
        /// <param name="TranslatedItemNames_str">Translated text</param>
        /// <param name="SourceLanguage">2 letter source language code</param>
        private void SetTranslation(List<ItemDetails> itemDetailsList, string TranslatedItemNames_str, string SourceLanguage)
        {
            string[] TranslatedItemNames = TranslatedItemNames_str.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            int fileDetailsIndex = 0;
            foreach (string fileName in TranslatedItemNames)
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
        /// <param name="itemDetailsList">List of items to translate</param>
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
        /// Allows derived class to replace invalid characters with alternatives.
        /// </summary>
        /// <param name="itemDetailsList">Item detail to translate</param>
        public string ItemDetailsReplaceInvalidChar(ItemDetails itemDetails)
        {
            // Example usage for derived classes
            //try
            //{
            //    // replaces illegal filename characters with alternatives
            //    itemDetails.Translation = itemDetails.Translation.Replace('\\', '-');
            //    itemDetails.Translation = itemDetails.Translation.Replace('/', '-');
            //    itemDetails.Translation = itemDetails.Translation.Replace(':', '-');
            //    itemDetails.Translation = itemDetails.Translation.Replace('*', ' ');
            //    itemDetails.Translation = itemDetails.Translation.Replace('?', ' ');
            //    itemDetails.Translation = itemDetails.Translation.Replace('\"', '\'');
            //    itemDetails.Translation = itemDetails.Translation.Replace('<', '[');
            //    itemDetails.Translation = itemDetails.Translation.Replace('>', ']');
            //    itemDetails.Translation = itemDetails.Translation.Replace('|', '-');
            //    itemDetails.Translation = itemDetails.Translation.Replace('\t', ' ');
            //    itemDetails.Translation = itemDetails.Translation.Replace('\n', ' ');
            //}
            //catch (Exception e)
            //{
            //    OutPut(e.Message, OutPutLevel.ErrorLvl);
            //    throw new Exception(e.Message);
            //}

            return itemDetails.Translation;
        }

        /// <summary>
        /// Allows derived class to get details on how many items will be checked for language translation candidates
        /// </summary>
        /// <param name="QtyItemsToCheck">Quantity of items which have to be checked</param>
        protected virtual void InitializeProgressBar(int QtyItemsToCheck) { }

        /// <summary>
        /// Allows derived class to get details on how many items will be checked for language translation candidates
        /// </summary>
        /// <param name="QtyItemsRenameCandidate">Quantity of items found that are rename candidates</param>
        protected virtual void TaskComplete(int QtyItemsRenameCandidate) { }

        /// <summary>
        /// Allows derived class to get on going progress status
        /// </summary>
        /// <param name="TotalCount">Quantity of items that have been processed</param>
        protected virtual void ShowProgress(int TotalCount) { }

        /// <summary>
        /// Allows derived class to get logging or status
        /// </summary>
        /// <param name="message">String to print</param>
        protected virtual void OutPut(string message, OutPutLevel outputlevel = OutPutLevel.NormalLvl) { }

        /// <summary>
        /// Allows derived class to have method to display help
        /// </summary>
        /// <param name="AdvanceHelpPage">Set to true to display advance help</param>
        protected virtual void PrintHelp(bool AdvanceHelpPage) { }
    }

}