using ETL.Assessment.Application.DataReaders.Implementation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ETL.Assessment.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging(builder => builder.AddConsole());
                    services.AddTransient<CabCsvDataProcessor>();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    IServiceProvider serviceProvider = services.BuildServiceProvider();
                    var csvDataReader = serviceProvider.GetRequiredService<CabCsvDataProcessor>();

                    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                    string connectionString = hostContext.Configuration["ConnectionStrings:DefaultConnection"];

                    // Check if there are at least two command-line arguments
                    if (args.Length >= 2)
                    {
                        // Use command-line arguments for input file and duplicates file
                        string inputCsvFilePath = args[0];
                        string duplicatesCsvFilePath = args[1];

                        // Your application logic here
                        csvDataReader.Run(inputCsvFilePath, duplicatesCsvFilePath, connectionString);
                    }
                    else
                    {
                        Console.WriteLine("Please provide input and duplicates file paths as command-line arguments.");
                    }
                });
    }
}
