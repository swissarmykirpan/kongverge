using Kongverge.Common.Helpers;
using TestStack.BDDfy;
using TestStack.BDDfy.Xunit;

namespace Kongverge.Integration.Tests
{
    public class ProgramWorkflowSuccessScenarios : ProgramSteps
    {
        [BddfyFact(DisplayName = nameof(KongIsInABlankState) + And + nameof(AValidHost) + And + nameof(AValidPort) + And  + nameof(InputFolderIs) + A)]
        public void Scenario1() =>
            this.Given(x => x.KongIsInABlankState())
                .And(x => x.AValidHost())
                .And(x => x.AValidPort())
                .And(x => x.InputFolderIs(A))
                .When(x => x.InvokingMain())
                .And(x => x.InvokingMainAgainForExport())
                .Then(x => x.TheExitCodeIs(ExitCode.Success))
                .And(x => x.OutputFolderContentsMatchInputFolderContents())
                .TearDownWith(s => KongIsInABlankState())
                .BDDfy();

        [BddfyFact(DisplayName = nameof(KongIsInABlankState) + And + nameof(AValidHost) + And + nameof(AValidPort) + And  + nameof(InputFolderIs) + B)]
        public void Scenario2() =>
            this.Given(x => x.KongIsInABlankState())
                .And(x => x.AValidHost())
                .And(x => x.AValidPort())
                .And(x => x.InputFolderIs(B))
                .When(x => x.InvokingMain())
                .And(x => x.InvokingMainAgainForExport())
                .Then(x => x.TheExitCodeIs(ExitCode.Success))
                .And(x => x.OutputFolderContentsMatchInputFolderContents())
                .TearDownWith(s => KongIsInABlankState())
                .BDDfy();

        [BddfyFact(DisplayName = nameof(KongIsInAStateMatchingInputFolder) + A + And + nameof(AValidHost) + And + nameof(AValidPort) + And + nameof(InputFolderIs) + B)]
        public void Scenario3() =>
            this.Given(x => x.KongIsInAStateMatchingInputFolder(A))
                .And(x => x.AValidHost())
                .And(x => x.AValidPort())
                .And(x => x.InputFolderIs(B))
                .When(x => x.InvokingMain())
                .And(x => x.InvokingMainAgainForExport())
                .Then(x => x.TheExitCodeIs(ExitCode.Success))
                .And(x => x.OutputFolderContentsMatchInputFolderContents())
                .TearDownWith(s => KongIsInABlankState())
                .BDDfy();

        [BddfyFact(DisplayName = nameof(KongIsInAStateMatchingInputFolder) + B + And + nameof(AValidHost) + And + nameof(AValidPort) + And + nameof(InputFolderIs) + A)]
        public void Scenario4() =>
            this.Given(x => x.KongIsInAStateMatchingInputFolder(B))
                .And(x => x.AValidHost())
                .And(x => x.AValidPort())
                .And(x => x.InputFolderIs(A))
                .When(x => x.InvokingMain())
                .And(x => x.InvokingMainAgainForExport())
                .Then(x => x.TheExitCodeIs(ExitCode.Success))
                .And(x => x.OutputFolderContentsMatchInputFolderContents())
                .TearDownWith(s => KongIsInABlankState())
                .BDDfy();
    }
}
