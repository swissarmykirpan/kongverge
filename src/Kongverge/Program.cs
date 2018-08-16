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
        private static int Main(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "Kongverge",
                Description = "Kong configuration convergence."
            };

            var options = new Options(app);

            app.OnExecute(async () =>
            {
                ServiceRegistration.CreateConsoleLogger();

                var exitCode = options.Validate();
                if (exitCode.HasValue)
                {
                    return ExitWithCode.Return(exitCode.Value);
                }

                var services = new ServiceCollection();
                ServiceRegistration.AddServices(services);

                var serviceProvider = services.BuildServiceProvider();

                options.Apply(serviceProvider);

                var workflow = serviceProvider.GetService<Workflow>();

                Console.WriteLine($"*************\n* {app.Name} *\n*************");

                return await workflow.Execute().ConfigureAwait(false);
            });

            var returnCode = app.Execute(args);
#if DEBUG
            Console.Write("Press a key to finish");
            Console.ReadKey();
#endif

            return returnCode;
        }

        private class Options
        {
            private const int MinPort = 1024;
            private const int MaxPort = 49151;

            public Options(CommandLineApplication app)
            {
                app.HelpOption("-?|-h|--help");

                DryRun = app.Option("-t|--test",
                    "Perform dry run without updating Kong system",
                    CommandOptionType.NoValue);

                InputFolder = app.Option("-i|--input <inputFolder>",
                    "Folder for input data",
                    CommandOptionType.SingleValue);

                OutputFolder = app.Option("-o|--output <outputFolder>",
                    "Folder to output data from host",
                    CommandOptionType.SingleValue);

                Host = app.Option("-H|--host <KongHostname>",
                    "Kong Admin host with which to communicate",
                    CommandOptionType.SingleValue);

                Port = app.Option("-p|--port <KongAdminPort>",
                    "Kong Admin API port",
                    CommandOptionType.SingleValue);
            }

            public CommandOption DryRun { get; }
            public CommandOption InputFolder { get; }
            public CommandOption OutputFolder { get; }
            public CommandOption Host { get; }
            public CommandOption Port { get; }

            public ExitCode? Validate()
            {
                if (InputFolder.HasValue() && OutputFolder.HasValue())
                {
                    return ExitCode.IncompatibleArguments;
                }

                if (Port.HasValue() && (!int.TryParse(Port.Value(), out var port) || port > MaxPort || port < MinPort))
                {
                    return ExitCode.InvalidPort;
                }

                return null;
            }

            public void Apply(IServiceProvider serviceProvider)
            {
                var settings = serviceProvider.GetService<IOptions<Settings>>().Value;

                if (Host.HasValue())
                {
                    settings.Admin.Host = Host.Value();
                }

                if (OutputFolder.HasValue())
                {
                    settings.OutputFolder = OutputFolder.Value();
                }

                if (Port.HasValue())
                {
                    settings.Admin.Port = int.Parse(Port.Value());
                }

                settings.DryRun = DryRun.HasValue();

                if (InputFolder.HasValue())
                {
                    settings.InputFolder = InputFolder.Value();
                }
            }
        }
    }
}
