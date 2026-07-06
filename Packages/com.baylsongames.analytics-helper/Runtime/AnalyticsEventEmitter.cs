using System.Collections.Generic;
using UnityEngine;

namespace BaylsonGames.Analytics
{
    [AddComponentMenu("Baylson Games/Analytics/Analytics Event Emitter")]
    public sealed class AnalyticsEventEmitter : MonoBehaviour
    {
        [SerializeField] private AnalyticsEventDefinition m_EventDefinition;
        [SerializeField] private bool m_RecordOnEnable;
        [SerializeField] private bool m_RecordOnStart;
        [SerializeField] private bool m_FlushAfterRecord;
        [SerializeField] private List<AnalyticsParameterOverride> m_ParameterOverrides =
            new List<AnalyticsParameterOverride>();

        public AnalyticsEventDefinition EventDefinition
        {
            get { return m_EventDefinition; }
            set { m_EventDefinition = value; }
        }

        private void OnEnable()
        {
            if (m_RecordOnEnable)
            {
                Record();
            }
        }

        private void Start()
        {
            if (m_RecordOnStart)
            {
                Record();
            }
        }

        public void Record()
        {
            if (m_EventDefinition == null)
            {
                Debug.LogWarning("Analytics event emitter has no event definition.", this);
                return;
            }

            AnalyticsParameterValueOverride[] overrides = BuildOverrides();
            if (AnalyticsRecorder.Record(m_EventDefinition, overrides) && m_FlushAfterRecord)
            {
                AnalyticsRecorder.Flush();
            }
        }

        private AnalyticsParameterValueOverride[] BuildOverrides()
        {
            List<AnalyticsParameterValueOverride> overrides = new List<AnalyticsParameterValueOverride>();

            foreach (AnalyticsParameterOverride parameterOverride in m_ParameterOverrides)
            {
                if (parameterOverride == null)
                {
                    continue;
                }

                if (parameterOverride.TryCreateValueOverride(out AnalyticsParameterValueOverride valueOverride, out string error))
                {
                    overrides.Add(valueOverride);
                    continue;
                }

                Debug.LogWarningFormat(
                    this,
                    "Analytics parameter override '{0}' was skipped. {1}",
                    parameterOverride.Name,
                    error);
            }

            return overrides.ToArray();
        }
    }
}
