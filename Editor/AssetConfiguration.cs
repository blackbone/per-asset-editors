using System;
using UnityEngine;

namespace B.PerAssetEditors
{
    [Serializable]
    internal sealed class AssetConfiguration : IConfiguration, ISerializationCallbackReceiver
    {
        [SerializeField] private UnityEngine.Object asset;
        [SerializeField] private string typeData;

        [NonSerialized] private Type type;

        public UnityEngine.Object Asset => asset;
        public Type Type => type;

        public void OnBeforeSerialize() => TypeSerializer.TryGetData(type, out typeData);

        public void OnAfterDeserialize() => TypeSerializer.TryGetType(typeData, out type);
    }
}