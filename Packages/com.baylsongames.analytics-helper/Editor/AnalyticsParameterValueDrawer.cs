using UnityEditor;
using UnityEngine;

namespace BaylsonGames.Analytics.Editor
{
    [CustomPropertyDrawer(typeof(AnalyticsParameterValue))]
    public sealed class AnalyticsParameterValueDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            AnalyticsParameterType type = GetExpectedType(property);
            SerializedProperty valueProperty = GetValueProperty(property, type);

            if (type == AnalyticsParameterType.Timestamp)
            {
                DrawTimestamp(position, property, label);
            }
            else
            {
                EditorGUI.PropertyField(position, valueProperty, label);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (GetExpectedType(property) != AnalyticsParameterType.Timestamp)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            SerializedProperty useCurrentUtcTime = property.FindPropertyRelative("m_UseCurrentUtcTime");
            if (useCurrentUtcTime.boolValue)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            return EditorGUIUtility.singleLineHeight * 2f + EditorGUIUtility.standardVerticalSpacing;
        }

        private static void DrawTimestamp(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty useCurrentUtcTime = property.FindPropertyRelative("m_UseCurrentUtcTime");
            SerializedProperty timestamp = property.FindPropertyRelative("m_TimestampIso8601Utc");

            Rect currentTimeRect = new Rect(
                position.x,
                position.y,
                position.width,
                EditorGUIUtility.singleLineHeight);

            EditorGUI.PropertyField(currentTimeRect, useCurrentUtcTime, label);

            if (!useCurrentUtcTime.boolValue)
            {
                Rect timestampRect = new Rect(
                    position.x,
                    position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing,
                    position.width,
                    EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(timestampRect, timestamp, new GUIContent("Timestamp ISO 8601 UTC"));
            }
        }

        private static SerializedProperty GetValueProperty(SerializedProperty property, AnalyticsParameterType type)
        {
            switch (type)
            {
                case AnalyticsParameterType.Int:
                    return property.FindPropertyRelative("m_IntValue");
                case AnalyticsParameterType.Long:
                    return property.FindPropertyRelative("m_LongValue");
                case AnalyticsParameterType.Float:
                    return property.FindPropertyRelative("m_FloatValue");
                case AnalyticsParameterType.Double:
                    return property.FindPropertyRelative("m_DoubleValue");
                case AnalyticsParameterType.Bool:
                    return property.FindPropertyRelative("m_BoolValue");
                default:
                    return property.FindPropertyRelative("m_StringValue");
            }
        }

        private static AnalyticsParameterType GetExpectedType(SerializedProperty property)
        {
            string typePath = property.propertyPath
                .Replace(".m_DefaultValue", ".m_Type")
                .Replace(".m_Value", ".m_Type");

            SerializedProperty typeProperty = property.serializedObject.FindProperty(typePath);
            if (typeProperty == null)
            {
                return AnalyticsParameterType.String;
            }

            return (AnalyticsParameterType)typeProperty.enumValueIndex;
        }
    }
}
