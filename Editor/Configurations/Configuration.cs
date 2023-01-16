using System;
using UnityEngine;

namespace OverrideEditors.Editor.Configurations
{
    [Serializable]
    internal abstract class Configuration : ISerializationCallbackReceiver
    {
        [SerializeField] private string editorTypeData;
        [NonSerialized] private Type editorType;
        public Type EditorType => editorType;
        
        
        public virtual void OnBeforeSerialize() => TypeSerializer.TryGetData(editorType, out editorTypeData);

        public virtual void OnAfterDeserialize() => TypeSerializer.TryGetType(editorTypeData, out editorType);
    }
}