using System.Linq;
using AutoFixture;
using Kongverge.Common.DTOs;
using TestStack.BDDfy;
using TestStack.BDDfy.Xunit;

namespace Kongverge.Common.Tests.DTOs
{
    public class KongRouteEqualityScenarios : EqualityScenarios<KongRoute>
    {
        [BddfyFact(DisplayName = nameof(ARandomInstance) + And + nameof(AnotherInstanceClonedFromTheFirst) + And + nameof(ListValuesAreShuffled))]
        public void Scenario4() =>
            this.Given(x => x.ARandomInstance())
                .And(x => x.AnotherInstanceClonedFromTheFirst())
                .And(x => x.ListValuesAreShuffled())
                .When(x => x.CheckingEquality())
                .And(x => x.CheckingHashCodes())
                .Then(x => x.TheyAreEqual())
                .BDDfy();

        protected void ListValuesAreShuffled()
        {
            OtherInstance.Hosts = OtherInstance.Hosts.Reverse();
            OtherInstance.Protocols = OtherInstance.Protocols.Reverse();
            OtherInstance.Methods = OtherInstance.Methods.Reverse();
            OtherInstance.Paths = OtherInstance.Paths.Reverse();
        }

        protected override void OnlyThePersistenceValuesAreDifferent()
        {
            OtherInstance.Id = this.Create<string>();
            OtherInstance.Service = this.Create<KongRoute.ServiceReference>();
            OtherInstance.CreatedAt = this.Create<long>();
        }
    }
}
