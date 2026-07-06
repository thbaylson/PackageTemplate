using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace BaylsonGames.Analytics.Tests
{
    public sealed class AnalyticsEventDefinitionTests
    {
        [TearDown]
        public void TearDown()
        {
            AnalyticsRecorder.ResetSink();
        }

        [Test]
        public void CreatePayload_UsesDefaultsAndOverrides()
        {
            AnalyticsEventDefinition definition = AnalyticsEventDefinition.CreateRuntimeDefinition(
                "level_started",
                new[]
                {
                    new AnalyticsParameterDefinition(
                        "level_id",
                        AnalyticsParameterType.String,
                        required: true,
                        useDefaultValue: false),
                    new AnalyticsParameterDefinition(
                        "attempt",
                        AnalyticsParameterType.Int,
                        defaultValue: AnalyticsParameterValue.Int(1))
                });

            AnalyticsEventPayload payload = definition.CreatePayload(
                AnalyticsParameterValueOverride.String("level_id", "forest_01"),
                AnalyticsParameterValueOverride.Int("attempt", 2));

            Assert.AreEqual("level_started", payload.EventName);
            Assert.AreEqual("forest_01", payload.Parameters["level_id"]);
            Assert.AreEqual(2, payload.Parameters["attempt"]);
        }

        [Test]
        public void CreatePayload_ThrowsWhenRequiredParameterIsMissing()
        {
            AnalyticsEventDefinition definition = AnalyticsEventDefinition.CreateRuntimeDefinition(
                "level_started",
                new[]
                {
                    new AnalyticsParameterDefinition(
                        "level_id",
                        AnalyticsParameterType.String,
                        required: true,
                        useDefaultValue: false)
                });

            Assert.Throws<InvalidOperationException>(() => definition.CreatePayload());
        }

        [Test]
        public void CreatePayload_ThrowsWhenOverrideIsUnknown()
        {
            AnalyticsEventDefinition definition = AnalyticsEventDefinition.CreateRuntimeDefinition(
                "level_started",
                new[]
                {
                    new AnalyticsParameterDefinition("level_id", AnalyticsParameterType.String)
                });

            Assert.Throws<InvalidOperationException>(
                () => definition.CreatePayload(AnalyticsParameterValueOverride.String("world_id", "forest")));
        }

        [Test]
        public void Recorder_UsesInjectedSink()
        {
            AnalyticsEventDefinition definition = AnalyticsEventDefinition.CreateRuntimeDefinition(
                "level_started",
                new[]
                {
                    new AnalyticsParameterDefinition("level_id", AnalyticsParameterType.String)
                });

            CapturingSink sink = new CapturingSink();
            AnalyticsRecorder.Sink = sink;

            bool recorded = AnalyticsRecorder.Record(
                definition,
                AnalyticsParameterValueOverride.String("level_id", "forest_01"));

            Assert.IsTrue(recorded);
            Assert.NotNull(sink.LastPayload);
            Assert.AreEqual("level_started", sink.LastPayload.EventName);
            Assert.AreEqual("forest_01", sink.LastPayload.Parameters["level_id"]);
        }

        private sealed class CapturingSink : IAnalyticsEventSink
        {
            public AnalyticsEventPayload LastPayload { get; private set; }

            public int FlushCount { get; private set; }

            public void Record(AnalyticsEventPayload payload)
            {
                LastPayload = payload;
            }

            public void Flush()
            {
                FlushCount++;
            }
        }
    }
}
