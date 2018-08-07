using System;
using System.Collections.Generic;
using Kongverge.Common.DTOs;
using Kongverge.KongPlugin;

namespace Kongverge.Integration.Tests
{
    public class ServiceBuilder
    {
        private readonly KongService _service = new KongService();

        public ServiceBuilder AddDefaultTestService()
        {
            _service.Name = $"testservice_{Guid.NewGuid().ToString()}";
            _service.Host = "www.example.com";
            _service.Port = 80;
            return this;
        }

        public ServiceBuilder WithHost(string host)
        {
            _service.Host = host;
            return this;
        }

        public ServiceBuilder WithPaths(params string[] paths)
        {
            var route = new KongRoute { Paths = paths };
            _service.Routes = new List<KongRoute> { route };
            return this;
        }

        public ServiceBuilder WithPlugin(IKongPluginConfig plugin)
        {
            if (_service.Extensions == null)
            {
                _service.Extensions = new List<IKongPluginConfig>();
            }
            _service.Extensions.Add(plugin);
            return this;
        }

        public KongService Build()
        {
            return _service;
        }
    }
}
