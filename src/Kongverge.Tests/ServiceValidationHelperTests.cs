using Kongverge.Common.DTOs;
using Kongverge.Common.Helpers;
using Xunit;

namespace KongVerge.Tests
{
    public class ServiceValidationHelperTests
    {
        [Fact]
        public void RoutePathsCannotBeEmpty()
        {
            //Arrange
            var data = new KongDataFile
            {
                Service = new KongService
                {
                    Routes = new[]
                    {
                        new KongRoute
                        {
                            Paths = null
                        }
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
