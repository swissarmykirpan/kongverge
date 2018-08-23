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
        public void CanRegisterServices()
        {
            var services = new ServiceCollection();
            ServiceRegistration.AddServices(services);

            var serviceProvider = services.BuildServiceProvider();

            var configuration = serviceProvider.GetService<IOptions<Settings>>().Value;
            configuration.Admin.Host = "www.example.com";

            var workflow = serviceProvider.GetService<Workflow>();

            workflow.Should().NotBe(null);
        }
    }
}
