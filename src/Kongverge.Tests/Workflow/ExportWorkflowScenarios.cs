using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Kongverge.DTOs;
using Kongverge.Helpers;
using Kongverge.Services;
using Kongverge.Workflow;
using Microsoft.Extensions.Options;
using Moq;
using TestStack.BDDfy;
using TestStack.BDDfy.Xunit;

namespace Kongverge.Tests.Workflow
{
    public class ExportWorkflowScenarios : ScenarioFor<ExportWorkflow>
    {
        protected Settings Settings;
        protected KongvergeConfiguration KongvergeConfiguration = new KongvergeConfiguration();
        protected ExitCode ExitCode;

        public ExportWorkflowScenarios()
        {
            Settings = new Fixture().Create<Settings>();
            GetMock<IOptions<Settings>>().SetupGet(x => x.Value).Returns(Settings);
            GetMock<ConfigBuilder>().Setup(x => x.FromKong(Get<IKongAdminReader>())).ReturnsAsync(KongvergeConfiguration);
        }

        [BddfyFact(DisplayName = nameof(KongIsNotReachable))]
        public void Scenario1() =>
            this.Given(s => s.KongIsNotReachable())
                .When(s => s.Executing())
                .Then(s => s.TheExitCodeIs(ExitCode.HostUnreachable))
                .BDDfy();

        [BddfyFact(DisplayName = nameof(KongIsReachable))]
        public void Scenario2() =>
            this.Given(s => s.KongIsReachable())
                .When(s => s.Executing())
                .Then(s => s.TheConfigurationIsRetrievedFromKong())
                .And(s => s.TheConfigurationIsWrittenToOutputFolder())
                .And(s => s.TheExitCodeIs(ExitCode.Success))
                .BDDfy();

        protected void KongIsNotReachable() => GetMock<IKongAdminReader>().Setup(x => x.KongIsReachable()).ReturnsAsync(false);

        protected void KongIsReachable() => GetMock<IKongAdminReader>().Setup(x => x.KongIsReachable()).ReturnsAsync(true);

        protected async Task Executing() => ExitCode = (ExitCode)await Subject.Execute();

        protected void TheExitCodeIs(ExitCode exitCode) => ExitCode.Should().Be(exitCode);

        protected void TheConfigurationIsRetrievedFromKong() => GetMock<ConfigBuilder>().Verify(x => x.FromKong(Get<IKongAdminReader>()));

        protected void TheConfigurationIsWrittenToOutputFolder() => GetMock<ConfigFileWriter>().Verify(x => x.WriteConfiguration(KongvergeConfiguration, Settings.OutputFolder));
    }
}
