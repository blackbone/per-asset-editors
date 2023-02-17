using System;
using System.IO;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace OverrideEditors.Editor.Editors
{
    public abstract class JsonTextAssetOverrideEditor<TData> : OverrideEditor<TextAsset>
    {
        private static JsonSerializerSettings jsonSettings;
        private SerializedProperty dataProperty;

        private GenericDataWrapperScriptableObject<TData> dataWrapper;
        private SerializedObject serializedObject;

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

        protected virtual bool TryDeserialize(TextAsset asset, out TData data)
        {
            try
            {
                data = JsonConvert.DeserializeObject<TData>(asset.text);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                data = default;
                return false;
            }
        }

        protected virtual bool Serialize(TData data, TextAsset asset)
        {
            data ??= Activator.CreateInstance<TData>();
            var path = AssetDatabase.GetAssetPath(asset);
            var previousText = File.Exists(path) ? File.ReadAllText(path) : string.Empty;
            string text;
            try
            {
                text = JsonConvert.SerializeObject(data, JsonSettings);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }

            if (previousText != text)
            {
                File.WriteAllText(path, text);
                return true;
            }

            return false;
        }

        protected override void ApplyChangesInternal()
        {
            base.ApplyChangesInternal();

            serializedObject?.ApplyModifiedProperties();

            if (dataWrapper == null)
                return;

            Data = dataWrapper.data;
            if (Serialize(Data, Target))
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(Target));
        }

        protected sealed override void EnableInternal()
        {
            base.EnableInternal();

            if (!TryDeserialize(Target, out var data))
                throw new SerializationException("Failed to deserialize data");

            Data = data;

            dataWrapper = ScriptableObject.CreateInstance(DataWrapperType) as GenericDataWrapperScriptableObject<TData>;
            if (dataWrapper == null)
                throw new InvalidCastException("Something wend wrong creating data wrapper scriptable object");
            
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
            if (dataProperty.isArray)
            {
                dataProperty.isExpanded = true;
                EditorGUILayout.PropertyField(dataProperty);
            }
            else
            {
                var endProperty = dataProperty.GetEndProperty();
                var nextProperty = dataProperty.Copy();
                nextProperty.NextVisible(true);

                while (nextProperty.propertyPath != endProperty.propertyPath)
                {
                    EditorGUILayout.PropertyField(nextProperty);
                    nextProperty.NextVisible(false);
                }
            }

            return EditorGUI.EndChangeCheck();
        }
    }
}