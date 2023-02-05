using OverrideEditors.Editor.Configurations;
using UnityEditor;
using UnityEngine;

namespace OverrideEditors.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(AssetPatternConfiguration))]
    public sealed class AssetPatternConfigurationPropertyDrawer : ConfigurationPropertyDrawer
    {
        protected override float GetContentHeight()
        {
            return EditorGUIUtility.singleLineHeight;
        }

        protected override void OnGUI(Rect position, SerializedProperty property)
        {
            var regexProperty = property.FindPropertyRelative("regex");
            regexProperty.stringValue = EditorGUI.DelayedTextField(position, "Regex", regexProperty.stringValue);
        }
    }
}