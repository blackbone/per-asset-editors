using System;
using UnityEditor;
using UnityEngine;

namespace B.PerAssetEditors
{
    internal sealed class DefaultAssetCustomInspectorConfiguration : ScriptableObject
    {
        private const string AssetPath = "Assets/Editor/" + nameof(DefaultAssetCustomInspectorConfiguration) + ".asset";

        private static DefaultAssetCustomInspectorConfiguration instance;

        public static DefaultAssetCustomInspectorConfiguration Instance
        {
            get
            {
                if (instance != null) return instance;
                instance = AssetDatabase.LoadAssetAtPath<DefaultAssetCustomInspectorConfiguration>(AssetPath);
                if (instance != null) return instance;
                AssetDatabase.CreateAsset(CreateInstance<DefaultAssetCustomInspectorConfiguration>(), AssetPath);
                AssetDatabase.ImportAsset(AssetPath, ImportAssetOptions.ForceSynchronousImport);
                instance = AssetDatabase.LoadAssetAtPath<DefaultAssetCustomInspectorConfiguration>(AssetPath);
                return instance;
            }
        }

        [SerializeField] private AssetConfiguration[] assetConfigurations;
        
        public CustomAssetInspector GetInspector(UnityEngine.Object target)
        {
            if (assetConfigurations == null) return null;
            foreach (var assetConfiguration in assetConfigurations)
            {
                if (assetConfiguration.Asset == target)
                    return Activator.CreateInstance(assetConfiguration.Type) as CustomAssetInspector;
            }

            return null;
        }
    }
}