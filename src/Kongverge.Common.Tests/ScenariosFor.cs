using Specify;
using Specify.Mocks;
using TinyIoC;

namespace Kongverge.Common.Tests
{
    public abstract class ScenariosFor<TSut> : Specify.ScenarioFor<TSut>
        where TSut : class
    {
        public ScenariosFor() => SetContainer(new TinyMockingContainer(new MoqMockFactory(), new TinyIoCContainer()));

        public override string Title => this.GetTitle();
    }
}
