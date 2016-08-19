using System;
using CommandLine;
using CommandLine.Text;

namespace AzureBillingExport.Console
{
    public class Options
    {
        // Omitting long name, default --verbose
        [Option('v',"verbose",
          HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }

        [Option('l',"language",DefaultValue = "Fr",
          HelpText = "Content language.")]
        public string Language { get; set; }

        [Option('m',"mail",
            HelpText="Send Message via mail using key in AppConfig")]
        public bool UseMail { get; set; }
        
        [Option('u', "use-interactive", 
           HelpText = "Use Interactive Dialog and User AD Account")]
        public bool UseInteractiveAccount { get; set; }

        [Option("start-date",Required = true,
           HelpText = "Specify Start Date")]
        public DateTime StartDate { get; set; }

        [Option("end-date",DefaultValue = null,
          HelpText = "Specify End Date, default is Today")]
        public DateTime EndDate { get; set; }


        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText
            {
                Heading = new HeadingInfo("Azure Billing Export Console", "1.0"),
                Copyright = new CopyrightInfo("jdevillard", 2016),
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true
            };
            help.AddPreOptionsLine("More information on github page : https://github.com/jdevillard/AzureBillingExport");
            help.AddPreOptionsLine("This softare is delivered under MIT Licence");
            help.AddOptions(this);
            return help;
        }
    }
}
