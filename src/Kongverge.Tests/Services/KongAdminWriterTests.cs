using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Kongverge.DTOs;
using Kongverge.Services;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Kongverge.Tests.Services
{
    public class KongAdminWriterTests
    {
        private readonly Mock<FakeHttpMessageHandler> _fakeHttpMessageHandler = new Mock<FakeHttpMessageHandler> { CallBase = true };

        private KongAdminHttpClient MakeKongAdminHttpClient() => new KongAdminHttpClient(_fakeHttpMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };

        [Fact]
        public async Task KongIsReachableReturnsTrue()
        {
            //Arrange
            _fakeHttpMessageHandler.Setup(x => x.Send(It.IsAny<HttpRequestMessage>()))
                .Returns(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("")
                });

            var httpClient = MakeKongAdminHttpClient();
            var sut = new KongAdminWriter(httpClient);

            //Act
            var result = await sut.KongIsReachable();

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task PoorlyKongReturnsFalse()
        {
            //Arrange
            _fakeHttpMessageHandler.Setup(x => x.Send(It.IsAny<HttpRequestMessage>()))
                .Returns(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadGateway,
                    Content = new StringContent("")
                });

            var httpClient = MakeKongAdminHttpClient();
            var sut = new KongAdminWriter(httpClient);

            //Act
            var result = await sut.KongIsReachable();

            //Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UnavailableKongReturnsFalse()
        {
            //Arrange
            _fakeHttpMessageHandler.Setup(x => x.Send(It.IsAny<HttpRequestMessage>())).Throws<HttpRequestException>();

            var httpClient = MakeKongAdminHttpClient();
            var sut = new KongAdminWriter(httpClient);

            //Act
            var result = await sut.KongIsReachable();

            //Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetServicesReturnsServices()
        {
            //Arrange
            _fakeHttpMessageHandler.Setup(x => x.Send(It.IsAny<HttpRequestMessage>()))
                .Returns(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        "{\"next\": null,\"data\": [{\"host\": \"www.example.com\",\"created_at\": 1529492319,\"connect_timeout\": 300,\"id\": \"a64ef803-b719-432a-a5ef-8e9d76e1a788\",\"protocol\": \"http\",\"name\": \"healthcheck\",\"read_timeout\": 1500,\"port\": 80,\"path\": null,\"updated_at\": 1529494361,\"retries\": 5,\"write_timeout\": 100}]}")
                });

            var expected = new[]
            {
                new KongService
                {
                    Host = "www.example.com",
                    ConnectTimeout = 300,
                    Protocol = "http",
                    Name = "healthcheck",
                    ReadTimeout = 1500,
                    Port = 80,
                    Path = null,
                    Retries = 5,
                    WriteTimeout = 100
                }
            };

            var httpClient = MakeKongAdminHttpClient();
            var sut = new KongAdminWriter(httpClient);

            //Act
            var result = await sut.GetServices();

            //Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task AddRouteWithStripPathSetToFalse_ShouldDeserializeCorrectly()
        {
            var route = new KongRoute
            {
                Paths = new[] { "/test" },
                StripPath = false
            };

            StringContent content = null;
            var successResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(route))
            };

            _fakeHttpMessageHandler.Setup(x => x.Send(It.IsAny<HttpRequestMessage>()))
                .Callback<HttpRequestMessage>(x => content = (StringContent)x.Content)
                .Returns(successResponse);

            var httpClient = MakeKongAdminHttpClient();
            var sut = new KongAdminWriter(httpClient);

            //Act
            await sut.AddRoute(Guid.NewGuid().ToString(), route);

            //Assert
            route.Plugins = null;
            var routeJson = JsonConvert.SerializeObject(route);
            var json = await content.ReadAsStringAsync();
            routeJson.Should().Be(json);
        }
    }
}
