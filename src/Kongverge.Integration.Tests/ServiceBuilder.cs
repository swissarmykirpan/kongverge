using System;
using System.Collections.Generic;
using System.Linq;
using Kongverge.Common.DTOs;
using Kongverge.KongPlugin;

namespace Kongverge.Integration.Tests
{
    public class ServiceBuilder
    {
        private readonly KongService _service = new KongService();

        public ServiceBuilder AddDefaultTestService(string testIdentifier = null)
        {
            _service.Name = string.IsNullOrWhiteSpace(testIdentifier) ?
                $"testservice_{Guid.NewGuid().ToString()}" :
                $"testservice_{testIdentifier}_{Guid.NewGuid().ToString()}";
            _service.Host = "www.example.com";
            _service.Port = 80;
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

            if (_service.Routes == null)
            {
                _service.Routes = new List<KongRoute> { route };
            }
            else
            {
                _service.Routes = _service.Routes.Concat(new[] { route }).ToList();
            }
            return this;
        }

        public ServiceBuilder WithPlugin(IKongPluginConfig plugin)
        {
            if (_service.Plugins == null)
            {
                _service.Plugins = new List<IKongPluginConfig>();
            }
            _service.Plugins.Add(plugin);
            return this;
        }


        public ServiceBuilder WithRoutePlugin(IKongPluginConfig plugin)
        {
            var route = _service.Routes.First();

            if (route.Plugins == null)
            {
                route.Plugins = new List<IKongPluginConfig>();
            }
            route.Plugins.Add(plugin);
            return this;
        }

        public KongService Build()
        {
            return _service;
        }
    }
}
