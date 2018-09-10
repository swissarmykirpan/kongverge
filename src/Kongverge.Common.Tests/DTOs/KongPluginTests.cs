using AutoFixture;
using FluentAssertions;
using Kongverge.Common.DTOs;
using Kongverge.TestHelpers;
using Xunit;

namespace Kongverge.Common.Tests.DTOs
{
    public class KongPluginTests : Fixture
    {
        [Fact]
        public void Equals_WithSameValues_IsTrue()
        {
            var instance = this.Create<KongPlugin>();
            var otherInstance = instance.Clone();

            instance.Equals(otherInstance).Should().BeTrue();
            instance.GetHashCode().Equals(otherInstance.GetHashCode()).Should().BeTrue();
        }

        [Fact]
        public void Equals_WithAllDifferentValues_IsFalse()
        {
            var instance = this.Create<KongPlugin>();
            var otherInstance = this.Create<KongPlugin>();

            instance.Equals(otherInstance).Should().BeFalse();
            instance.GetHashCode().Equals(otherInstance.GetHashCode()).Should().BeFalse();
        }

        [Fact]
        public void Equals_WithDifferentValuesForPersistence_IsTrue()
        {
            var instance = this.Create<KongPlugin>();
            var otherInstance = instance.Clone();

            otherInstance.Id = this.Create<string>();
            otherInstance.CreatedAt = this.Create<long>();
            otherInstance.ConsumerId = this.Create<string>();
            otherInstance.ServiceId = this.Create<string>();
            otherInstance.RouteId = this.Create<string>();

            instance.Equals(otherInstance).Should().BeTrue();
            instance.GetHashCode().Equals(otherInstance.GetHashCode()).Should().BeTrue();
        }
    }
}
