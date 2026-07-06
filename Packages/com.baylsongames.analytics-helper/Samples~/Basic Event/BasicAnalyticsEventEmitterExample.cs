using BaylsonGames.Analytics;
using UnityEngine;

public sealed class BasicAnalyticsEventEmitterExample : MonoBehaviour
{
    [SerializeField] private AnalyticsEventDefinition m_LevelStarted;
    [SerializeField] private string m_LevelId = "example_level";

    public void RecordLevelStarted()
    {
        AnalyticsRecorder.Record(
            m_LevelStarted,
            AnalyticsParameterValueOverride.String("level_id", m_LevelId));
    }
}
