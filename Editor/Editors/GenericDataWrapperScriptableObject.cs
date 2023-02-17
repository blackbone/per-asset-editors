using System;
using UnityEngine;

namespace OverrideEditors.Editor.Editors
{
    [Serializable]
    public abstract class GenericDataWrapperScriptableObject<T> : ScriptableObject
    {
        [SerializeField] internal T data;
            
        protected T Data
        {
            get => data;
            set => data = value;
        }
    }
}