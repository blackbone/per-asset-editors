using OverrideEditors.Editor.Configurations;
using UnityEditor;
using UnityEngine;

namespace OverrideEditors.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(AssetLinkConfiguration))]
    public sealed class AssetConfigurationPropertyDrawer : ConfigurationPropertyDrawer
    {
        protected override float GetContentHeight() => EditorGUIUtility.singleLineHeight;
        
        protected override void OnGUI(Rect position, SerializedProperty property)
        {
            var assetProperty = property.FindPropertyRelative("asset");
            assetProperty.objectReferenceValue = EditorGUI.ObjectField(position, "Asset", assetProperty.objectReferenceValue, typeof(Object), false);
        }
    }
}