using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Kongverge.Common.DTOs;
using Kongverge.TestHelpers;
using Xunit;

namespace Kongverge.Common.Tests.DTOs
{
    public class KongServiceTests : Fixture
    {
        [Fact]
        public void Equals_WithSameValues_IsTrue()
        {
            var instance = this.Create<KongService>();
            var otherInstance = instance.Clone();

            instance.Equals(otherInstance).Should().BeTrue();
            instance.GetHashCode().Equals(otherInstance.GetHashCode()).Should().BeTrue();
        }

        [Fact]
        public void Equals_WithAllDifferentValues_IsFalse()
        {
            var instance = this.Create<KongService>();
            var otherInstance = this.Create<KongService>();

            instance.Equals(otherInstance).Should().BeFalse();
            instance.GetHashCode().Equals(otherInstance.GetHashCode()).Should().BeFalse();
        }

        [Fact]
        public void Equals_WithDifferentValuesForPersistence_IsTrue()
        {
            var instance = this.Create<KongService>();
            var otherInstance = instance.Clone();

            otherInstance.Id = this.Create<string>();
            otherInstance.CreatedAt = this.Create<long>();
            otherInstance.ValidateHost = !instance.ValidateHost;

            instance.Equals(otherInstance).Should().BeTrue();
            instance.GetHashCode().Equals(otherInstance.GetHashCode()).Should().BeTrue();
        }

        [Fact]
        public async Task RoutePathsCannotBeEmpty()
        {
            //Arrange
            var data = new KongService
            {
                Routes = new[]
                {
                    new KongRoute
                    {
                        Paths = null
                    }
                }
            };

            var errorMessages = new List<string>();

            //Act
            await data.Validate(errorMessages);

            //Assert
            errorMessages.Count.Should().BeGreaterThan(0);
        }
    }
}
