using System.IO;
using System.Net.Http;
using Kongverge.Common.Services;
using Kongverge.Common.DTOs;
using Kongverge.Common.Helpers;
using Kongverge.Common.Plugins;
using Kongverge.Extension;
using Kongverge.Validation.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;

namespace Kongverge.Validation
{
    public class Startup
    {
        private readonly TestTracker _testTracker;

        public Startup(TestTracker testTracker)
        {
            _testTracker = testTracker;
        }

        // This Method gets called by the runtime. Use this Method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            services.Replace(new ServiceDescriptor(typeof(TestTracker), _testTracker));
            services.AddSingleton<IKongAdminService, KongAdminService>();
            services.AddSingleton<ITestFileHelper, TestFileHelper>();


            services.AddSingleton<PluginConverter>();
            services.AddSingleton<IExtensionCollection>(s => new ExtensionCollection(s.GetServices<IKongPlugin>()));


            services.AddSingleton<HttpClient>();

            services.AddSingleton<ITestHelper, TestHelper>();
            //services.AddSingleton(_testTracker);

            services.AddOptions();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            services.Configure<Settings>(set => configuration.Bind(set));
            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This Method gets called by the runtime. Use this Method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
