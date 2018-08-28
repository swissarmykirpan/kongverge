using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Kongverge.Common.DTOs;
using Kongverge.Common.Helpers;
using Kongverge.Common.Services;
using Kongverge.Common.Workflow;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Kongverge.Common.Tests.Workflow
{
    public class ExportWorkflowTests
    {
        public class ExportWorkflowSut
        {
            private static readonly Fixture Fixture = new Fixture();

            public Mock<IDataFileHelper> MockDataFiles = new Mock<IDataFileHelper>();
            public Mock<IKongAdminReader> MockKongReader = new Mock<IKongAdminReader>();

            public Settings Settings { get; }
            public ExportWorkflow Sut { get; }

            public ExportWorkflowSut()
            {
                Settings = new Settings
                {
                    Admin = Fixture.Create<Admin>()
                };

                var configuration = new Mock<IOptions<Settings>>();
                configuration.Setup(c => c.Value).Returns(Settings);

                Sut = new ExportWorkflow(MockKongReader.Object, configuration.Object, MockDataFiles.Object);
            }
        }

        [Fact]
        public async Task CanExportStuff()
        {
            var system = new ExportWorkflowSut();

            system.MockKongReader.Setup(k => k.KongIsReachable()).ReturnsAsync(true);
            var services = new List<KongService>();
            system.MockKongReader.Setup(k => k.GetServices()).ReturnsAsync(services);

            await system.Sut.Execute();

            system.MockKongReader.Verify(k => k.GetServices(), Times.Once());
            system.MockDataFiles.Verify(f => f.WriteConfigFiles(services), Times.Once());
        }

        [Fact]
        public async Task CantExportStuff_IfKongIsntAvailable()
        {
            var system = new ExportWorkflowSut();

            system.MockKongReader.Setup(k => k.KongIsReachable()).ReturnsAsync(false);

            await system.Sut.Execute();

            system.MockKongReader.Verify(k => k.GetServices(), Times.Never());
            system.MockDataFiles.Verify(f => f.WriteConfigFiles(It.IsAny<List<KongService>>()), Times.Never());
        }
    }
}