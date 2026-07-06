using System;
using Unity.Services.Analytics;

namespace BaylsonGames.Analytics
{
    public sealed class UnityAnalyticsEventSink : IAnalyticsEventSink
    {
        public void Record(AnalyticsEventPayload payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            if (!payload.HasParameters)
            {
                AnalyticsService.Instance.RecordEvent(payload.EventName);
                return;
            }

            CustomEvent customEvent = new CustomEvent(payload.EventName);
            foreach (var parameter in payload.Parameters)
            {
                customEvent.Add(parameter.Key, parameter.Value);
            }

            AnalyticsService.Instance.RecordEvent(customEvent);
        }

        public void Flush()
        {
            AnalyticsService.Instance.Flush();
        }
    }
}
