using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace BaylsonGames.Analytics.Editor
{
    public static class AnalyticsSchemaMarkdownExporter
    {
        [MenuItem("Tools/Baylson Games/Analytics/Validate Event Definitions")]
        public static void ValidateAllDefinitions()
        {
            AnalyticsEventDefinition[] definitions = LoadAllDefinitions();
            int errorCount = 0;
            int warningCount = 0;

            foreach (AnalyticsEventDefinition definition in definitions)
            {
                List<AnalyticsValidationMessage> messages = AnalyticsEventDefinitionValidator.Validate(definition);
                foreach (AnalyticsValidationMessage message in messages)
                {
                    if (message.Severity == AnalyticsValidationSeverity.Error)
                    {
                        errorCount++;
                    }
                    else if (message.Severity == AnalyticsValidationSeverity.Warning)
                    {
                        warningCount++;
                    }

                    Debug.Log(message.ToString(), definition);
                }
            }

            Debug.LogFormat(
                "Analytics validation complete. Definitions: {0}, Errors: {1}, Warnings: {2}",
                definitions.Length,
                errorCount,
                warningCount);
        }

        [MenuItem("Tools/Baylson Games/Analytics/Export Event Schema Markdown")]
        public static void ExportAllDefinitions()
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Export Analytics Event Schema",
                "AnalyticsEventSchema",
                "md",
                "Choose where to write the schema markdown file.");

            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            File.WriteAllText(path, BuildMarkdown(LoadAllDefinitions()));
            AssetDatabase.ImportAsset(path);
        }

        public static string BuildMarkdown(IEnumerable<AnalyticsEventDefinition> definitions)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("# Unity Analytics Event Schema");
            builder.AppendLine();
            builder.AppendLine("Mirror these event and parameter names in Unity Cloud Dashboard > Analytics > Event Manager.");
            builder.AppendLine();

            foreach (AnalyticsEventDefinition definition in definitions
                         .Where(definition => definition != null)
                         .OrderBy(definition => definition.EventName))
            {
                AppendDefinition(builder, definition);
            }

            return builder.ToString();
        }

        private static void AppendDefinition(StringBuilder builder, AnalyticsEventDefinition definition)
        {
            builder.Append("## ");
            builder.AppendLine(string.IsNullOrWhiteSpace(definition.EventName) ? "(Unnamed event)" : definition.EventName);
            builder.AppendLine();

            if (!string.IsNullOrWhiteSpace(definition.Description))
            {
                builder.AppendLine(definition.Description);
                builder.AppendLine();
            }

            builder.AppendLine("| Parameter | Type | Required | Default | Description |");
            builder.AppendLine("| --- | --- | --- | --- | --- |");

            foreach (AnalyticsParameterDefinition parameter in definition.Parameters)
            {
                if (parameter == null)
                {
                    continue;
                }

                builder.Append("| ");
                builder.Append(EscapeTable(parameter.Name));
                builder.Append(" | ");
                builder.Append(parameter.Type);
                builder.Append(" | ");
                builder.Append(parameter.Required ? "Yes" : "No");
                builder.Append(" | ");
                builder.Append(EscapeTable(GetDefaultValueText(parameter)));
                builder.Append(" | ");
                builder.Append(EscapeTable(parameter.Description));
                builder.AppendLine(" |");
            }

            builder.AppendLine();
        }

        private static string GetDefaultValueText(AnalyticsParameterDefinition parameter)
        {
            if (!parameter.UseDefaultValue)
            {
                return string.Empty;
            }

            if (parameter.DefaultValue == null)
            {
                return "(missing)";
            }

            return parameter.DefaultValue.ToDisplayString(parameter.Type);
        }

        private static string EscapeTable(string value)
        {
            return string.IsNullOrEmpty(value) ? string.Empty : value.Replace("|", "\\|");
        }

        private static AnalyticsEventDefinition[] LoadAllDefinitions()
        {
            return AssetDatabase.FindAssets("t:AnalyticsEventDefinition")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<AnalyticsEventDefinition>)
                .Where(definition => definition != null)
                .ToArray();
        }
    }
}
