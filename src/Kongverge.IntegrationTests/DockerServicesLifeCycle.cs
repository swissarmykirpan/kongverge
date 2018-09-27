using System;
using System.Diagnostics;
using System.Threading;
using Xunit;

namespace Kongverge.IntegrationTests
{
    [CollectionDefinition(ProgramSteps.Host, DisableParallelization = true)]
    public class DockerServicesLifecycle : IDisposable, ICollectionFixture<DockerServicesLifecycle>
    {
        public DockerServicesLifecycle()
        {
            Process.Start("docker-compose", "start").WaitForExit();
            Thread.Sleep(3000);
        }

        public void Dispose()
        {
            Process.Start("docker-compose", "stop").WaitForExit();
        }
    }
}
