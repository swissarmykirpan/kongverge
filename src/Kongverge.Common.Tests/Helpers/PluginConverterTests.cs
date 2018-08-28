using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Kongverge.Common.DTOs;
using Kongverge.Common.Helpers;
using Kongverge.KongPlugin;
using Newtonsoft.Json;
using Xunit;

namespace Kongverge.Common.Tests.Helpers
{
    public class PluginConverterTests
    {
        private class ExtendibleTestObject : ExtendibleKongObject
        {
            protected override PluginBody DoDecoratePluginBody(PluginBody body)
            {
                return body;
            }
        }

        private readonly Fixture _fixture = new Fixture();

        private readonly PluginConverter _converter =
            new PluginConverter(
                new IKongPlugin[] {
                    new TestParsingPlugin("test"),
                    new OtherTestParsingPlugin("other"),
                    new NestedTestParsingPlugin("nested")
                });

        [Fact]
        public void CantSerializerAnyOldThing()
        {
            _converter.CanConvert(typeof(string)).Should().BeFalse("Can't convert any old thing");
        }

        [Fact]
        public void CanSerializeExtendibleThing()
        {
            _converter.CanConvert(typeof(ExtendibleTestObject)).Should().BeTrue("Can convert an extendible object");
        }

        [Fact]
        public void CanReadOrWrite()
        {
            _converter.CanRead.Should().BeTrue("Can read json");
            _converter.CanWrite.Should().BeTrue("Can write json");
        }

        [Fact]
        public void CanWriteExtendibleThing_WithNoExtensions()
        {
            var result = JsonConvert.DeserializeObject<ExtendibleTestObject>("{}", _converter);

            result.Should().NotBeNull("Should deserialize something");
        }

        [Fact]
        public void CanWriteExtendibleThing_WithAnExtension()
        {
            var result = JsonConvert.DeserializeObject<ExtendibleTestObject>("{\"test\":{}}", _converter);

            result.Should().NotBeNull("Should deserialize something");
        }

        [Fact]
        public void CanRoundTrip_AnExtension()
        {
            var testKongConfig = _fixture.Create<TestKongConfig>();

            var start =
                new ExtendibleTestObject
                {
                    Plugins = new List<IKongPluginConfig>
                    {
                        testKongConfig
                    }
                };

            var json = JsonConvert.SerializeObject(start, _converter);
            var result = JsonConvert.DeserializeObject<ExtendibleTestObject>(json, _converter);

            result.Should().NotBeNull("Should deserialize something");

            result.Plugins.Should().NotBeEmpty();
            result.Plugins.Cast<TestKongConfig>().First().Should().BeEquivalentTo(testKongConfig);
        }

        [Fact]
        public void CanRoundTrip_TwoExtensions()
        {
            var testKongConfig = _fixture.Create<TestKongConfig>();

            var testKongConfig2 = _fixture.Create<OtherTestKongConfig>();

            var start =
                new ExtendibleTestObject
                {
                    Plugins = new List<IKongPluginConfig>
                    {
                        testKongConfig,
                        testKongConfig2
                    }
                };

            var json = JsonConvert.SerializeObject(start, _converter);
            var result = JsonConvert.DeserializeObject<ExtendibleTestObject>(json, _converter);

            result.Should().NotBeNull("Should deserialize something");

            result.Plugins.Should().NotBeEmpty();
            result.Plugins.OfType<TestKongConfig>().First().Should().BeEquivalentTo(testKongConfig);
            result.Plugins.OfType<OtherTestKongConfig>().First().Should().BeEquivalentTo(testKongConfig2);
        }

        [Fact]
        public void CanRoundTrip_NestedExtensions()
        {
            var testKongConfig = _fixture.Create<NestedTestKongConfig>();

            var start =
                new ExtendibleTestObject
                {
                    Plugins = new List<IKongPluginConfig>
                    {
                        testKongConfig,
                    }
                };

            var json = JsonConvert.SerializeObject(start, _converter);
            var result = JsonConvert.DeserializeObject<ExtendibleTestObject>(json, _converter);

            result.Should().NotBeNull("Should deserialize something");

            result.Plugins.Should().NotBeEmpty();
            result.Plugins.OfType<NestedTestKongConfig>().First().Should().BeEquivalentTo(testKongConfig);
        }
    }
}