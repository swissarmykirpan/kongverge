using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Kongverge.Common.DTOs;
using Kongverge.TestHelpers;

namespace Kongverge.Common.Tests.Workflow
{
    public static class TestPlugins
    {
        public static IReadOnlyList<KongPlugin> CreatePlugins(this Fixture fixture, int count) =>
            Enumerable.Range(0, count).Select(x => fixture.CreatePlugin()).ToArray();

        private static KongPlugin CreatePlugin(this Fixture fixture) =>
            fixture
                .Build<KongPlugin>()
                .Without(x => x.Id)
                .Create();

        public static bool IsTheSameAs(this KongPlugin kongPlugin, KongPlugin expectedKongPlugin) =>
            ReferenceEquals(kongPlugin, expectedKongPlugin);

        public static bool CorrespondsToKongService(this KongPlugin kongPlugin, KongService kongService) =>
            kongPlugin.ServiceId == kongService.Id &&
            !string.IsNullOrWhiteSpace(kongPlugin.ServiceId) &&
            kongPlugin.ConsumerId == null &&
            kongPlugin.RouteId == null;

        public static bool CorrespondsToKongRoute(this KongPlugin kongPlugin, KongRoute kongRoute) =>
            kongPlugin.RouteId == kongRoute.Id &&
            !string.IsNullOrWhiteSpace(kongPlugin.RouteId) &&
            kongPlugin.ConsumerId == null &&
            kongPlugin.ServiceId == null;

        public static KongPlugin AsExisting(this KongPlugin kongPlugin) =>
            kongPlugin.Clone().WithId();

        public static KongPlugin WithId(this KongPlugin kongPlugin)
        {
            if (kongPlugin.Id == null)
            {
                kongPlugin.Id = Guid.NewGuid().ToString();
            }
            return kongPlugin;
        }

        public static KongPlugin AsTarget(this KongPlugin kongPlugin, bool modified = false)
        {
            var target = kongPlugin.Clone();
            if (modified)
            {
                if (target.Config.Any())
                {
                    target.Config[target.Config.Keys.First()] = Guid.NewGuid().ToString();
                }
                else
                {
                    target.Config.Add(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
                }
            }
            return target;
        }
    }
}
