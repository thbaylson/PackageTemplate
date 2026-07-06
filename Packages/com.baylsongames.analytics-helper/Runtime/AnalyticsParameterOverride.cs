using System;
using UnityEngine;

namespace BaylsonGames.Analytics
{
    [Serializable]
    public sealed class AnalyticsParameterOverride
    {
        [SerializeField] private string m_Name = string.Empty;
        [SerializeField] private AnalyticsParameterType m_Type;
        [SerializeField] private AnalyticsParameterValue m_Value = new AnalyticsParameterValue();

        public AnalyticsParameterOverride()
        {
        }

        public AnalyticsParameterOverride(string name, AnalyticsParameterType type, AnalyticsParameterValue value)
        {
            m_Name = name;
            m_Type = type;
            m_Value = value ?? new AnalyticsParameterValue();
        }

        public string Name
        {
            get { return m_Name; }
        }

        public AnalyticsParameterType Type
        {
            get { return m_Type; }
        }

        public AnalyticsParameterValue Value
        {
            get { return m_Value; }
        }

        public bool TryCreateValueOverride(out AnalyticsParameterValueOverride valueOverride, out string error)
        {
            if (!m_Value.TryGetValue(m_Type, out object value, out error))
            {
                valueOverride = default;
                return false;
            }

            valueOverride = new AnalyticsParameterValueOverride(m_Name, value);
            return true;
        }
    }
}
