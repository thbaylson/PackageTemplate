using NUnit.Framework;

namespace BaylsonGames.Analytics.Editor.Tests
{
    public sealed class AnalyticsSchemaMarkdownExporterTests
    {
        [Test]
        public void BuildMarkdown_IncludesEventAndParameters()
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

            string markdown = AnalyticsSchemaMarkdownExporter.BuildMarkdown(new[] { definition });

            StringAssert.Contains("## level_started", markdown);
            StringAssert.Contains("| level_id | String | Yes |", markdown);
            StringAssert.Contains("| attempt | Int | No | 1 |", markdown);
        }
    }
}
