using Kongverge.Common.DTOs;
using Kongverge.Common.Helpers;
using Xunit;

namespace Kongverge.Common.Tests.Helpers
{
    public class ServiceValidationHelperTests
    {
        [Fact]
        public void RoutePathsCannotBeEmpty()
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

            //Act
            var result = ServiceValidationHelper.RoutesAreValid(data);

            //Assert
            Assert.False(result);
        }
    }
}
