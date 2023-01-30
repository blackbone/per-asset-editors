using System;
using UnityEngine;

namespace OverrideEditors.Editor.Configurations
{
    [Serializable]
    internal sealed class AssetLinkConfiguration : Configuration
    {
        [SerializeField] private UnityEngine.Object asset;

        public UnityEngine.Object Asset => asset;
    }
}