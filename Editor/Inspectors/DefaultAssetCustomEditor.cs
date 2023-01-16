using System;
using UnityEditor;

namespace OverrideEditors.Editor.Inspectors
{
    [CustomEditor(typeof(DefaultAsset), true)]
    public sealed class DefaultAssetCustomEditor : BaseCustomInspector
    {
        protected override Type DefaultEditorType
            => typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.DefaultAssetInspector");

    }
}