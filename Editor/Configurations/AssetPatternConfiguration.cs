using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace OverrideEditors.Editor.Configurations
{
    [Serializable]
    internal sealed class AssetPatternConfiguration : Configuration
    {
        [SerializeField] private string regex;
        [NonSerialized] private Regex _regex;

        public Regex Regex => _regex ??= new(regex);
    }
}