using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BaylsonGames.Analytics.Editor
{
    [CustomEditor(typeof(AnalyticsEventDefinition))]
    public sealed class AnalyticsEventDefinitionEditor : UnityEditor.Editor
    {
        private SerializedProperty m_EventName;
        private SerializedProperty m_Description;
        private SerializedProperty m_Parameters;

        private void OnEnable()
        {
            m_EventName = serializedObject.FindProperty("m_EventName");
            m_Description = serializedObject.FindProperty("m_Description");
            m_Parameters = serializedObject.FindProperty("m_Parameters");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_EventName);
            EditorGUILayout.PropertyField(m_Description);
            EditorGUILayout.PropertyField(m_Parameters, true);

            serializedObject.ApplyModifiedProperties();

            DrawValidationMessages();
            DrawActions();
        }

        private void DrawValidationMessages()
        {
            AnalyticsEventDefinition definition = (AnalyticsEventDefinition)target;
            List<AnalyticsValidationMessage> messages = AnalyticsEventDefinitionValidator.Validate(definition);
            if (messages.Count == 0)
            {
                EditorGUILayout.HelpBox("Definition passes local validation.", MessageType.Info);
                return;
            }

            foreach (AnalyticsValidationMessage message in messages)
            {
                EditorGUILayout.HelpBox(message.ToString(), ToMessageType(message.Severity));
            }
        }

        private void DrawActions()
        {
            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Copy Dashboard Summary"))
                {
                    AnalyticsEventDefinition definition = (AnalyticsEventDefinition)target;
                    EditorGUIUtility.systemCopyBuffer =
                        AnalyticsSchemaMarkdownExporter.BuildMarkdown(new[] { definition });
                }

                if (GUILayout.Button("Export All Schemas"))
                {
                    AnalyticsSchemaMarkdownExporter.ExportAllDefinitions();
                }
            }
        }

        private static MessageType ToMessageType(AnalyticsValidationSeverity severity)
        {
            switch (severity)
            {
                case AnalyticsValidationSeverity.Error:
                    return MessageType.Error;
                case AnalyticsValidationSeverity.Warning:
                    return MessageType.Warning;
                default:
                    return MessageType.Info;
            }
        }
    }
}
