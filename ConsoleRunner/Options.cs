using CommandLine;
using CommandLine.Text;

namespace ConsoleRunner {
    public class Options {
        [Option('d', "directory", HelpText = "Directory with json files to be processed.")]
        public string DirectoryToRead { get; set; }

        [Option('s', "save-file", HelpText = "Path to output file.")]
        public string SaveTo { get; set; }

        [Option('l', "log-path", HelpText = "Path to directory with logs.")]
        public string LogTo { get; set; }

        [HelpOption]
        public string GetUsage() {
            return HelpText.AutoBuild(this);
        }
    }
}
