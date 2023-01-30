using System;
using OverrideEditors.Editor.Editors;
using UnityEditor;
using UnityEngine;

namespace OverrideEditors.Editor.Inspectors
{
    public abstract class BaseCustomInspector : UnityEditor.Editor
    {
        private static readonly GUIContent[] ToolbarContents = {
            new("Custom"),
            new("Raw")
        };

        protected abstract Type DefaultEditorType { get; }
        
        private UnityEditor.Editor defaultEditor;
        private OverrideEditor customInspector;
        [SerializeField] private int drawMode;

        private void OnEnable()
        {
            defaultEditor ??= CreateEditor(target, DefaultEditorType);
            customInspector = OverrideEditorSettings.instance.GetOverrideEditor(target);
            customInspector?.Enable(target);
        }

        private void OnDisable()
        {
            customInspector?.Disable();
            customInspector = null;
            DestroyImmediate(defaultEditor);
        }

        public override void OnInspectorGUI()
        {
            if (customInspector == null)
            {
                if (defaultEditor != null)
                    defaultEditor.OnInspectorGUI();
                return;
            }

            var enabled = GUI.enabled;
            GUI.enabled = true;
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
                drawMode = GUILayout.SelectionGrid(drawMode, ToolbarContents, ToolbarContents.Length, EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
                EditorGUILayout.EndHorizontal();
            
                switch (drawMode)
                {
                    case 0:
                        customInspector.OnInspectorGUI();
                        break;
                    case 1:
                        if (defaultEditor != null)
                            defaultEditor.OnInspectorGUI();
                        break;
                }
            }
            GUI.enabled = enabled;
        }
    }
}