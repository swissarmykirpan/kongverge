using FluentAssertions;
using Kongverge;
using Kongverge.Common.DTOs;
using Kongverge.Common.Workflow;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace KongVerge.Tests
{
    public class ServiceRegistrationTests
    {
        [Fact]
        public void CanResolveKongvergeWorkflowAndDependencies()
        {
            var serviceProvider = BuildServiceProvider();

            var workflow = serviceProvider.GetService<KongvergeWorkflow>();

            workflow.Should().NotBe(null);
        }

        [Fact]
        public void CanResolveExportWorkflowAndDependencies()
        {
            var serviceProvider = BuildServiceProvider();

            var workflow = serviceProvider.GetService<ExportWorkflow>();

            workflow.Should().NotBe(null);
        }

        private static ServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();
            ServiceRegistration.AddServices(services);

            var serviceProvider = services.BuildServiceProvider();

            var configuration = serviceProvider.GetService<IOptions<Settings>>().Value;
            configuration.Admin.Host = "www.example.com";

            return serviceProvider;
        }
    }
}
