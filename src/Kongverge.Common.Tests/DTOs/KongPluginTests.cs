using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Kongverge.Common.DTOs;
using Xunit;

namespace Kongverge.Common.Tests.DTOs
{
    public class KongPluginTests
    {
        [Fact]
        public void Equals_WithSameValues_IsTrue()
        {
            var value = Guid.NewGuid().ToString();
            var plugin = new KongPlugin
            {
                ConsumerId = Guid.NewGuid().ToString(),
                ServiceId = Guid.NewGuid().ToString(),
                RouteId = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString(),
                Config = new Dictionary<string, object>
                {
                    { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() },
                    { Guid.NewGuid().ToString(), new { property = value } }
                }
            };

            var otherPlugin = new KongPlugin
            {
                ConsumerId = plugin.ConsumerId,
                ServiceId = plugin.ServiceId,
                RouteId = plugin.RouteId,
                Name = plugin.Name,
                Config = new Dictionary<string, object>
                {
                    { plugin.Config.Keys.ElementAt(0), plugin.Config[plugin.Config.Keys.ElementAt(0)] },
                    { plugin.Config.Keys.ElementAt(1), new { property = value } }
                }
            };

            plugin.Equals(otherPlugin).Should().BeTrue();
        }
    }
}
