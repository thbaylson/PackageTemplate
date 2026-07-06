using System;
using System.Collections.Generic;
using UnityEngine;

namespace BaylsonGames.Analytics
{
    [CreateAssetMenu(
        fileName = "AnalyticsEventDefinition",
        menuName = "Baylson Games/Analytics/Event Definition")]
    public sealed class AnalyticsEventDefinition : ScriptableObject
    {
        [SerializeField] private string m_EventName = string.Empty;
        [SerializeField] private string m_Description = string.Empty;
        [SerializeField] private List<AnalyticsParameterDefinition> m_Parameters =
            new List<AnalyticsParameterDefinition>();

        public static AnalyticsEventDefinition CreateRuntimeDefinition(
            string eventName,
            IEnumerable<AnalyticsParameterDefinition> parameters = null)
        {
            AnalyticsEventDefinition definition = CreateInstance<AnalyticsEventDefinition>();
            definition.m_EventName = eventName;
            definition.m_Parameters = parameters != null
                ? new List<AnalyticsParameterDefinition>(parameters)
                : new List<AnalyticsParameterDefinition>();
            return definition;
        }

        public string EventName
        {
            get { return m_EventName; }
        }

        public string Description
        {
            get { return m_Description; }
        }

        public IReadOnlyList<AnalyticsParameterDefinition> Parameters
        {
            get { return m_Parameters ?? (IReadOnlyList<AnalyticsParameterDefinition>)System.Array.Empty<AnalyticsParameterDefinition>(); }
        }

        public bool Record(params AnalyticsParameterValueOverride[] overrides)
        {
            return AnalyticsRecorder.Record(this, overrides);
        }

        public AnalyticsEventPayload CreatePayload(params AnalyticsParameterValueOverride[] overrides)
        {
            return CreatePayload((IEnumerable<AnalyticsParameterValueOverride>)overrides);
        }

        public AnalyticsEventPayload CreatePayload(IEnumerable<AnalyticsParameterValueOverride> overrides)
        {
            ValidateEventName();

            Dictionary<string, AnalyticsParameterValueOverride> overridesByName = BuildOverrideMap(overrides);
            Dictionary<string, object> values = new Dictionary<string, object>(StringComparer.Ordinal);
            HashSet<string> knownParameters = new HashSet<string>(StringComparer.Ordinal);

            foreach (AnalyticsParameterDefinition parameter in Parameters)
            {
                ValidateParameter(parameter, knownParameters);

                if (overridesByName.TryGetValue(parameter.Name, out AnalyticsParameterValueOverride valueOverride))
                {
                    values[parameter.Name] = ConvertOverride(parameter, valueOverride);
                    continue;
                }

                if (parameter.UseDefaultValue)
                {
                    values[parameter.Name] = ConvertDefaultValue(parameter);
                    continue;
                }

                if (parameter.Required)
                {
                    throw new InvalidOperationException(
                        string.Format("Required analytics parameter '{0}' has no default value or override.", parameter.Name));
                }
            }

            foreach (string overrideName in overridesByName.Keys)
            {
                if (!knownParameters.Contains(overrideName))
                {
                    throw new InvalidOperationException(
                        string.Format("Analytics parameter override '{0}' is not defined on event '{1}'.", overrideName, m_EventName));
                }
            }

            return new AnalyticsEventPayload(m_EventName, values);
        }

        private void ValidateEventName()
        {
            if (!AnalyticsEventDefinitionValidator.IsValidDashboardName(m_EventName))
            {
                throw new InvalidOperationException(
                    string.Format(
                        "Analytics event name '{0}' is invalid. Names must start with a letter and contain only letters, numbers, and underscores.",
                        m_EventName));
            }
        }

        private static Dictionary<string, AnalyticsParameterValueOverride> BuildOverrideMap(
            IEnumerable<AnalyticsParameterValueOverride> overrides)
        {
            Dictionary<string, AnalyticsParameterValueOverride> overridesByName =
                new Dictionary<string, AnalyticsParameterValueOverride>(StringComparer.Ordinal);

            if (overrides == null)
            {
                return overridesByName;
            }

            foreach (AnalyticsParameterValueOverride valueOverride in overrides)
            {
                if (!AnalyticsEventDefinitionValidator.IsValidDashboardName(valueOverride.Name))
                {
                    throw new InvalidOperationException(
                        string.Format("Analytics parameter override name '{0}' is invalid.", valueOverride.Name));
                }

                if (overridesByName.ContainsKey(valueOverride.Name))
                {
                    throw new InvalidOperationException(
                        string.Format("Analytics parameter override '{0}' is duplicated.", valueOverride.Name));
                }

                overridesByName.Add(valueOverride.Name, valueOverride);
            }

            return overridesByName;
        }

        private static void ValidateParameter(
            AnalyticsParameterDefinition parameter,
            HashSet<string> knownParameters)
        {
            if (parameter == null)
            {
                throw new InvalidOperationException("Analytics event contains an empty parameter definition.");
            }

            if (!AnalyticsEventDefinitionValidator.IsValidDashboardName(parameter.Name))
            {
                throw new InvalidOperationException(
                    string.Format(
                        "Analytics parameter name '{0}' is invalid. Names must start with a letter and contain only letters, numbers, and underscores.",
                        parameter.Name));
            }

            if (AnalyticsReservedParameterNames.IsReserved(parameter.Name))
            {
                throw new InvalidOperationException(
                    string.Format("Analytics parameter name '{0}' is reserved by Unity Analytics.", parameter.Name));
            }

            if (!knownParameters.Add(parameter.Name))
            {
                throw new InvalidOperationException(
                    string.Format("Analytics parameter '{0}' is duplicated.", parameter.Name));
            }
        }

        private static object ConvertOverride(
            AnalyticsParameterDefinition parameter,
            AnalyticsParameterValueOverride valueOverride)
        {
            if (!valueOverride.TryGetTypedValue(parameter.Type, out object value, out string error))
            {
                throw new InvalidOperationException(error);
            }

            return value;
        }

        private static object ConvertDefaultValue(AnalyticsParameterDefinition parameter)
        {
            if (parameter.DefaultValue == null)
            {
                throw new InvalidOperationException(
                    string.Format("Analytics parameter '{0}' has no default value object.", parameter.Name));
            }

            if (!parameter.DefaultValue.TryGetValue(parameter.Type, out object value, out string error))
            {
                throw new InvalidOperationException(
                    string.Format("Analytics parameter '{0}' has an invalid default value. {1}", parameter.Name, error));
            }

            return value;
        }
    }
}
