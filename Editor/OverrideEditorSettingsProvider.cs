using UnityEditor;
using UnityEngine.UIElements;

namespace OverrideEditors.Editor
{
    public sealed class OverrideEditorSettingsProvider : SettingsProvider
    {
        private SerializedObject settingsSerializedObject;

        private OverrideEditorSettingsProvider() : base($"ProjectSettings/{ObjectNames.NicifyVariableName(nameof(OverrideEditorSettings))}", SettingsScope.Project)
        {
        }

        [SettingsProvider]
        private static SettingsProvider Create() => new OverrideEditorSettingsProvider();

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            base.OnActivate(searchContext, rootElement);
            settingsSerializedObject = new SerializedObject(OverrideEditorSettings.instance);
        }

        public override void OnDeactivate()
        {
            base.OnDeactivate();
            settingsSerializedObject = null;
        }

        public override void OnGUI(string searchContext)
        {
            // generation options
            using var _ = new EditorGUILayout.VerticalScope("box");

            settingsSerializedObject.UpdateIfRequiredOrScript();

            EditorGUILayout.PropertyField(settingsSerializedObject.FindProperty(OverrideEditorSettings.Names.PerAsset));
            EditorGUILayout.PropertyField(settingsSerializedObject.FindProperty(OverrideEditorSettings.Names.PerAssetType));

            settingsSerializedObject.ApplyModifiedProperties();
        }
    }
}