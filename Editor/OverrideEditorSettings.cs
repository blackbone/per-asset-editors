using System;
using OverrideEditors.Editor.Configurations;
using OverrideEditors.Editor.Editors;
using UnityEditor;
using UnityEngine;

namespace OverrideEditors.Editor
{
    [FilePath("ProjectSettings/" + nameof(OverrideEditorSettings) + ".asset", FilePathAttribute.Location.ProjectFolder)]
    internal sealed class OverrideEditorSettings : ScriptableSingleton<OverrideEditorSettings>
    {
        internal static class Names
        {
            public static string PerAsset => nameof(assetConfigurations);
            public static string PerAssetType => nameof(assetTypeConfigurations);
        }
        
        [SerializeField] private AssetConfiguration[] assetConfigurations;
        [SerializeField] private AssetTypeConfiguration[] assetTypeConfigurations;

        internal void Save() => Save(true);
        
        public OverrideEditor GetOverrideEditor(UnityEngine.Object target)
        {
            // per asset is 1st priority
            if (TryGetPerAssetOverrideEditor(target, out var editor))
                return editor;

            // per pattern is 2nd priority
            // TODO [Dmitrii Osipov]
            
            // per type is 3rd priority
            return TryGetPerAssetTypeOverrideEditor(target, out editor) ? editor : null;
        }

        private bool TryGetPerAssetOverrideEditor(UnityEngine.Object target, out OverrideEditor editor)
        {
            editor = null;
            if (assetConfigurations == null)
                return false;

            foreach (var assetConfiguration in assetConfigurations)
            {
                if (assetConfiguration.EditorType == null)
                    continue;

                if (assetConfiguration.Asset != target)
                    continue;

                editor =  Activator.CreateInstance(assetConfiguration.EditorType) as OverrideEditor;
                return true;
            }
            
            return false;
        }
        
        private bool TryGetPerAssetTypeOverrideEditor(UnityEngine.Object target, out OverrideEditor editor)
        {
            editor = null;
            if (assetConfigurations == null)
                return false;

            foreach (var assetTypeConfiguration in assetTypeConfigurations)
            {
                if (assetTypeConfiguration.EditorType == null)
                    continue;

                if (!(assetTypeConfiguration.AssetType?.IsInstanceOfType(target) ?? false))
                    continue;
                
                editor =  Activator.CreateInstance(assetTypeConfiguration.EditorType) as OverrideEditor;
                return true;
            }
            
            return false;
        }
    }
}