using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Kongverge.Common.DTOs;
using Kongverge.Common.Helpers;
using TestStack.BDDfy;
using TestStack.BDDfy.Xunit;

namespace Kongverge.Common.Tests.DTOs
{
    public class KongServiceEqualityScenarios : EqualityScenarios<KongService>
    {
        protected override void OnlyThePersistenceValuesAreDifferent()
        {
            OtherInstance.Id = this.Create<string>();
            OtherInstance.CreatedAt = this.Create<long>();
            OtherInstance.ValidateHost = !OtherInstance.ValidateHost;
        }
    }

    public class KongServiceValidationScenarios : Fixture
    {
        protected const string And = "_";

        protected KongService Instance;
        protected ICollection<string> ErrorMessages = new List<string>();

        [BddfyFact(DisplayName = nameof(ARandomInstance) + And + nameof(ValidateHostIs) + "True" + And + nameof(HostIsReachable))]
        public void Scenario1() =>
            this.Given(x => x.ARandomInstance())
                .And(x => ValidateHostIs(true))
                .And(x => HostIsReachable())
                .When(x => x.Validating())
                .Then(x => x.ItIsValid())
                .BDDfy();

        [BddfyFact(DisplayName = nameof(ARandomInstance) + And + nameof(ValidateHostIs) + "True" + And + nameof(HostIsNotReachable))]
        public void Scenario2() =>
            this.Given(x => x.ARandomInstance())
                .And(x => ValidateHostIs(true))
                .And(x => HostIsNotReachable())
                .When(x => x.Validating())
                .Then(x => x.ItIsInvalid())
                .BDDfy();

        [BddfyFact(DisplayName = nameof(ARandomInstance) + And + nameof(ValidateHostIs) + "False")]
        public void Scenario3() =>
            this.Given(x => x.ARandomInstance())
                .And(x => ValidateHostIs(false))
                .When(x => x.Validating())
                .Then(x => x.ItIsValid())
                .BDDfy();

        [BddfyFact(DisplayName = nameof(ARandomInstance) + And + nameof(ValidateHostIs) + "False" + And + nameof(RoutePathsAreEmpty))]
        public void Scenario4() =>
            this.Given(x => x.ARandomInstance())
                .And(x => ValidateHostIs(false))
                .And(x => x.RoutePathsAreEmpty())
                .When(x => x.Validating())
                .Then(x => x.ItIsInvalid())
                .BDDfy();

        protected void ARandomInstance() => Instance = this.Create<KongService>();

        protected void ValidateHostIs(bool value) => Instance.ValidateHost = value;

        protected void HostIsReachable()
        {
            Instance.Host = "www.google.com";
            Instance.Port = 80;
        }

        protected void HostIsNotReachable() => Instance.Host = this.Create<string>();

        protected void RoutePathsAreEmpty() => Instance.Routes[0].Paths = null;

        protected Task Validating() => Instance.Validate(ErrorMessages);

        protected void ItIsValid() => ErrorMessages.Count.Should().Be(0);

        protected void ItIsInvalid() => ErrorMessages.Count.Should().BeGreaterThan(0);
    }

    public class KongServiceSerializationScenarios : SerializationScenarios<KongService>
    {
        [BddfyFact(DisplayName = nameof(ARandomInstance) + And + nameof(SerializingToStringContent))]
        public void Scenario2() =>
            this.Given(x => x.ARandomInstance())
                .When(x => x.SerializingToStringContent())
                .Then(x => x.ValidateHostIsNotSerialized())
                .And(x => x.RoutesIsNotSerialized())
                .And(x => x.PluginsIsNotSerialized())
                .And(x => x.ValidateHostIsNotNull())
                .And(x => x.PluginsIsNotNull())
                .And(x => x.RoutesIsNotNull())
                .BDDfy();

        protected override StringContent MakeStringContent() => Instance.ToJsonStringContent();

        protected override string SerializingToConfigJson() => Serialized = Instance.ToConfigJson();

        protected void ValidateHostIsNotSerialized() => Serialized.Contains("\"validate-host\":").Should().BeFalse();

        protected void PluginsIsNotSerialized() => Serialized.Contains("\"plugins\":").Should().BeFalse();

        protected void RoutesIsNotSerialized() => Serialized.Contains("\"routes\":").Should().BeFalse();

        protected void ValidateHostIsNotNull() => Instance.ValidateHost.Should().NotBeNull();

        protected void PluginsIsNotNull() => Instance.Plugins.Should().NotBeNull();

        protected void RoutesIsNotNull() => Instance.Routes.Should().NotBeNull();
    }
}
