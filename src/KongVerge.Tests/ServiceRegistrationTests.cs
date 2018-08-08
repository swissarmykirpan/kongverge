using FluentAssertions;
using Kongverge;
using Microsoft.Extensions.DependencyInjection;
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

            var workflow = serviceProvider.GetService<Kongverge.Common.Workflow>();

            workflow.Should().NotBe(null);
        }
    }
}
