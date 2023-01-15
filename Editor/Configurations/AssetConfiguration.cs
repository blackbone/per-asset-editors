using System;
using UnityEngine;

namespace B.PerAssetEditors
{
    [Serializable]
    internal sealed class AssetConfiguration : Configuration
    {
        [SerializeField] private UnityEngine.Object asset;

        public UnityEngine.Object Asset => asset;
    }
}