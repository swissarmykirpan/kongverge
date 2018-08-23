using Kongverge.Common.DTOs;
using Xunit;

namespace KongVerge.Tests
{
    public class KongRouteTests
    {
        [Fact]
        public void EqualsImplementedCorrectly()
        {
            //Arrange
            var route1 = new KongRoute
            {
                Id = "ID1",
                Hosts = new[] { "example.com" },
                Methods = new[] { "GET", "POST" },
                Paths = new[] { "/path" },
                Protocols = new[] { "http", "https" },
                RegexPriority = 4,
                StripPath = false
            };

            var route2 = new KongRoute
            {
                Id = "ID2",
                Hosts = new[] { "example.com" },
                Methods = new[] { "GET", "POST" },
                Paths = new[] { "/path" },
                Protocols = new[] { "http", "https" },
                RegexPriority = 4,
                StripPath = false
            };

            //Act
            var result1 = route1.GetHashCode();
            var result2 = route2.GetHashCode();

            //Assert
            Assert.Equal(result1, result2);
            Assert.Equal(route1, route2);
        }
    }
}
