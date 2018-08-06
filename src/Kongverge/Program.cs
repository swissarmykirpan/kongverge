using System;
using Kongverge.Common;
using Kongverge.Common.DTOs;
using Kongverge.Common.Helpers;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Kongverge
{
    internal class Program
    {
        private const int MaxPort = 49151;
        private const int MinPort = 1024;

        private static int Main(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "Kongverge",
                Description = "Kong configuration convergence."
            };
            app.HelpOption("-?|-h|--help");

            var dryrunOption = app.Option("-t|--test",
                "Perform dry run without updating Kong system",
                CommandOptionType.NoValue);

            var inputFolderOption = app.Option("-i|--input <inputFolder>",
                "Folder for input data",
                CommandOptionType.SingleValue);

            var outputFolderOption = app.Option("-o|--output <outputFolder>",
                "Folder to output data from host",
                CommandOptionType.SingleValue);

            var hostOption = app.Option("-H|--host <KongHostname>",
                "Kong Admin host with which to communicate",
                CommandOptionType.SingleValue);

            var portOption = app.Option("-p|--port <KongAdminPort>",
                "Kong Admin API port",
                CommandOptionType.SingleValue);

            app.OnExecute(async () =>
            {
                ServiceRegistration.CreateConsoleLogger();
                var services = new ServiceCollection();
                ServiceRegistration.AddServices(services);

                var serviceProvider = services.BuildServiceProvider();

                #region Configuration Options

                var configuration = serviceProvider.GetService<IOptions<Settings>>().Value;

                if (hostOption.HasValue())
                {
                    configuration.Admin.Host = hostOption.Value();
                }

                if (outputFolderOption.HasValue())
                {
                    configuration.OutputFolder = outputFolderOption.Value();
                }

                if (portOption.HasValue())
                {
                    if (!int.TryParse(portOption.Value(), out var port)
                        || port > MaxPort
                        || port < MinPort)
                    {
                        return ExitWithCode.Return(ExitCodes.InvalidPort);
                    }
                    configuration.Admin.Port = port;
                }

                configuration.DryRun = dryrunOption.HasValue();
                if (inputFolderOption.HasValue() && outputFolderOption.HasValue())
                {
                    return ExitWithCode.Return(ExitCodes.IncompatibleArguments);
                }

                if (inputFolderOption.HasValue())
                {
                    configuration.InputFolder = inputFolderOption.Value();
                }

                #endregion

                var workflow = serviceProvider.GetService<Workflow>();

                Console.WriteLine($"*************\n* {app.Name} *\n*************");

                return await workflow.Execute().ConfigureAwait(false);
            });

            var returncode = app.Execute(args);
#if DEBUG
            Console.Write("Press a key to finish");
            Console.ReadKey();
#endif

            return returncode;
        }
    }
}
