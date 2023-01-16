using System;
using UnityEditor;
using UnityEngine;

namespace OverrideEditors.Editor.Inspectors
{
    [CustomEditor(typeof(TextAsset), true)]
    public sealed class TextAssetCustomEditor : BaseCustomInspector
    {
        protected override Type DefaultEditorType
            => typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.TextAssetInspector");
    }
}