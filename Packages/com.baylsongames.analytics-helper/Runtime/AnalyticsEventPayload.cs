using System;
using System.Collections.Generic;

namespace BaylsonGames.Analytics
{
    public sealed class AnalyticsEventPayload
    {
        private readonly Dictionary<string, object> m_Parameters;

        public AnalyticsEventPayload(string eventName, IDictionary<string, object> parameters)
        {
            if (string.IsNullOrWhiteSpace(eventName))
            {
                throw new ArgumentException("Event name cannot be blank.", nameof(eventName));
            }

            EventName = eventName;
            m_Parameters = parameters != null
                ? new Dictionary<string, object>(parameters, StringComparer.Ordinal)
                : new Dictionary<string, object>(StringComparer.Ordinal);
        }

        public string EventName { get; }

        public IReadOnlyDictionary<string, object> Parameters
        {
            get { return m_Parameters; }
        }

        public bool HasParameters
        {
            get { return m_Parameters.Count > 0; }
        }
    }
}
