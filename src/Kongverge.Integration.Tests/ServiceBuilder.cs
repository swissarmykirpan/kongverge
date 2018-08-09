using System;
using System.Collections.Generic;
using Kongverge.Common.DTOs;

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
            route.Protocols = new[] { "http" };
            route.Methods = new[] { "GET" };
            _service.Routes = new List<KongRoute> { route };
            return this;
        }

        public KongService Build()
        {
            return _service;
        }
    }
}
