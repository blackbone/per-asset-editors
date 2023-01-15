using UnityEditor;
using UnityEngine;

namespace B.PerAssetEditors.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(AssetTypeConfiguration))]
    public sealed class AssetTypeConfigurationPropertyDrawer : ConfigurationPropertyDrawer
    {
        protected override float GetContentHeight() => EditorGUIUtility.singleLineHeight;

        protected override void OnGUI(Rect position, SerializedProperty property)
            => DrawTypeSelector<Object>(position, property, "assetTypeData", "Asset Type");
    }
}