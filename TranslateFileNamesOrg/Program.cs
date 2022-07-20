using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;
using GTranslate.Translators;


namespace TranslateFilenamesOrg
{
    public class Program
    {
        #region class static variables
        private static Assembly _assembly = Assembly.GetExecutingAssembly();
        private static string _fromLang = null;
        private static string _targetLang = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
        private static string _path = Directory.GetCurrentDirectory();
        private static bool _appendOrgName = false;
        private static int _appendLangName = 0;
        private static bool _noRename = false;
        private static bool _verbose = false;
        private static bool _dottedOutput = false;
        private static bool _progressBarOutput = true;
        private static bool _silent = false;
        private static FilesPerTransReq _filesPerTransReq = FilesPerTransReq.Auto;
        private static bool _waitKeyPress = false;
        private static SearchOption _searchOption = SearchOption.TopDirectoryOnly;
        private static string _fileType = "*";
        private static bool _createUndoList = false;
        //private static bool _undoFileRename = false;
        private static string _undoListFileName = "";
        private const string _LONGPATHPREFIX = "\\\\?\\";
        private static string _longPathPrefix = "";
        private static string _programCommandLineName = "";
        private static string _programVersion = "";
        private static string _programFileVersion = "";
        private static string _programDescription = "";
        private static StreamWriter _writer = null;
        private static int _postProgressBarPos = 8;
        private static int _totalCount = 0;
        private static int _renameCount = 0;
        private static int _renameErrorCount = 0;
        private static int _errorCount = 0;
        private static int _maximumTranslateDataLength = 10000; // Fails at 10500
        private static int _workerThreadCount = 0;
        private static int _maxWorkerThreads = Math.Max(Environment.ProcessorCount, 4);
        private static List<string> _filesToRename = null;
        private static readonly object _lock = new object();
        #endregion

        /// <summary>
        /// Parses commandline arguments.
        /// </summary>
        /// <param name="args">Commandline argument list</param>
        private static void ParseArgs(string[] args)
        {
            bool AdvanceHelpPage = false;
            int StartAt = 0;
            if (args.Length != 0 && !args[0].StartsWith("-") && !args[0].Equals("/?") && !args[0].Equals("?")) _path = args[StartAt++];

            for (int i = StartAt; i < args.Length; i++)
            {
                string arg = args[i].StartsWith("--") ? args[i].Substring(1).ToLower() : args[i].ToLower();
                switch (arg)
                {
                    case "-fromlang":
                    case "-f":
                        if ((i + 1) < args.Length && !args[i + 1].StartsWith("-") && args[i + 1].Length == 2 && Char.IsLetter(args[i + 1][0]) && Char.IsLetter(args[i + 1][1])) _fromLang = args[++i].ToLower();
                        break;
                    case "-tolang":
                    case "-t":
                        if ((i + 1) < args.Length && !args[i + 1].StartsWith("-") && args[i + 1].Length == 2 && Char.IsLetter(args[i + 1][0]) && Char.IsLetter(args[i + 1][1])) _targetLang = args[++i].ToLower();
                        break;
                    case "-lentrans":
                    case "-n":
                        if ((i + 1) < args.Length && !args[i + 1].StartsWith("-") && Int32.Parse(args[i + 1]) > 250) _maximumTranslateDataLength = Int32.Parse(args[++i]);
                        break;
                    case "-maxthrds":
                    case "-m":
                        if ((i + 1) < args.Length && !args[i + 1].StartsWith("-") && Int32.Parse(args[i + 1]) > 1) _maxWorkerThreads = Int32.Parse(args[++i]);
                        break;
                    case "-createlist":
                    case "-c":
                        _createUndoList = true;
                        if ((i + 1) < args.Length && !args[i + 1].StartsWith("-")) _undoListFileName = args[++i];
                        break;
                    //case "-undo":
                    //case "-u":
                    //    _undoFileRename = true;
                    //    if ((i + 1) < args.Length && !args[i + 1].StartsWith("-")) _undoListFileName = args[++i];
                    //    break;
                    case "-ext":
                    case "-e":
                        if ((i + 1) < args.Length && !args[i + 1].StartsWith("-")) _fileType = args[++i].ToLower();
                        break;
                    case "-appendorigname":
                    case "-a":
                        _appendOrgName = true;
                        break;
                    case "-lang":
                    case "-l":
                        if ((i + 1) < args.Length && !args[i + 1].StartsWith("-") && Int32.Parse(args[i + 1]) > 1) _appendLangName = Int32.Parse(args[++i]);
                        else
                            _appendLangName = 9;
                        break;
                    case "-norename":
                    case "-x":
                        _noRename = true;
                        break;
                    case "-recursive":
                    case "-r":
                        _searchOption = SearchOption.AllDirectories;
                        break;
                    case "-one":
                    case "-0":
                    case "-1":
                    case "-o":
                        _filesPerTransReq = FilesPerTransReq.OnePerFile;
                        break;
                    case "-longpath":
                    case "-g":
                        _longPathPrefix = _LONGPATHPREFIX;
                        break;
                    case "-verbose":
                    case "-v":
                        _verbose = true;
                        break;
                    case "-itemize":
                    case "-z":
                        _progressBarOutput = false;
                        break;
                    case "-dotoutput":
                    case "-d":
                        _dottedOutput = true;
                        break;
                    case "-silent":
                    case "-s":
                        _silent = true;
                        break;
                    case "-presskey":
                    case "-p":
                        _waitKeyPress = true;
                        break;
                    case "-?":
                    case "/?":
                    case "?":
                        AdvanceHelpPage=true;
                        goto case "-h";
                    case "-help":
                    case "-h":
                        PrintHelp(AdvanceHelpPage);
                        Environment.Exit(0);
                        return;
                    default:
                        break;
                }
            }

            if (_dottedOutput) _progressBarOutput = _verbose = _silent = false;
            if (_verbose) _progressBarOutput = _silent = false;
            if (_silent) _progressBarOutput = false;
            if (_appendLangName > 0) _filesPerTransReq = FilesPerTransReq.OnePerFile;
        }
        public static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            if (!Initialize(args)) 
                return;

