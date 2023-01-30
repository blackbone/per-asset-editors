using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace OverrideEditors.Editor.Configurations
{
    [Serializable]
    internal sealed class AssetPatternConfiguration : Configuration
    {
        [SerializeField] private string regex;

        public Regex Regex => new(regex);
    }
}