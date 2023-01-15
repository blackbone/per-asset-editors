using UnityEditor;
using UnityEngine;

namespace B.PerAssetEditors
{
    public abstract class OverrideEditor
    {
        protected Object RawTarget { get; private set; }

        internal void Enable(Object target)
        {
            RawTarget = target;
            OnEnable();
        }

        internal void Disable()
        {
            OnDisable();
            RawTarget = null;
        }

        internal void OnInspectorGUI()
        {
            if (RawTarget == null)
                return;

            if (OnInspectorGUIInternal())
                OnAssetDirty();
        }

        protected virtual void OnEnable()
        {
            // no op
        }

        protected virtual void OnDisable()
        {
            // no op
        }

        protected virtual void OnAssetDirty()
        {
            // no op
        }

        protected virtual bool OnInspectorGUIInternal()
        {
            EditorGUILayout.LabelField("GUI are not implemented.");
            return false;
        }
    }
}