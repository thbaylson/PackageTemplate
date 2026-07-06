using System.Collections.Generic;

namespace BaylsonGames.Analytics
{
    public static class AnalyticsEventDefinitionValidator
    {
        public static bool IsValidDashboardName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            if (!IsAsciiLetter(value[0]))
            {
                return false;
            }

            for (int i = 1; i < value.Length; i++)
            {
                char character = value[i];
                if (!IsAsciiLetter(character) && !IsAsciiDigit(character) && character != '_')
                {
                    return false;
                }
            }

            return true;
        }

        public static List<AnalyticsValidationMessage> Validate(AnalyticsEventDefinition definition)
        {
            List<AnalyticsValidationMessage> messages = new List<AnalyticsValidationMessage>();
            Validate(definition, messages);
            return messages;
        }

        public static void Validate(AnalyticsEventDefinition definition, IList<AnalyticsValidationMessage> messages)
        {
            if (definition == null)
            {
                messages.Add(new AnalyticsValidationMessage(
                    AnalyticsValidationSeverity.Error,
                    "Analytics event definition is missing."));
                return;
            }

            if (!IsValidDashboardName(definition.EventName))
            {
                messages.Add(new AnalyticsValidationMessage(
                    AnalyticsValidationSeverity.Error,
                    "Event names must start with a letter and contain only letters, numbers, and underscores.",
                    definition.EventName));
            }

            HashSet<string> exactNames = new HashSet<string>(System.StringComparer.Ordinal);
            HashSet<string> caseInsensitiveNames = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);

            foreach (AnalyticsParameterDefinition parameter in definition.Parameters)
            {
                ValidateParameter(parameter, messages, exactNames, caseInsensitiveNames);
            }
        }

        private static void ValidateParameter(
            AnalyticsParameterDefinition parameter,
            IList<AnalyticsValidationMessage> messages,
            HashSet<string> exactNames,
            HashSet<string> caseInsensitiveNames)
        {
            if (parameter == null)
            {
                messages.Add(new AnalyticsValidationMessage(
                    AnalyticsValidationSeverity.Error,
                    "Parameter definition is empty."));
                return;
            }

            if (!IsValidDashboardName(parameter.Name))
            {
                messages.Add(new AnalyticsValidationMessage(
                    AnalyticsValidationSeverity.Error,
                    "Parameter names must start with a letter and contain only letters, numbers, and underscores.",
                    parameter.Name));
                return;
            }

            if (!exactNames.Add(parameter.Name))
            {
                messages.Add(new AnalyticsValidationMessage(
                    AnalyticsValidationSeverity.Error,
                    "Parameter name is duplicated.",
                    parameter.Name));
            }

            if (!caseInsensitiveNames.Add(parameter.Name))
            {
                messages.Add(new AnalyticsValidationMessage(
                    AnalyticsValidationSeverity.Warning,
                    "Parameter names are case-sensitive, but names that differ only by case are hard to maintain.",
                    parameter.Name));
            }

            if (AnalyticsReservedParameterNames.IsReserved(parameter.Name))
            {
                messages.Add(new AnalyticsValidationMessage(
                    AnalyticsValidationSeverity.Error,
                    "Parameter name is reserved by Unity Analytics.",
                    parameter.Name));
            }

            if (parameter.UseDefaultValue && parameter.DefaultValue != null)
            {
                if (!parameter.DefaultValue.TryGetValue(parameter.Type, out _, out string error))
                {
                    messages.Add(new AnalyticsValidationMessage(
                        AnalyticsValidationSeverity.Error,
                        error,
                        parameter.Name));
                }
            }

            if (parameter.Required && !parameter.UseDefaultValue)
            {
                messages.Add(new AnalyticsValidationMessage(
                    AnalyticsValidationSeverity.Info,
                    "Required parameter must be supplied by an emitter override or code call.",
                    parameter.Name));
            }
        }

        private static bool IsAsciiLetter(char character)
        {
            return character >= 'a' && character <= 'z' || character >= 'A' && character <= 'Z';
        }

        private static bool IsAsciiDigit(char character)
        {
            return character >= '0' && character <= '9';
        }
    }
}
