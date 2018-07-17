using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using Kongverge.Common.DTOs;
using Kongverge.Common.Services;
using Kongverge.Extension;
using Kongverge.Validation.Helpers;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;

namespace Kongverge.Validation
{
    public class Program
    {
        private static IKongAdminService _kongAdminService;
        private static ITestHelper _testHelper;
        private static string _pluginId;
        
        public static async Task<int> Main(string[] args)
        {
            //Ensure we definitely remove that plugin, even if we crash
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
            Console.CancelKeyPress += CancelHandler;

            var app = new CommandLineApplication
            {
                Name = "Kongverge",
                Description = "Kong configuration convergence."
            };

            app.HelpOption("-?|-h|--help");

            var testsFolderOption = app.Option("-t|--testFolder",
                "Specify folder containing tests",
                CommandOptionType.SingleValue);

            var portOption = app.Option("-p|--port",
                "Specify local logging listening port",
                CommandOptionType.SingleValue);

            app.OnExecute(async () =>
            {
                var services = new ServiceCollection();
                var testTracker = new TestTracker();
                var startup = new Startup(testTracker);
                startup.ConfigureServices(services);
                var serviceProvider = services.BuildServiceProvider();

                _kongAdminService = serviceProvider.GetRequiredService<IKongAdminService>();
                _testHelper = serviceProvider.GetService<ITestHelper>();

                var configuration = serviceProvider.GetService<IOptions<Settings>>().Value;

                if (portOption.HasValue())
                    configuration.TestPort = int.Parse(portOption.Value());

                if (testsFolderOption.HasValue())
                    configuration.TestFolder = testsFolderOption.Value();

                
                //Setup HTTP Logging Plugin
                var localIp = GetMostLikelyIpAddress().ToString();
                var httpEndpoint = $"http://{localIp}:{configuration.TestPort}";
                var config = new Dictionary<string, object> {{"http_endpoint", httpEndpoint}};
                Log.Information($"Adding HTTP Logging Plugin with url: {httpEndpoint}");
                var plugin = await _kongAdminService.UpsertPlugin(new PluginBody("http-log", config));
                _pluginId = plugin.Id;

                await Task.Delay(5000);

                //Start Logging Webserver
                BuildWebhost(args, testTracker, configuration.TestPort).Start();
                await WaitForWebserver(configuration.TestPort);


                _testHelper.PopulateTests();
                await _testHelper.RunTests().ConfigureAwait(false);
                await _testHelper.Validate().ConfigureAwait(false);


#if DEBUG
                Console.WriteLine("Press any key to continue");
                Console.ReadKey();
#endif

                return await Task.FromResult(0);
            });

            return await Task.FromResult(app.Execute(args));
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            RemovePlugin();
        }

        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            RemovePlugin();
        }

        private static void CancelHandler(object sender, ConsoleCancelEventArgs args)
        {
            RemovePlugin();
        }
        private static void RemovePlugin()
        {
            Log.Information($"Removing HTTP Logging Plugin {_pluginId}");
            _kongAdminService?.DeletePlugin(_pluginId);
        }

        private static IPAddress GetMostLikelyIpAddress()
        {
            var gateway = GetDefaultGateway().ToString();

            var iphostname = Dns.GetHostName();  // Resolving Host name
            var ipentry = Dns.GetHostEntry(iphostname);
            var addr = ipentry.AddressList;// Resolving IP Addresses

            foreach (var ipAddress in addr)
            {
                if (ipAddress.ToString().Substring(0, 6) == gateway.Substring(0, 6))  //dirty match for most likely IPAddress
                    return ipAddress;
            }

            throw new Exception("Unable to determine monitoring IP Address.");
        }
        public static IPAddress GetDefaultGateway()
        {
            return NetworkInterface
                .GetAllNetworkInterfaces()
                .Where(n => n.OperationalStatus == OperationalStatus.Up)
                .Where(n => n.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .SelectMany(n => n.GetIPProperties()?.GatewayAddresses)
                .Select(g => g?.Address)
                .FirstOrDefault(a => (a != null && a.AddressFamily != AddressFamily.InterNetworkV6));
        }

        private static async Task WaitForWebserver(int port)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri($"http://localhost:{port}");
                var success = false;
                do
                {
                    try
                    {
                        var response = await httpClient.GetAsync("/").ConfigureAwait(false);
                        success = response.StatusCode == HttpStatusCode.OK;
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, ex.Message);
                    }
                    await Task.Delay(100);
                } while (!success);
            }
        }

        private static IWebHost BuildWebhost(string[] args, TestTracker testTracker, int port)
        {
            return WebHost
                .CreateDefaultBuilder(args)
                .UseUrls($"http://*:{port}", $"http://0.0.0.0:{port}")
                .UseKestrel()
                .UseIISIntegration()
                .UseStartup<Startup>()
                .ConfigureServices(services => services.AddSingleton(testTracker))
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                })
                .Build();
        }

    }
}
