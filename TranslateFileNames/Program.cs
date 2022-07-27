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
using TranslateFilenamesCore;


namespace TranslateFilenamesOrg
{
    public class Program
    {
        public static void Main(string[] args)
        {
            TranslateFilenamesConsole translateFilenamesConsole = new TranslateFilenamesConsole(args);
            translateFilenamesConsole.TranslatePath();
        }
    }

    public class TranslateFilenamesConsole : TranslateFilenamesCore.TranslateFilenames
    {
        public TranslateFilenamesConsole(string[] args): base(args, true, true)
        {
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
            {
                if (_options._progressBarOutput)
                {
                    int LinePos = 0;
                    switch (outputlevel)
                    {
                        case OutPutLevel.PreProgressBar:
                            LinePos = 0;
                            break;
                        case OutPutLevel.PostProgressBar:
                            LinePos = _options._postProgressBarPos;
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
                        _options._postProgressBarPos = Console.CursorTop + 1;
                }
                else if (_options._dottedOutput)
                    Console.WriteLine("\n" + message);
                else
                    Console.WriteLine(message);
            }
            else if (_options._progressBarOutput)
                return;
            else if (_options._dottedOutput)
                Console.Write(".");
            else if (_options._verbose)
                Console.WriteLine("VERBOSE: - " + message);
            else if (outputlevel == OutPutLevel.VerboseIfNotSilent)
                Console.WriteLine("INFO: " + message);
        }

        /// <summary>
        /// Prints the help text to console.
        /// </summary>
        protected override void PrintHelp(bool AdvanceHelpPage)
        {
            Console.WriteLine(_options._programCommandLineName + " Ver:" + _options._programVersion + ":\t" + _options._programDescription + ".");
            Console.WriteLine("Usage: " + _options._programCommandLineName + " [PATH]...");
            Console.WriteLine("       " + _options._programCommandLineName + " [PATH] [OPTION]...");
            Console.WriteLine("       " + _options._programCommandLineName + " [OPTION]...");
            Console.WriteLine("If no path is given, the current working directory is used.");
            Console.WriteLine("Options:");
            Console.WriteLine("  -r, -Recursive\tIncludes sub-directories.");
            Console.WriteLine("  -e, -Ext [type]\tExtension type. (example: *.mp4) (default: *)");
            Console.WriteLine("  -g, -LongPath\t\tSupport paths longer than 256.");
            Console.WriteLine("  -a, -AppendOrigName\tExample: [translated-name]([original-name]).[ext].");
            Console.WriteLine("  -t, -ToLang [lang]\t2 letter target language code (default: " + _options._targetLang + ")");
            if (AdvanceHelpPage)
            {
                Console.WriteLine("  -f, -FromLang [lang]\t2 letter source language code (default: -- [ = Auto Detect]) ");
                Console.WriteLine("  -x, -NoRename\t\tDo NOT rename files. Only prints rename candidates.");
                Console.WriteLine("  -d, -DotOutput\tPrints dot for progress. Overrides all other outputs.");
                Console.WriteLine("  -i, -Itemize\t\tReplaces progressbar with itemized output.");
                Console.WriteLine("  -v, -Verbose\t\tPrints extra details. Overrides Silent option.");
                Console.WriteLine("  -s, -Silent\t\tNo output to the console. Overrides Itemize.");
                Console.WriteLine("  -m, -MaxThrds [num]\tMaximum worker threads. (default: " + _options._maxWorkerThreads + ").");
                Console.WriteLine("  -n, -LenTrans [num]\tMax len translation request. (default: " + _options._maximumTranslateDataLength + ").");
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
            Console.WriteLine("\t" + _options._programCommandLineName + " -r");
            Console.WriteLine("\t" + _options._programCommandLineName + " \"C:\\Users\\jane-doe\\Pictures\" -r -ext *.jpg");
            Console.WriteLine("\nFor more help, see https://github.com/David-Maisonave/TranslateFilenamesOrg\n");
        }

    }
}