            string RecursiveModeText = (_searchOption == SearchOption.AllDirectories) ? " recursively" : "";
            string FromLang = (_fromLang == null) ? "AutoDetect" : _fromLang;
            ToConsole("Scanning path \"" + _path + "\"" + RecursiveModeText + " for files to translate.\n" +
                "[To-Lang:\"" + _targetLang + "\"] [From-Lang:\"" + FromLang + "\"] [Ext:\"" + _fileType + "\"] [AppOrg:" + _appendOrgName + "\"] [AppLang:\" + _appendLangName + \"\\\"] [Threads:" + _maxWorkerThreads + "\"] [Recursive:" + (_searchOption == SearchOption.AllDirectories) + "]\n"
                , OutPutLevel.PreProgressBar);
            ConsoleProgressBar progressBar = _progressBarOutput ? new ConsoleProgressBar(_filesToRename.Count, "Files processed") : null;
            if (_filesToRename.Count < ((_maxWorkerThreads * 4) + 11) && _filesPerTransReq == FilesPerTransReq.Auto)
                _filesPerTransReq = FilesPerTransReq.OnePerFile;
            string sourceText = "";
            List<FileDetails> fileDetailsList = new List<FileDetails>();
            List<Task> tasks = new List<Task>();
            int subCount = 0;
            foreach (var file in _filesToRename)
            {
                ToConsole("Parsing file: " + file);
                var fileDetails = new FileDetails()
                {
                    Name = Path.GetFileNameWithoutExtension(file),
                    Extension = Path.GetExtension(file),
                    ParrentPath = Path.GetDirectoryName(file)
                };

                if (fileDetails.Name.Length == 0)
                {
                    ToConsole("Failed to get file name for  \"" + file + "\".", OutPutLevel.ErrorLvl);
                    continue;
                }

                if ((_filesPerTransReq == FilesPerTransReq.OnePerFile && subCount > 0)
                    || 
                    (sourceText.Length + fileDetails.Name.Length + 4 > _maximumTranslateDataLength))
                {
                    if (_progressBarOutput)
                        progressBar.ShowProgress(_totalCount);
                    while (Volatile.Read(ref _workerThreadCount) > _maxWorkerThreads)
                        Thread.Sleep(10);
                    ToConsole("** Sending " + subCount.ToString() + " file names to translator.");
                    tasks.Add(TranslateSourceText(sourceText, fileDetailsList));
                    subCount = 0;
                    sourceText = "";
                    fileDetailsList = new List<FileDetails>();
                }

                fileDetailsList.Add(fileDetails);
                sourceText += fileDetails.Name;
                ++subCount;
                if (_filesToRename.Last() != file)
                    sourceText += "\n";
            }

