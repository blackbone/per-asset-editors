using OverrideEditors.Editor.Configurations;
using UnityEditor;
using UnityEngine;

namespace OverrideEditors.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(AssetTypeConfiguration))]
    public sealed class AssetTypeConfigurationPropertyDrawer : ConfigurationPropertyDrawer
    {
        protected override float GetContentHeight()
        {
            return EditorGUIUtility.singleLineHeight;
        }

        protected override void OnGUI(Rect position, SerializedProperty property)
        {
            DrawTypeSelector<Object>(position, property, "assetTypeData", "Asset Type");
        }
    }
}