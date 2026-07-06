using System;
using UnityEngine;

namespace BaylsonGames.Analytics
{
    [Serializable]
    public sealed class AnalyticsParameterDefinition
    {
        [SerializeField] private string m_Name = string.Empty;
        [SerializeField] private string m_Description = string.Empty;
        [SerializeField] private AnalyticsParameterType m_Type;
        [SerializeField] private bool m_Required;
        [SerializeField] private bool m_UseDefaultValue = true;
        [SerializeField] private AnalyticsParameterValue m_DefaultValue = new AnalyticsParameterValue();

        public AnalyticsParameterDefinition()
        {
        }

        public AnalyticsParameterDefinition(
            string name,
            AnalyticsParameterType type,
            bool required = false,
            bool useDefaultValue = true,
            AnalyticsParameterValue defaultValue = null)
        {
            m_Name = name;
            m_Type = type;
            m_Required = required;
            m_UseDefaultValue = useDefaultValue;
            m_DefaultValue = defaultValue ?? new AnalyticsParameterValue();
        }

        public string Name
        {
            get { return m_Name; }
        }

        public string Description
        {
            get { return m_Description; }
        }

        public AnalyticsParameterType Type
        {
            get { return m_Type; }
        }

        public bool Required
        {
            get { return m_Required; }
        }

        public bool UseDefaultValue
        {
            get { return m_UseDefaultValue; }
        }

        public AnalyticsParameterValue DefaultValue
        {
            get { return m_DefaultValue; }
        }
    }
}
