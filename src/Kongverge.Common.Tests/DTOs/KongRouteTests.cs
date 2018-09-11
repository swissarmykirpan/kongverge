using System.Linq;
using AutoFixture;
using FluentAssertions;
using Kongverge.Common.DTOs;
using Kongverge.TestHelpers;
using Xunit;

namespace Kongverge.Common.Tests.DTOs
{
    public class KongRouteTests : Fixture
    {
        [Fact]
        public void Equals_WithSameValues_IsTrue()
        {
            var instance = this.Create<KongRoute>();
            var otherInstance = instance.Clone();

            instance.Equals(otherInstance).Should().BeTrue();
            instance.GetHashCode().Equals(otherInstance.GetHashCode()).Should().BeTrue();
        }

        [Fact]
        public void Equals_WithSameValuesInDifferentOrder_IsTrue()
        {
            var hosts = new[]
            {
                this.Create<string>(),
                this.Create<string>()
            };
            var protocols = new[]
            {
                this.Create<string>(),
                this.Create<string>()
            };
            var methods = new[]
            {
                this.Create<string>(),
                this.Create<string>()
            };
            var paths = new[]
            {
                this.Create<string>(),
                this.Create<string>()
            };
            var instance = Build<KongRoute>()
                .With(x => x.Hosts, hosts)
                .With(x => x.Protocols, protocols)
                .With(x => x.Methods, methods)
                .With(x => x.Paths, paths)
                .Create();
            var otherInstance = instance.Clone();
            otherInstance.Hosts = hosts.Reverse();
            otherInstance.Protocols = protocols.Reverse();
            otherInstance.Methods = methods.Reverse();
            otherInstance.Paths = paths.Reverse();

            instance.Equals(otherInstance).Should().BeTrue();
            instance.GetHashCode().Equals(otherInstance.GetHashCode()).Should().BeTrue();
        }

        [Fact]
        public void Equals_WithAllDifferentValues_IsFalse()
        {
            var instance = this.Create<KongRoute>();
            var otherInstance = this.Create<KongRoute>();

            instance.Equals(otherInstance).Should().BeFalse();
            instance.GetHashCode().Equals(otherInstance.GetHashCode()).Should().BeFalse();
        }

        [Fact]
        public void Equals_WithDifferentValuesForPersistence_IsTrue()
        {
            var instance = this.Create<KongRoute>();
            var otherInstance = instance.Clone();

            otherInstance.Id = this.Create<string>();
            otherInstance.Service = this.Create<KongRoute.ServiceReference>();
            otherInstance.CreatedAt = this.Create<long>();

            instance.Equals(otherInstance).Should().BeTrue();
            instance.GetHashCode().Equals(otherInstance.GetHashCode()).Should().BeTrue();
        }
    }
}
