using System;
using UnityEditor;
using UnityEngine;

namespace B.PerAssetEditors
{
    [CustomEditor(typeof(TextAsset), true)]
    [Serializable]
    public class TextAssetCustomEditor : Editor
    {
        private static readonly Type TextAssetDefaultInspector = typeof(Editor).Assembly.GetType("UnityEditor.TextAssetInspector");
        private static readonly GUIContent[] ToolbarContents = {
            new("Custom"),
            new("Raw")
        };

        private Editor textAssetInspector;
        private CustomAssetInspector customAssetInspector;
        [SerializeField] private int drawMode;

        private void OnEnable()
        {
            textAssetInspector ??= CreateEditor(target, TextAssetDefaultInspector);
            customAssetInspector = DefaultAssetCustomInspectorConfiguration.Instance.GetInspector(target);
            customAssetInspector?.Enable(target);
        }

        private void OnDisable()
        {
            DestroyImmediate(textAssetInspector);
            customAssetInspector?.Disable();
            customAssetInspector = null;
        }

        public override void OnInspectorGUI()
        {
            var enabled = GUI.enabled;
            GUI.enabled = true;
            if (customAssetInspector == null)
            {
                textAssetInspector.OnInspectorGUI();
                return;
            }
            
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
            drawMode = GUILayout.SelectionGrid(drawMode, ToolbarContents, ToolbarContents.Length, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();

            switch (drawMode)
            {
                case 0:
                    customAssetInspector.OnInspectorGUI();
                    break;
                case 1:
                    textAssetInspector.OnInspectorGUI();
                    break;
            }
            
            GUI.enabled = enabled;
        }
    }
}