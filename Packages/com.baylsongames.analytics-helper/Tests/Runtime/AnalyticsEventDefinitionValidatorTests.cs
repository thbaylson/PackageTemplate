using System.Linq;
using NUnit.Framework;

namespace BaylsonGames.Analytics.Tests
{
    public sealed class AnalyticsEventDefinitionValidatorTests
    {
        [TestCase("level_started", true)]
        [TestCase("LevelStarted01", true)]
        [TestCase("_level_started", false)]
        [TestCase("level-started", false)]
        [TestCase("1_level_started", false)]
        public void IsValidDashboardName_UsesUnityDashboardRules(string value, bool expected)
        {
            Assert.AreEqual(expected, AnalyticsEventDefinitionValidator.IsValidDashboardName(value));
        }

        [Test]
        public void Validate_FlagsReservedParameterNames()
        {
            AnalyticsEventDefinition definition = AnalyticsEventDefinition.CreateRuntimeDefinition(
                "level_started",
                new[]
                {
                    new AnalyticsParameterDefinition("timestamp", AnalyticsParameterType.String)
                });

            Assert.IsTrue(AnalyticsEventDefinitionValidator
                .Validate(definition)
                .Any(message => message.Severity == AnalyticsValidationSeverity.Error));
        }

        [Test]
        public void Validate_WarnsWhenParameterNamesDifferOnlyByCase()
        {
            AnalyticsEventDefinition definition = AnalyticsEventDefinition.CreateRuntimeDefinition(
                "level_started",
                new[]
                {
                    new AnalyticsParameterDefinition("level_id", AnalyticsParameterType.String),
                    new AnalyticsParameterDefinition("Level_id", AnalyticsParameterType.String)
                });

            Assert.IsTrue(AnalyticsEventDefinitionValidator
                .Validate(definition)
                .Any(message => message.Severity == AnalyticsValidationSeverity.Warning));
        }
    }
}