            tasks.Add(TranslateSourceText(sourceText, fileDetailsList));
            Task.WaitAll(tasks.ToArray());
            if (_createUndoList)
                _writer.Close();
            sw.Stop();
            if (_progressBarOutput)
            {
                progressBar.ShowProgress(_filesToRename.Count);
                Console.SetCursorPosition(0, 6);
            }
            ToConsole("\nFound " + _renameCount + " files to rename out of " + _filesToRename.Count + ".", OutPutLevel.PostProgressBar);
            if (_renameErrorCount > 0)
                ToConsole(_renameErrorCount + " Rename errors occurred.", OutPutLevel.PostProgressBar);
            if (_errorCount > 0)
                ToConsole(_errorCount + " unknown errors occurred.", OutPutLevel.PostProgressBar);
            ToConsole("Translation completed. Elapse time = " + sw.Elapsed, OutPutLevel.PostProgressBar);
            if (_waitKeyPress)
            {
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Initialize 
        /// </summary>
        /// <param name="args">The list of commandline arguments</param>
        public static bool Initialize(string[] args)
        {
            _programCommandLineName = _assembly.GetCustomAttribute<AssemblyTitleAttribute>().Title;
            _programDescription = _assembly.GetCustomAttribute<AssemblyDescriptionAttribute>().Description;
            _programVersion = _assembly.GetName().Version.ToString();
            _programFileVersion = _assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version.ToString();

            Console.OutputEncoding = System.Text.Encoding.Unicode;
            ParseArgs(args);
            ToConsole("Opening path: " + _path, OutPutLevel.VerboseIfNotSilent);
            if (!Directory.Exists(_path))
            {
                ToConsole("Path does not exist. Exiting.", OutPutLevel.ErrorLvl);
                return false;
            }
            if (_undoListFileName.Length == 0)
                _undoListFileName = Path.Combine(_path, "undo_rename.bat");
            if (_createUndoList)
            {
                if (File.Exists(_undoListFileName))
                    File.Delete(_undoListFileName);
                try
                {
                    _writer = new StreamWriter(_undoListFileName);
                }
                catch (Exception e)
                {
                    ToConsole("Early exit because could not create undo file at path \"" + _undoListFileName + "\".\nErr Msg: \"" + e.Message + "\"", OutPutLevel.ErrorLvl);
                    ++_errorCount;
                    return false;
                }
            }
            
            _filesToRename = Directory.GetFiles(_path, _fileType, _searchOption).ToList();
            if (_filesToRename.Count == 0)
            {
                ToConsole("No files exist. Exiting.");
                ToConsole("Warning: Early exit because no files exist in path \"" + _path + "\".", OutPutLevel.ErrorLvl);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Translator Wrapper
        /// </summary>
        /// <param name="sourceText">Consolidated source text to translate</param>
        /// <param name="fileDetailsList">List of files to translate</param>
        private static async Task TranslateSourceText(string sourceText, List<FileDetails> fileDetailsList)
        {
            try
            {
                try
                {
                    Interlocked.Increment(ref _workerThreadCount);
                    await GTranslate_TranslateSourceText(sourceText, fileDetailsList);
                }catch (Exception ee){
                    ToConsole(ee.Message, OutPutLevel.ErrorLvl);
                    ++_errorCount;
                    throw new Exception(ee.Message);
                }finally{
                    Interlocked.Decrement(ref _workerThreadCount);
                }

                string UndoListText = "";
                foreach (FileDetails fileDetail in fileDetailsList)
                {
                    ++_totalCount;
                    if (fileDetail.Name == null || fileDetail.Name.Length == 0 || fileDetail.Translation == null || fileDetail.Translation.Length == 0)
                        continue;
                    try
                    { 
                        string oldFileName = Path.Combine(fileDetail.ParrentPath, fileDetail.Name + fileDetail.Extension);
                        string appendedName = (_appendOrgName ? " (" + fileDetail.Name + ")" : "") + ((_appendLangName > 0 && fileDetail.SourceLang != null && fileDetail.SourceLang.Length > 0) ? fileDetail.SourceLang : "");
                        fileDetail.Translation = FileDetailsReplaceIllegalFileNameChar(fileDetail);// Replaces illegal windows file name characters with alternative characters
                        string newFileName = Path.Combine(fileDetail.ParrentPath, fileDetail.Translation + appendedName + fileDetail.Extension);

                        if (fileDetail.Translation.Equals(fileDetail.Name))
                            ToConsole("Skipping file rename \"" + fileDetail.Name + "\" to \"" + fileDetail.Translation + "\"");
                        else
                        {
                            ++_renameCount;
                            if (_noRename)
                            {
                                UndoListText += newFileName + "\n" + oldFileName + "\n";
                                ToConsole("Rename candidate \"" + oldFileName + "\" to-> \"" + newFileName + "\"");
                            }
                            else
                            {
                                try
                                {
                                    File.Move(_longPathPrefix + oldFileName, _longPathPrefix + newFileName);
                                    UndoListText += newFileName + "|" + oldFileName + "\n";
                                    ToConsole("Renamed file \"" + oldFileName + "\" to \"" + newFileName + "\"");
                                } catch (Exception exc) {
                                    int lastErrCode = Marshal.GetLastWin32Error();
                                    string newIndexFileName = lastErrCode == 183 ? RenameFileWithAppendedIndex(_longPathPrefix + oldFileName, _longPathPrefix + Path.Combine(fileDetail.ParrentPath, fileDetail.Translation + appendedName), fileDetail.Extension) : null;
                                    if (newIndexFileName != null)
                                    {
                                        UndoListText += newIndexFileName + "|" + oldFileName + "\n";
                                        ToConsole("Renamed file \"" + oldFileName + "\" to \"" + newIndexFileName + "\"");
                                    }
                                    else
                                    {
                                        --_renameCount;
                                        ++_renameErrorCount;
                                        ToConsole("Could not rename file \"" + oldFileName + "\" to \"" + newFileName + "\"\nErr Msg: " + exc.Message, OutPutLevel.ErrorLvl);
                                    }
                                }
                            }
                        }
                    } catch (Exception eeeee) {
                        ToConsole(eeeee.Message, OutPutLevel.ErrorLvl);
                        ++_errorCount;
                        throw new Exception(eeeee.Message);
                    }
                }

                if (UndoListText.Length > 0)
                {
                    if (_createUndoList)
                    {
                        ToConsole("Creating undo list file at \"" + _undoListFileName + "\"");
                        Monitor.Enter(_lock);
                        try
                        {
                            Task write_result = _writer.WriteAsync(UndoListText);
                            while (!write_result.IsCompleted)
                                Thread.Sleep(10);
                            Monitor.Pulse(_lock);
                        } finally {
                            Monitor.Exit(_lock);
                        }
                    }
                }
            } catch (Exception e) {
                 ToConsole(e.Message, OutPutLevel.ErrorLvl);
                ++_errorCount;
            }
        }

        /// <summary>
        /// Sets Translation variable in each FileDetails
        /// </summary>
        /// <param name="oldFileName">Original file name</param>
        /// <param name="newFileName">Translated file name</param>
        /// <param name="Ext">file name extension</param>
        private static string RenameFileWithAppendedIndex(string oldFileName, string newFileName, string Ext)
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
                catch(Exception e)
                {
                    ++_renameErrorCount;
                    ToConsole("Could not rename file \"" + oldFileName + "\" to \"" + newIndexFileName + "\"\nErr Msg: " + e.Message, OutPutLevel.ErrorLvl);
                }
            }
            return null;
        }


        /// <summary>
        /// Sets Translation variable in each FileDetails
        /// </summary>
        /// <param name="fileDetailsList">List of files to translate</param>
        /// <param name="TranslatedFileNames_str">Translated text</param>
        /// <param name="SourceLanguage">2 letter source language code</param>
        private static void SetTranslation(List<FileDetails> fileDetailsList, string TranslatedFileNames_str, string SourceLanguage)
        {
            string[] TranslatedFileNames = TranslatedFileNames_str.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            int fileDetailsIndex = 0;
            foreach (string fileName in TranslatedFileNames)
            {
                fileDetailsList[fileDetailsIndex].Translation = fileName.Contains("\\n") ? fileName.Substring(0, fileName.Length - 2) : fileName;
                fileDetailsIndex++;
            }

            if (fileDetailsIndex == 1 && fileDetailsList.Count == 1 && SourceLanguage.Length > 0)
                fileDetailsList[0].SourceLang = SourceLanguage;
        }


        /// <summary>
        /// Translate via GTranslate
        /// </summary>
        /// <param name="sourceText">Source text to trnaslate</param>
        /// <param name="fileDetailsList">List of files to translate</param>
        private static async Task GTranslate_TranslateSourceText(string sourceText, List<FileDetails> fileDetailsList)
        {
            var translator = new GoogleTranslator2();// AggregateTranslator();
            try
            {
                GTranslate.Results.ITranslationResult result = await translator.TranslateAsync(sourceText, _targetLang, _fromLang);
                //GTranslate.Language SourceLanguage = new GTranslate.Language(result.SourceLanguage.Name);
                ToConsole($"Translated text: {result.Translation}");
                ToConsole($"Source Language: {result.SourceLanguage}");
                ToConsole($"Target Language: {result.TargetLanguage}");
                ToConsole($"Service: {result.Service}");
                string SrcLang = "";
                if (result.SourceLanguage.Name != null && result.SourceLanguage.Name.Length > 0)
                {
                    switch (_appendLangName)
                    {
                        case 2:
                            SrcLang = "_[" + result.SourceLanguage.ISO6391 + "]";
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
                SetTranslation(fileDetailsList, result.Translation, SrcLang);

            }
            catch (Exception e)
            {
                ToConsole(e.Message, OutPutLevel.ErrorLvl);
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// replaces illegal filename characters with alternatives
        /// </summary>
        /// <param name="fileDetailsList">List of files to translate</param>
        private static string FileDetailsReplaceIllegalFileNameChar(FileDetails fileDetail)
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
                ToConsole(e.Message, OutPutLevel.ErrorLvl);
                throw new Exception(e.Message);
            }

            return fileDetail.Translation;
        }

        enum OutPutLevel
        {
            VerboseIfNotSilent,
            NormalLvl,
            PreProgressBar,
            PostProgressBar,
            ErrorLvl
        }

        enum FilesPerTransReq
        {
            Auto,
            OnePerFile,
            Many
        }

        /// <summary>
        /// Prints message to the console depending on settings for verbosity, silent, dotOutput, &  outputlevel
        /// </summary>
        /// <param name="message">String to print</param>
        private static void ToConsole(string message, OutPutLevel outputlevel = OutPutLevel.NormalLvl)
        { 
            if (_silent)
                return;
            else if (outputlevel == OutPutLevel.ErrorLvl || outputlevel == OutPutLevel.PreProgressBar || outputlevel == OutPutLevel.PostProgressBar)
            {
                if (_progressBarOutput)
                {
                    int LinePos = 0;
                    switch (outputlevel)
                    {
                        case OutPutLevel.PreProgressBar:
                            LinePos = 0;
                            break;
                        case OutPutLevel.PostProgressBar:
                            LinePos = _postProgressBarPos;
                            break;
                        case OutPutLevel.ErrorLvl:
                            LinePos = 5;
                            message = "ERROR: - " + message;
                            break;
                    }
                    Console.SetCursorPosition(0, LinePos);
                    string Buffer = new string(' ', Console.WindowWidth);
                    Console.WriteLine(Buffer);
                    Console.WriteLine(Buffer);
                    Console.SetCursorPosition(0, LinePos);
                    Console.WriteLine(message);
                    if (outputlevel == OutPutLevel.PostProgressBar)
                        _postProgressBarPos = Console.CursorTop + 1;
                }
                else if (_dottedOutput)
                    Console.WriteLine("\n" + message);
                else
                    Console.WriteLine(message);
            }
            else if (_progressBarOutput)
                return;
            else if (_dottedOutput)
                Console.Write(".");
            else if (_verbose)
                Console.WriteLine("VERBOSE: - " + message);
            else if (outputlevel == OutPutLevel.VerboseIfNotSilent)
                Console.WriteLine("INFO: " + message);
        }

        /// <summary>
        /// Prints the help text to console.
        /// </summary>
        private static void PrintHelp(bool AdvanceHelpPage)
        {
            Console.WriteLine(_programCommandLineName + " Ver:" + _programVersion + ":\t" + _programDescription + ".");
            Console.WriteLine("Usage: " + _programCommandLineName + " [PATH]...");
            Console.WriteLine("       " + _programCommandLineName + " [PATH] [OPTION]...");
            Console.WriteLine("       " + _programCommandLineName + " [OPTION]...");
            Console.WriteLine("If no path is given, the current working directory is used.");
            Console.WriteLine("Options:");
            Console.WriteLine("  -r, -Recursive\tIncludes sub-directories.");
            Console.WriteLine("  -e, -Ext [type]\tExtension type. (example: *.mp4) (default: *)");
            Console.WriteLine("  -g, -LongPath\t\tSupport paths longer than 256.");
            Console.WriteLine("  -a, -AppendOrigName\tExample: [translated-name]([original-name]).[ext].");
            Console.WriteLine("  -t, -ToLang [lang]\t2 letter target language code (default: " + _targetLang + ")");
            if (AdvanceHelpPage)
            {
                Console.WriteLine("  -f, -FromLang [lang]\t2 letter source language code (default: -- [ = Auto Detect]) ");
                Console.WriteLine("  -x, -NoRename\t\tDo NOT rename files. Only prints rename candidates.");
                Console.WriteLine("  -d, -DotOutput\tPrints dot for progress. Overrides all other outputs.");
                Console.WriteLine("  -z, -Itemize\t\tReplaces progressbar with itemized output.");
                Console.WriteLine("  -v, -Verbose\t\tPrints extra details. Overrides Silent option.");
                Console.WriteLine("  -s, -Silent\t\tNo output to the console. Overrides Itemize.");
                Console.WriteLine("  -m, -MaxThrds [num]\tMaximum worker threads. (default: " + _maxWorkerThreads + ").");
                Console.WriteLine("  -n, -LenTrans [num]\tMax len translation request. (default: " + _maximumTranslateDataLength + ").");
                Console.WriteLine("  -o, -One\t\tOne file name per translation request. Overrides LenTrans. (default: Auto)");
                Console.WriteLine("  -l, -Lang\tAppends 2 letter source language. Requires -One");
                Console.WriteLine("  -c, -CreateList\tCreate a list of files renamed (renamedFiles.txt).");
                //Console.WriteLine("  -u, -undo\tUndo file rename using (renamedFiles.txt).");
                Console.WriteLine("  -p, -PressKey\t\tPrompt press key before program exit.");
                Console.WriteLine("  -?, -AdvHelp\t\tDisplays this help page.");
                Console.WriteLine("  -h, -Help\t\tDisplays basic help page.");
            }
            else
            {
                Console.WriteLine("  -?, -AdvHelp\t\tDisplays advanced help page.");
                Console.WriteLine("  -h, -Help\t\tDisplays this help page.");
            }

            Console.WriteLine("Examples:");
            Console.WriteLine("\t" + _programCommandLineName + " -r");
            Console.WriteLine("\t" + _programCommandLineName + " \"C:\\Users\\jane-doe\\Pictures\" -r -ext *.jpg");
            Console.WriteLine("\nFor more help, see https://github.com/David-Maisonave/TranslateFilenamesOrg\n");
        }
    }

    public class FileDetails
    {
        public string Name { get; set; }
        public string Extension { get; set; }
        public string Translation { get; set; }
        public string ParrentPath { get; set; }
        public string SourceLang { get; set; } // 2 letter ISO source lang
    }
}
