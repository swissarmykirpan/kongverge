using System.Linq;
using Kongverge.Common.DTOs;
using Kongverge.Common.Helpers;
using Xunit;

namespace Kongverge.Common.Tests.Helpers
{
    public class ComparisonExtensionsTests
    {
        [Fact]
        public void DetailedCompareReturnsDiff()
        {
            //Arrange
            var obj1Paths = new[] { "obj1" };
            var obj1 = new KongRoute
            {
                Hosts = new[] { "example.com" },
                Paths = obj1Paths,
                RegexPriority = 5,
                StripPath = false
            };

            var obj2Paths = new[] { "obj2" };
            var obj2 = new KongRoute
            {
                Hosts = new[] { "example.com" },
                Paths = obj2Paths,
                RegexPriority = 3,
                StripPath = false
            };

            //Act
            var result = obj1.DetailedCompare(obj2);

            //Assert
            var regexPriority = result.First(x => x.Field == "RegexPriority");
            Assert.Equal(5, regexPriority.Existing);
            Assert.Equal(3, regexPriority.New);

            var paths = result.First(x => x.Field == "Paths");
            Assert.Equal(obj1Paths, paths.Existing);
            Assert.Equal(obj2Paths, paths.New);
        }
    }
}
