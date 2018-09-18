using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Kongverge.Common.DTOs;
using TestStack.BDDfy;
using TestStack.BDDfy.Xunit;

namespace Kongverge.Common.Tests.DTOs
{
    public class KongPluginEqualityScenarios : EqualityScenarios<KongPlugin>
    {
        [BddfyFact(DisplayName = nameof(ARandomInstance) + And + nameof(AnotherInstanceClonedFromTheFirst) + And + nameof(ConfigValuesAreShuffled))]
        public void Scenario4() =>
            this.Given(x => x.ARandomInstance())
                .And(x => x.AnotherInstanceClonedFromTheFirst())
                .And(x => x.ConfigValuesAreShuffled())
                .When(x => x.CheckingEquality())
                .And(x => x.CheckingHashCodes())
                .Then(x => x.TheyAreEqual())
                .BDDfy();

        protected void ConfigValuesAreShuffled()
        {
            OtherInstance.Config = new Dictionary<string, object>(OtherInstance.Config.Reverse());
        }

        protected override void OnlyThePersistenceValuesAreDifferent()
        {
            OtherInstance.Id = this.Create<string>();
            OtherInstance.CreatedAt = this.Create<long>();
            OtherInstance.ConsumerId = this.Create<string>();
            OtherInstance.ServiceId = this.Create<string>();
            OtherInstance.RouteId = this.Create<string>();
        }
    }
}
