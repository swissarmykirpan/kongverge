using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Kongverge.Common.DTOs;
using Kongverge.Common.Plugins;
using Kongverge.Common.Services;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace KongVerge.Tests
{
    public class KongAdminWriterTests
    {
        private readonly Mock<FakeHttpMessageHandler> _fakeHttpMessageHandler =
            new Mock<FakeHttpMessageHandler> {CallBase = true};

        private readonly IOptions<Settings> _configuration = Options.Create(new Settings
        {
            Admin = new Admin
            {
                Host = "localhost",
                Port = 8001
            }
        });

        [Fact]
        public async Task KongIsReachableReturnsTrue()
        {
            //Arrange
            _fakeHttpMessageHandler.Setup(f => f.Send(It.IsAny<HttpRequestMessage>()))
                .Returns(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("")
                });

            var kongPluginCollection = new Mock<IKongPluginCollection>();
            var httpClient = new HttpClient(_fakeHttpMessageHandler.Object);
            var sut = new KongAdminWriter(_configuration, httpClient, kongPluginCollection.Object, null);

            //Act
            var result = await sut.KongIsReachable().ConfigureAwait(false);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task PoorlyKongReturnsFalse()
        {
            //Arrange
            _fakeHttpMessageHandler.Setup(f => f.Send(It.IsAny<HttpRequestMessage>()))
                .Returns(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadGateway,
                    Content = new StringContent("")
                });

            var kongPluginCollection = new Mock<IKongPluginCollection>();
            var httpClient = new HttpClient(_fakeHttpMessageHandler.Object);
            var sut = new KongAdminWriter(_configuration, httpClient, kongPluginCollection.Object, null);

            //Act
            var result = await sut.KongIsReachable().ConfigureAwait(false);

            //Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UnavailableKongReturnsFalse()
        {
            //Arrange
            _fakeHttpMessageHandler.Setup(f => f.Send(It.IsAny<HttpRequestMessage>())).Throws<HttpRequestException>();

            var kongPluginCollection = new Mock<IKongPluginCollection>();
            var httpClient = new HttpClient(_fakeHttpMessageHandler.Object);
            var sut = new KongAdminWriter(_configuration, httpClient, kongPluginCollection.Object, null);

            //Act
            var result = await sut.KongIsReachable().ConfigureAwait(false);

            //Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetServicesReturnsServices()
        {
            //Arrange
            _fakeHttpMessageHandler.Setup(f => f.Send(It.IsAny<HttpRequestMessage>()))
                .Returns(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        "{\"next\": null,\"data\": [{\"host\": \"www.example.com\",\"created_at\": 1529492319,\"connect_timeout\": 300,\"id\": \"a64ef803-b719-432a-a5ef-8e9d76e1a788\",\"protocol\": \"http\",\"name\": \"healthcheck\",\"read_timeout\": 1500,\"port\": 80,\"path\": null,\"updated_at\": 1529494361,\"retries\": 5,\"write_timeout\": 100}]}")
                });

            var expected = new List<KongService>
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

            var kongPluginCollection = new Mock<IKongPluginCollection>();
            var httpClient = new HttpClient(_fakeHttpMessageHandler.Object);
            var sut = new KongAdminWriter(_configuration, httpClient, kongPluginCollection.Object, null);

            //Act
            var result = await sut
                .GetServices()
                .ConfigureAwait(false);

            //Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task AddRouteWithStripPathSetToFalse_ShouldDeserializeCorrectly()
        {
            var service = new KongService
            {
                Host = "www.example.com",
                Name = "testservice",
            };

            var route = new KongRoute
            {
                Paths = new[] {"/test"},
                StripPath = false
            };

            StringContent content = null;
            var successResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("")
            };

            _fakeHttpMessageHandler.Setup(f => f.Send(It.IsAny<HttpRequestMessage>()))
                .Callback<HttpRequestMessage>(r => content = (StringContent) r.Content)
                .Returns(successResponse);


            var kongPluginCollection = new Mock<IKongPluginCollection>();
            var httpClient = new HttpClient(_fakeHttpMessageHandler.Object);
            var sut = new KongAdminWriter(_configuration, httpClient, kongPluginCollection.Object, null);

            //Act
            await sut.AddRoute(service, route);

            //Assert
            var serializerSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            var routeJson = JsonConvert.SerializeObject(route, serializerSettings);
            var json = await content.ReadAsStringAsync();
            routeJson.Should().Be(json);
        }
    }
}
