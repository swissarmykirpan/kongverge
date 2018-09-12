using Kongverge.Common.Helpers;
using Kongverge.Integration.Tests.ProgramTests;
using TestStack.BDDfy;
using TestStack.BDDfy.Xunit;

// ReSharper disable once CheckNamespace
namespace Kongverge.Integration.Tests
{
    public class ProgramWorkflowScenarios : ProgramSteps
    {
        public ProgramWorkflowScenarios()
        {
            KongIsInABlankState();
        }

        [BddfyFact(DisplayName = nameof(AValidHost) + Plus + nameof(AValidPort) + Plus  + nameof(InputFolderIs) + InputFolder1)]
        public void Scenario1() =>
            this.Given(x => x.AValidHost())
                .And(x => x.AValidPort())
                .And(x => x.InputFolderIs(InputFolder1))
                .When(x => x.InvokingMain())
                .And(x => x.InvokingMainAgainForExport())
                .Then(x => x.TheExitCodeIs(ExitCode.Success))
                .And(x => x.OutputFolderContentsMatchInputFolderContents())
                .TearDownWith(s => KongIsInABlankState())
                .BDDfy();

        [BddfyFact(DisplayName = nameof(AValidHost) + Plus + nameof(AValidPort) + Plus  + nameof(InputFolderIs) + InputFolder2)]
        public void Scenario2() =>
            this.Given(x => x.AValidHost())
                .And(x => x.AValidPort())
                .And(x => x.InputFolderIs(InputFolder2))
                .When(x => x.InvokingMain())
                .And(x => x.InvokingMainAgainForExport())
                .Then(x => x.TheExitCodeIs(ExitCode.Success))
                .And(x => x.OutputFolderContentsMatchInputFolderContents())
                .TearDownWith(s => KongIsInABlankState())
                .BDDfy();

        [BddfyFact(DisplayName = nameof(KongIsInAStateMatchingInputFolder) + InputFolder1 + Plus + nameof(AValidHost) + Plus + nameof(AValidPort) + Plus  + nameof(InputFolderIs) + InputFolder2)]
        public void Scenario3() =>
            this.Given(x => x.KongIsInAStateMatchingInputFolder(InputFolder1))
                .And(x => x.AValidHost())
                .And(x => x.AValidPort())
                .And(x => x.InputFolderIs(InputFolder2))
                .When(x => x.InvokingMain())
                .And(x => x.InvokingMainAgainForExport())
                .Then(x => x.TheExitCodeIs(ExitCode.Success))
                .And(x => x.OutputFolderContentsMatchInputFolderContents())
                .TearDownWith(s => KongIsInABlankState())
                .BDDfy();

        [BddfyFact(DisplayName = nameof(KongIsInAStateMatchingInputFolder) + InputFolder2 + Plus + nameof(AValidHost) + Plus + nameof(AValidPort) + Plus  + nameof(InputFolderIs) + InputFolder1)]
        public void Scenario4() =>
            this.Given(x => x.KongIsInAStateMatchingInputFolder(InputFolder2))
                .And(x => x.AValidHost())
                .And(x => x.AValidPort())
                .And(x => x.InputFolderIs(InputFolder1))
                .When(x => x.InvokingMain())
                .And(x => x.InvokingMainAgainForExport())
                .Then(x => x.TheExitCodeIs(ExitCode.Success))
                .And(x => x.OutputFolderContentsMatchInputFolderContents())
                .TearDownWith(s => KongIsInABlankState())
                .BDDfy();
    }
}
