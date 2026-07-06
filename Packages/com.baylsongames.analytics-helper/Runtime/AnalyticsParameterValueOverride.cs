using System;

namespace BaylsonGames.Analytics
{
    public readonly struct AnalyticsParameterValueOverride
    {
        public AnalyticsParameterValueOverride(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }

        public object Value { get; }

        public static AnalyticsParameterValueOverride String(string name, string value)
        {
            return new AnalyticsParameterValueOverride(name, value);
        }

        public static AnalyticsParameterValueOverride Int(string name, int value)
        {
            return new AnalyticsParameterValueOverride(name, value);
        }

        public static AnalyticsParameterValueOverride Long(string name, long value)
        {
            return new AnalyticsParameterValueOverride(name, value);
        }

        public static AnalyticsParameterValueOverride Float(string name, float value)
        {
            return new AnalyticsParameterValueOverride(name, value);
        }

        public static AnalyticsParameterValueOverride Double(string name, double value)
        {
            return new AnalyticsParameterValueOverride(name, value);
        }

        public static AnalyticsParameterValueOverride Bool(string name, bool value)
        {
            return new AnalyticsParameterValueOverride(name, value);
        }

        public static AnalyticsParameterValueOverride Timestamp(string name, DateTime value)
        {
            return new AnalyticsParameterValueOverride(name, value);
        }

        public bool TryGetTypedValue(AnalyticsParameterType expectedType, out object value, out string error)
        {
            if (Value == null)
            {
                value = null;
                error = "Parameter override values cannot be null.";
                return false;
            }

            switch (expectedType)
            {
                case AnalyticsParameterType.String:
                    return TryGetString(out value, out error);
                case AnalyticsParameterType.Int:
                    return TryGetInt(out value, out error);
                case AnalyticsParameterType.Long:
                    return TryGetLong(out value, out error);
                case AnalyticsParameterType.Float:
                    return TryGetFloat(out value, out error);
                case AnalyticsParameterType.Double:
                    return TryGetDouble(out value, out error);
                case AnalyticsParameterType.Bool:
                    return TryGetBool(out value, out error);
                case AnalyticsParameterType.Timestamp:
                    return TryGetTimestamp(out value, out error);
                default:
                    value = null;
                    error = "Unsupported analytics parameter type.";
                    return false;
            }
        }

        private bool TryGetString(out object value, out string error)
        {
            if (Value is string stringValue)
            {
                value = stringValue;
                error = null;
                return true;
            }

            return TypeMismatch("string", out value, out error);
        }

        private bool TryGetInt(out object value, out string error)
        {
            if (Value is int intValue)
            {
                value = intValue;
                error = null;
                return true;
            }

            return TypeMismatch("int", out value, out error);
        }

        private bool TryGetLong(out object value, out string error)
        {
            if (Value is long longValue)
            {
                value = longValue;
                error = null;
                return true;
            }

            if (Value is int intValue)
            {
                value = (long)intValue;
                error = null;
                return true;
            }

            return TypeMismatch("long", out value, out error);
        }

        private bool TryGetFloat(out object value, out string error)
        {
            if (Value is float floatValue)
            {
                value = floatValue;
                error = null;
                return true;
            }

            if (Value is int intValue)
            {
                value = (float)intValue;
                error = null;
                return true;
            }

            if (Value is long longValue)
            {
                value = (float)longValue;
                error = null;
                return true;
            }

            return TypeMismatch("float", out value, out error);
        }

        private bool TryGetDouble(out object value, out string error)
        {
            if (Value is double doubleValue)
            {
                value = doubleValue;
                error = null;
                return true;
            }

            if (Value is float floatValue)
            {
                value = (double)floatValue;
                error = null;
                return true;
            }

            if (Value is int intValue)
            {
                value = (double)intValue;
                error = null;
                return true;
            }

            if (Value is long longValue)
            {
                value = (double)longValue;
                error = null;
                return true;
            }

            return TypeMismatch("double", out value, out error);
        }

        private bool TryGetBool(out object value, out string error)
        {
            if (Value is bool boolValue)
            {
                value = boolValue;
                error = null;
                return true;
            }

            return TypeMismatch("bool", out value, out error);
        }

        private bool TryGetTimestamp(out object value, out string error)
        {
            if (Value is DateTime timestamp)
            {
                value = timestamp;
                error = null;
                return true;
            }

            return TypeMismatch("DateTime", out value, out error);
        }

        private bool TypeMismatch(string expectedTypeName, out object value, out string error)
        {
            value = null;
            error = string.Format(
                "Parameter '{0}' expected {1}, but received {2}.",
                Name,
                expectedTypeName,
                Value.GetType().Name);
            return false;
        }
    }
}
