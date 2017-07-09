using System;
using System.IO;
using c2eLib.Caos;
using Microsoft.Extensions.DependencyInjection;  
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using CommandLine;
using CommandLine.Text;



namespace c2eConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            
            // build and configure instance
            var parser = new Parser(config => 
                       {
                           config.HelpWriter = Console.Out;
                       });
                       // var result = Parser.Default.ParseArguments<Options>(args)
            var result = parser.ParseArguments<OptionsCommand>(args)
                // options is an instance of Options type
                .WithParsed(options =>
                    {
                         LaunchApp(options); 
                    }
                )
                // errors is a sequence of type IEnumerable<Error>
                .WithNotParsed(errors => 
                    {
                         Environment.Exit(-1);
                    }
                );
        }

        private static void LaunchApp(Options options){
            // create service collection
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection,options);
     
            // create service provider
            var serviceProvider = serviceCollection.BuildServiceProvider();
            
            var logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();

            logger.LogDebug("Starting application");

            // entry to run app
            App application = serviceProvider.GetService<App>();
            application.Config(options);
            application.Run();
            
            // using (App app = serviceProvider.GetService<App>()){
            //    app.Run();
            // }

            logger.LogDebug("All done!");
        }
    
        private static void ConfigureServices(IServiceCollection serviceCollection, Options options)
        {
            // build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("app-settings.json", false)
                .Build();

            // add logging
            var loggingConfig = configuration.GetSection("Logging");

            if(options.Verbose){
                serviceCollection.AddSingleton(new LoggerFactory()
                            .AddConsole(LogLevel.Trace)
                         //   .AddFile(loggingConfig)
                );
            }
            else{
                 serviceCollection.AddSingleton(new LoggerFactory()
                         //   .AddFile(loggingConfig)
                 );
            }
            

            serviceCollection.AddLogging(); 
            serviceCollection.AddOptions();

            serviceCollection.Configure<AppSettings>(configuration.GetSection("Configuration"));
         
            // add services
            serviceCollection.AddTransient<ICaosInjector, SharedMemoryInjector>();
            serviceCollection.AddTransient<BufferLayout,BufferLayout>();
            // serviceCollection.AddTransient<ICaosInjector, SocketsInjector>()
     
            // add app
            serviceCollection.AddTransient<App>();
        }
    }
}