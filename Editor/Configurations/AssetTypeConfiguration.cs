using System;
using UnityEngine;

namespace B.PerAssetEditors
{
    [Serializable]
    internal sealed class AssetTypeConfiguration : Configuration
    {
        [SerializeField] private string assetTypeData;

        [NonSerialized] private Type assetType;

        public Type AssetType => assetType;

        public override void OnBeforeSerialize()
        {
            base.OnBeforeSerialize();
            TypeSerializer.TryGetData(assetType, out assetTypeData);
        }

        public override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();
            TypeSerializer.TryGetType(assetTypeData, out assetType);
        }
    }
}