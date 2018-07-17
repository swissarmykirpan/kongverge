using Kongverge;
using Kongverge.Common.DTOs;
using Kongverge.Common.Helpers;
using Kongverge.Common.Services;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using AutoFixture;

namespace KongVerge.Tests.Workflow
{
    public class ExportWorkflowTests
    {
        public class ExportWorkflowSut
        {
            private static Fixture _fixture = new Fixture();

            public Mock<IDataFileHelper> mockDataFiles = new Mock<IDataFileHelper>();
            public Mock<IKongAdminReadService> mockKongService = new Mock<IKongAdminReadService>();

            public Settings Settings { get; }
            public ExportWorkflow Sut { get; }

            public ExportWorkflowSut()
            {
                Settings = new Settings()
                {
                    Admin = _fixture.Create<Admin>()
                };

                var configuration = new Mock<IOptions<Settings>>();
                configuration.Setup(c => c.Value).Returns(Settings);

                Sut = new ExportWorkflow(mockKongService.Object, mockDataFiles.Object, configuration.Object);
            }
        }

        [Fact]
        public async Task CanExportStuff()
        {
            var system = new ExportWorkflowSut();

            system.mockKongService.Setup(k => k.KongIsReachable()).ReturnsAsync(true);
            var services = new List<KongService>();
            system.mockKongService.Setup(k => k.GetServices()).ReturnsAsync(services);

            await system.Sut.Execute();

            system.mockKongService.Verify(k => k.GetServices(), Times.Once());
            system.mockDataFiles.Verify(f => f.WriteConfigFiles(services), Times.Once());
        }

        [Fact]
        public async Task CantExportStuff_IfKongIsntAvailable()
        {
            var system = new ExportWorkflowSut();

            system.mockKongService.Setup(k => k.KongIsReachable()).ReturnsAsync(false);

            await system.Sut.Execute();

            system.mockKongService.Verify(k => k.GetServices(), Times.Never());
            system.mockDataFiles.Verify(f => f.WriteConfigFiles(It.IsAny<List<KongService>>()), Times.Never());
        }
    }
}
