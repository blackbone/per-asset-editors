using System;
using UnityEditor;
using UnityEngine;

namespace B.PerAssetEditors
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
        
        public OverrideEditor GetOverrideEditor(UnityEngine.Object target)
        {
            // per asset is 1st priority
            if (assetConfigurations != null)
            {
                foreach (var assetConfiguration in assetConfigurations)
                {
                    if (assetConfiguration.Asset == null)
                        continue;
                        
                    if (assetConfiguration.Asset != target)
                        continue;
                    
                    return Activator.CreateInstance(assetConfiguration.EditorType) as OverrideEditor;
                }
            }
            
            // per pattern is 2nd priority
            
            // per type is 3rd priority
            if (assetTypeConfigurations != null)
            {
                foreach (var assetTypeConfiguration in assetTypeConfigurations)
                {
                    var assetType = assetTypeConfiguration.AssetType;
                    if (assetType == null)
                        continue;

                    if (!assetType.IsInstanceOfType(target))
                        continue;
                    
                    return Activator.CreateInstance(assetTypeConfiguration.EditorType) as OverrideEditor;
                }
            }

            return null;
        }
    }
}