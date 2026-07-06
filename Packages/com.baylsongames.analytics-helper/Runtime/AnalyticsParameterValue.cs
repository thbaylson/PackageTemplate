using System;
using System.Globalization;
using UnityEngine;

namespace BaylsonGames.Analytics
{
    [Serializable]
    public sealed class AnalyticsParameterValue
    {
        [SerializeField] private string m_StringValue = string.Empty;
        [SerializeField] private int m_IntValue;
        [SerializeField] private long m_LongValue;
        [SerializeField] private float m_FloatValue;
        [SerializeField] private double m_DoubleValue;
        [SerializeField] private bool m_BoolValue;
        [SerializeField] private bool m_UseCurrentUtcTime = true;
        [SerializeField] private string m_TimestampIso8601Utc = string.Empty;

        public static AnalyticsParameterValue String(string value)
        {
            return new AnalyticsParameterValue { m_StringValue = value };
        }

        public static AnalyticsParameterValue Int(int value)
        {
            return new AnalyticsParameterValue { m_IntValue = value };
        }

        public static AnalyticsParameterValue Long(long value)
        {
            return new AnalyticsParameterValue { m_LongValue = value };
        }

        public static AnalyticsParameterValue Float(float value)
        {
            return new AnalyticsParameterValue { m_FloatValue = value };
        }

        public static AnalyticsParameterValue Double(double value)
        {
            return new AnalyticsParameterValue { m_DoubleValue = value };
        }

        public static AnalyticsParameterValue Bool(bool value)
        {
            return new AnalyticsParameterValue { m_BoolValue = value };
        }

        public static AnalyticsParameterValue Timestamp(DateTime value)
        {
            return new AnalyticsParameterValue
            {
                m_UseCurrentUtcTime = false,
                m_TimestampIso8601Utc = value.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)
            };
        }

        public bool TryGetValue(AnalyticsParameterType type, out object value, out string error)
        {
            switch (type)
            {
                case AnalyticsParameterType.String:
                    value = m_StringValue ?? string.Empty;
                    error = null;
                    return true;
                case AnalyticsParameterType.Int:
                    value = m_IntValue;
                    error = null;
                    return true;
                case AnalyticsParameterType.Long:
                    value = m_LongValue;
                    error = null;
                    return true;
                case AnalyticsParameterType.Float:
                    value = m_FloatValue;
                    error = null;
                    return true;
                case AnalyticsParameterType.Double:
                    value = m_DoubleValue;
                    error = null;
                    return true;
                case AnalyticsParameterType.Bool:
                    value = m_BoolValue;
                    error = null;
                    return true;
                case AnalyticsParameterType.Timestamp:
                    return TryGetTimestamp(out value, out error);
                default:
                    value = null;
                    error = "Unsupported analytics parameter type.";
                    return false;
            }
        }

        public string ToDisplayString(AnalyticsParameterType type)
        {
            if (type == AnalyticsParameterType.Timestamp && m_UseCurrentUtcTime)
            {
                return "Current UTC time";
            }

            if (!TryGetValue(type, out object value, out string error))
            {
                return error;
            }

            if (value is DateTime timestamp)
            {
                return timestamp.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture);
            }

            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        private bool TryGetTimestamp(out object value, out string error)
        {
            if (m_UseCurrentUtcTime)
            {
                value = DateTime.UtcNow;
                error = null;
                return true;
            }

            if (DateTime.TryParse(
                    m_TimestampIso8601Utc,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                    out DateTime timestamp))
            {
                value = timestamp;
                error = null;
                return true;
            }

            value = null;
            error = "Timestamp values must be valid ISO 8601 date/time strings, or Use Current UTC Time must be enabled.";
            return false;
        }
    }
}
