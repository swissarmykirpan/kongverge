using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Kongverge.Common.DTOs;
using Kongverge.KongPlugin;

namespace Kongverge.Integration.Tests
{
    public class ServiceBuilder
    {
        private readonly string _testServiceNamePrefix;
        private KongService _service;

        public ServiceBuilder(string testServiceNamePrefix)
        {
            _testServiceNamePrefix = testServiceNamePrefix;
        }

        public ServiceBuilder CreateDefaultTestService(string testIdentifier = null)
        {
            _service = new KongService
            {
                Name = string.IsNullOrWhiteSpace(testIdentifier)
                    ? $"{_testServiceNamePrefix}{Guid.NewGuid().ToString()}"
                    : $"{_testServiceNamePrefix}{testIdentifier}_{Guid.NewGuid().ToString()}",
                Host = "www.example.com",
                Port = 80
            };
            return this;
        }

        public ServiceBuilder WithRoutePaths(params string[] paths)
        {
            var route = new KongRoute
            {
                Paths = paths,
                Protocols = new[] { "http" },
                Methods = new[] { "GET" }
            };

            _service.Routes = _service.Routes.Concat(new[] { route }).ToArray();
            return this;
        }

        public ServiceBuilder WithPlugin(IKongPluginConfig plugin)
        {
            _service.Plugins = _service.Plugins.Concat(new [] { plugin }).ToArray();
            return this;
        }
        
        public ServiceBuilder WithRoutePlugin(IKongPluginConfig plugin)
        {
            var route = _service.Routes.First();

            route.Plugins = route.Plugins.Concat(new [] { plugin }).ToArray();
            return this;
        }

        public KongService Build()
        {
            return _service;
        }
    }
}
