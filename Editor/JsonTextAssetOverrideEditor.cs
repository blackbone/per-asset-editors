using System;
using System.IO;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace B.PerAssetEditors
{
    public abstract class JsonTextAssetOverrideEditor<TData> : OverrideEditor<TextAsset> where TData : class, new()
    {
        private static JsonSerializerSettings jsonSettings;

        private static JsonSerializerSettings JsonSettings
        {
            get
            {
                if (jsonSettings != null)
                    return jsonSettings;

                if (JsonConvert.DefaultSettings != null)
                    jsonSettings = JsonConvert.DefaultSettings();
                
                if (jsonSettings == null)
                    throw new Exception();
                
                jsonSettings.NullValueHandling = NullValueHandling.Ignore;
                jsonSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
                jsonSettings.Formatting = Formatting.Indented;
                return jsonSettings;
            }
        }
        
        protected TData Data { get; private set; }
        protected abstract Type DataWrapperType { get; }

        private GenericDataWrapperScriptableObject<TData> dataWrapper;
        private SerializedObject serializedObject;
        private SerializedProperty dataProperty;

        protected virtual bool TryDeserialize(TextAsset asset, out TData data)
        {
            data = JsonConvert.DeserializeObject<TData>(asset.text);
            return true;
        }

        protected virtual bool TrySerialize(TData data, TextAsset asset)
        {
            data ??= new TData();
            var text = JsonConvert.SerializeObject(data, JsonSettings);
            File.WriteAllText(AssetDatabase.GetAssetPath(asset), text);
            return true;
        }

        protected override void ApplyChangesInternal()
        {
            base.ApplyChangesInternal();
            
            serializedObject.ApplyModifiedProperties();
            Data = dataWrapper.data;
            
            if (!TrySerialize(Data, Target))
                throw new SerializationException("Failed to serialize data");
            
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(Target));
        }

        protected sealed override void EnableInternal()
        {
            base.EnableInternal();
            
            if (!TryDeserialize(Target, out var data))
                throw new SerializationException("Failed to deserialize data");

            Data = data;
            
            dataWrapper = ScriptableObject.CreateInstance(DataWrapperType) as GenericDataWrapperScriptableObject<TData>;
            dataWrapper.data = data;
            
            serializedObject = new SerializedObject(dataWrapper);
            dataProperty = serializedObject.FindProperty(nameof(GenericDataWrapperScriptableObject<TData>.data));
        }

        protected sealed override void DisableInternal()
        {
            base.EnableInternal();
            
            ApplyChangesInternal();

            UnityEngine.Object.DestroyImmediate(dataWrapper);
            dataWrapper = null;
            serializedObject.Dispose();
            serializedObject = null;
            dataProperty.Dispose();
            dataProperty = null;
        }

        protected override bool OnInspectorGUIInternal()
        {
            EditorGUI.BeginChangeCheck();
            var endProperty = dataProperty.GetEndProperty();
            var nextProperty = dataProperty.Copy();
            nextProperty.NextVisible(true);

            while (nextProperty.propertyPath != endProperty.propertyPath)
            {
                EditorGUILayout.PropertyField(nextProperty);
                nextProperty.NextVisible(false);
            }
            return EditorGUI.EndChangeCheck();
        }

        [Serializable]
        public abstract class GenericDataWrapperScriptableObject<T> : ScriptableObject
        {
            [SerializeField] internal T data;
        }
    }
}