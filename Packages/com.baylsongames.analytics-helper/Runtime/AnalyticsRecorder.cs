using System;
using UnityEngine;

namespace BaylsonGames.Analytics
{
    public static class AnalyticsRecorder
    {
        private static readonly IAnalyticsEventSink DefaultSink = new UnityAnalyticsEventSink();
        private static IAnalyticsEventSink s_Sink = DefaultSink;

        public static IAnalyticsEventSink Sink
        {
            get { return s_Sink; }
            set { s_Sink = value ?? DefaultSink; }
        }

        public static bool ThrowOnRecordFailure { get; set; }

        public static bool Record(AnalyticsEventDefinition definition, params AnalyticsParameterValueOverride[] overrides)
        {
            if (definition == null)
            {
                Debug.LogWarning("Analytics event was not recorded because the definition is missing.");
                return false;
            }

            try
            {
                AnalyticsEventPayload payload = definition.CreatePayload(overrides);
                Sink.Record(payload);
                return true;
            }
            catch (Exception exception)
            {
                if (ThrowOnRecordFailure)
                {
                    throw;
                }

                Debug.LogWarningFormat(
                    "Analytics event '{0}' was not recorded. {1}",
                    definition.EventName,
                    exception.Message);
                return false;
            }
        }

        public static void Flush()
        {
            Sink.Flush();
        }

        public static void ResetSink()
        {
            s_Sink = DefaultSink;
            ThrowOnRecordFailure = false;
        }
    }
}
