using Kongverge.Common.Helpers;
using Kongverge.Integration.Tests.ProgramTests;
using TestStack.BDDfy;
using TestStack.BDDfy.Xunit;

// ReSharper disable once CheckNamespace
namespace Kongverge.Integration.Tests
{
    public class ProgramArgumentsScenarios : ProgramSteps
    {
        [BddfyFact(DisplayName = nameof(NoArguments))]
        public void Scenario1() =>
            this.Given(x => x.NoArguments())
                .When(x => x.InvokingMain())
                .Then(x => x.TheExitCodeIs(ExitCode.MissingHost))
                .BDDfy();

        [BddfyFact(DisplayName = nameof(AValidHost))]
        public void Scenario2() =>
            this.Given(x => x.AValidHost())
                .When(x => x.InvokingMain())
                .Then(x => x.TheExitCodeIs(ExitCode.MissingPort))
                .BDDfy();

        [BddfyFact(DisplayName = nameof(AValidHost) + Plus + nameof(AnInvalidPort))]
        public void Scenario3() =>
            this.Given(x => x.AValidHost())
                .And(x => x.AnInvalidPort())
                .When(x => x.InvokingMain())
                .Then(x => x.TheExitCodeIs(ExitCode.InvalidPort))
                .BDDfy();

        [BddfyFact(DisplayName = nameof(AValidHost) + Plus + nameof(AValidPort) + Plus + nameof(NoInputOrOutputFolder))]
        public void Scenario4() =>
            this.Given(x => x.AValidHost())
                .And(x => x.AValidPort())
                .And(x => x.NoInputOrOutputFolder())
                .When(x => x.InvokingMain())
                .Then(x => x.TheExitCodeIs(ExitCode.IncompatibleArguments))
                .BDDfy();

        [BddfyFact(DisplayName = nameof(AValidHost) + Plus + nameof(AValidPort) + Plus + nameof(InputAndOutputFolders))]
        public void Scenario5() =>
            this.Given(x => x.AValidHost())
                .And(x => x.AValidPort())
                .And(x => x.InputAndOutputFolders())
                .When(x => x.InvokingMain())
                .Then(x => x.TheExitCodeIs(ExitCode.IncompatibleArguments))
                .BDDfy();

        [BddfyFact(DisplayName = nameof(AValidHost) + Plus + nameof(AValidPort) + Plus + nameof(InputFolderIs) + NonExistentInputFolder)]
        public void Scenario6() =>
            this.Given(x => x.AValidHost())
                .And(x => x.AValidPort())
                .And(x => x.InputFolderIs(NonExistentInputFolder))
                .When(x => x.InvokingMain())
                .Then(x => x.TheExitCodeIs(ExitCode.InputFolderUnreachable))
                .BDDfy();

        [BddfyFact(DisplayName = nameof(AValidHost) + Plus + nameof(AValidPort) + Plus + nameof(InputFolderIs) + InputFolderInvalidData)]
        public void Scenario7() =>
            this.Given(x => x.AValidHost())
                .And(x => x.AValidPort())
                .And(x => x.InputFolderIs(InputFolderInvalidData))
                .When(x => x.InvokingMain())
                .Then(x => x.TheExitCodeIs(ExitCode.InvalidConfigurationFile))
                .BDDfy();

        [BddfyFact(DisplayName = nameof(AValidHost) + Plus + nameof(AValidPort) + Plus + nameof(InputFolderIs) + InputFolderBadFormat)]
        public void Scenario8() =>
            this.Given(x => x.AValidHost())
                .And(x => x.AValidPort())
                .And(x => x.InputFolderIs(InputFolderBadFormat))
                .When(x => x.InvokingMain())
                .Then(x => x.TheExitCodeIs(ExitCode.InvalidConfigurationFile))
                .BDDfy();
    }
}
