using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Kongverge.Common.DTOs;
using Kongverge.TestHelpers;

namespace Kongverge.Common.Tests.Workflow
{
    public static class TestRoutes
    {
        public static IReadOnlyList<KongRoute> CreateRoutes(this Fixture fixture, int count) =>
            Enumerable.Range(0, count).Select(x => fixture.CreateRoute()).ToArray();

        private static KongRoute CreateRoute(this Fixture fixture) =>
            fixture
                .Build<KongRoute>()
                .Without(x => x.Id)
                .Without(x => x.Plugins)
                .Create();

        public static KongRoute AsExisting(this KongRoute kongRoute) =>
            kongRoute.Clone().WithId();

        public static KongRoute WithId(this KongRoute kongRoute)
        {
            kongRoute.Id = Guid.NewGuid().ToString();
            return kongRoute;
        }

        public static KongRoute AsTarget(this KongRoute kongRoute, bool modified = false)
        {
            var target = kongRoute.Clone();
            if (modified)
            {
                target.Hosts = new [] { Guid.NewGuid().ToString() };
            }
            return target;
        }
    }
}