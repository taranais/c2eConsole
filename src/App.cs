using c2eLib.Caos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace c2eConsole
{
    public class App
    {
        private readonly ICaosInjector _caosInjector;
        private readonly ILogger<App> _logger;
        private readonly AppSettings _config;
        private Options _options;
     
        public App(ICaosInjector caosInjector,
                    IOptions<AppSettings> config,
                    ILogger<App> logger = null)
        {
            _caosInjector = caosInjector;
            _config = config.Value;
            _logger = logger;
        }
     
        public void Config(Options options)
        {
            _options = options;
        }

        public void Run()
        {
            _logger?.LogInformation($"This is a console application for {_config.Title}");

            if(_caosInjector.Init(_options.Game))
            {
                if(_options is OptionsCommand)
                {
                    CommandLineCaos();
                }
                else if(_options is OptionsFile)
                {
                    // TODO exec file
                    //FileCaos();
                }
                _caosInjector.Stop();
                  
            /*
            String getallcreatures =
                              "enum 4 0 0 " +                    // iterate creatures
                                "doif targ <> null " +           // check not null ??
                                  "sets va01 gtos 0 " +          // get moniker to va01
                                  "outs va01 " +                 // PRINT moniker
                                  "outs \" | \" " +              // PRINT separator
                                "endi " +
                              "next ";
            */
            }
            else{
                Console.WriteLine("{0} not started.",_options.Game);
                Console.WriteLine("Start {0} first. ",_options.Game);
            }
        }

        private void CommandLineCaos(){

             if(!((OptionsCommand)_options).CommandLine){
                Console.WriteLine("Warnning you can damage your world with this utility.");
                Console.WriteLine("To close type \":exit\" .");    
                var lineRead = System.ReadLine.Read("Caos > ");
                    while(!lineRead.Equals(":exit"))
                    {
                        CaosResult result = _caosInjector.SendCaos(
                                lineRead.TrimEnd(Environment.NewLine.ToCharArray()),"execute");    
                        if(result.Failed)
                        {
                           // TODO report fail
                        }
                        Console.WriteLine(result.ContentAsString());                                  
                        
                        lineRead = System.ReadLine.Read("Caos > ");
                    }
                }
        }
    }
}