using CommandLine;
using CommandLine.Text;

namespace c2eConsole
{  
    public class Options {
        [Option('v', "vervose", 
            Default = false,
            HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }

        [Option('g', "game", 
            Default = "Docking Station",
            HelpText = "Game to attach.")]
        public string Game { get; set; }
    }
    
    public class OptionsCommand : Options {
        [Option('e', "execute",
            HelpText = "Launch as CommandLine or execute command.")]
        public bool CommandLine { get; set; }
    }

    public class OptionsFile : Options {
        [Option('f', "file", 
            HelpText = "Input caos files to be processed.")]
        public string InputFile { get; set; }
    }
}