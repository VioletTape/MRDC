using System;
using System.IO;
using CommandLine;
using MRDC;
using MRDC.Extenstions;
using Serilog;

namespace ConsoleRunner {
    internal class Program {
        private static void Main(string[] args) {
            var options = new Options();

            if (args.Length == 0) {
                Console.WriteLine(options.GetUsage());
                return;
            }


            if (Parser.Default.ParseArguments(args, options)) {
                var logger = new LoggerConfiguration()
                        .WriteTo.ColoredConsole()
                        .CreateLogger();

                Log.Logger = logger;

                ProcessArguments(options);
                LogArguments(options);

                var marketDataCleanser = new MarketDataCleanser();
                var sourceDirectory = new DirectoryInfo(options.DirectoryToRead);
                marketDataCleanser.CleanupDataIn(sourceDirectory);
            }
            else {
                // Display the default usage information
                Console.WriteLine(options.GetUsage());
            }
        }

        private static void ProcessArguments(Options options) {
            try {
                if (!options.DirectoryToRead.IsNullOrEmpty())
                    options.DirectoryToRead = Path.GetFullPath(options.DirectoryToRead);
            }
            catch (Exception e) {
                Log.Error(e.Message);
                options.DirectoryToRead += "- invalid.";
            }


            try {
                if (!options.SaveTo.IsNullOrEmpty())
                    options.SaveTo = Path.GetFullPath(options.SaveTo);
            }
            catch (Exception e) {
                Log.Error(e.Message);
                options.SaveTo += "- invalid.";
            }


            if (options.LogTo.IsNullOrEmpty()) {
                var baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                var isBaseDirAccessable = IsPathAccessable(baseDir);
                if (isBaseDirAccessable.Result) {
                    options.LogTo = baseDir;
                }
                else {
                    var fallbackUserDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @"MarketDataCleaner\Logs");
                    var isFallbackAccessable = IsPathAccessable(fallbackUserDir);
                    if (isFallbackAccessable.Result)
                        options.LogTo = fallbackUserDir;
                }
                if (!options.LogTo.IsNullOrEmpty())
                    Log.Logger.Debug("Logging directory changed to {LogTo}", options.LogTo);
                else
                    Log.Logger.Debug("Can't setup logging directory. "
                                     + $"{baseDir} and {Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)} not accesable. "
                                     + "Loggin to console only.");
            }
        }

        public static (bool Result, string Message) IsPathAccessable(string path) {
            if (path.IsNullOrEmpty())
                return (false, "Path is empty.");

            try {
                var fullPath = Path.GetFullPath(path);
                var directoryName = Path.GetDirectoryName(fullPath);

                var directoryInfo = new DirectoryInfo(directoryName);
                if (!directoryInfo.Exists)
                    directoryInfo.Create();

                var tempFileName = Path.GetFileName(Path.GetTempFileName());
                var testFile = Path.Combine(directoryInfo.FullName, tempFileName);
                var fileInfo = new FileInfo(testFile);
                fileInfo.Create().Close();
                fileInfo.Delete();
            }

            catch (Exception e) {
                return (false, e.Message);
            }

            return (true, "");
        }

        private static void LogArguments(Options options) {
            Log.Logger.Information("Working parameters:");
            Log.Logger.Information("\tTarget directory: {SourceDir}", options.DirectoryToRead);
            Log.Logger.Information("\tOutput file: {CleanData}", options.SaveTo);
            Log.Logger.Information("\tLog directory: {LogTo}", options.LogTo);
            Log.Logger.Information("");
        }
    }
}
