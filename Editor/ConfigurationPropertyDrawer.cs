using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace B.PerAssetEditors
{
    [UsedImplicitly]
    public abstract class ConfigurationPropertyDrawer : PropertyDrawer
    {
        private static readonly GUIContent None = new("None");
        private static readonly Dictionary<Type, GUIContent> Contents = new();
        private static Type[] types;

        public sealed override float GetPropertyHeight(SerializedProperty property, GUIContent label) => GetContentHeight() + EditorGUIUtility.singleLineHeight;

        public sealed override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //position = EditorGUI.PrefixLabel(position, label);
            var contentRect = new Rect(position.x, position.y, position.width, position.height - EditorGUIUtility.singleLineHeight);
            OnGUI(contentRect, property);

            position.y += position.height -= EditorGUIUtility.singleLineHeight;
            var typeDataProperty = property.FindPropertyRelative("typeData");
            var selected = TypeSerializer.TryGetType(typeDataProperty.stringValue, out var type)
                ? (type, new GUIContent(type.Name))
                : (null, None);

            var typeDataLabel = EditorGUI.BeginProperty(position, new GUIContent("Handler Type"), typeDataProperty);
            position = EditorGUI.PrefixLabel(position, typeDataLabel);
            if (EditorGUI.DropdownButton(position, selected.Item2, FocusType.Passive))
                ShowDropdown(selected.Item1, typeDataProperty);
            EditorGUI.EndProperty();
        }
        
        private void ShowDropdown(Type selectedType, SerializedProperty dataProperty)
        {
            var menu = new GenericMenu();

            // none selection
            if (selectedType == null) menu.AddDisabledItem(None);
            else menu.AddItem(None, false, OnMenuItemClick, null);

            types ??= TypeCache.GetTypesDerivedFrom(typeof(CustomAssetInspector))
                               .Where(type => !type.IsAbstract)
                               .Where(type => !type.IsInterface)
                               .ToArray();
            
            // type selection
            foreach (var type in types)
            {
                if (!Contents.TryGetValue(type, out var content))
                    content = Contents[type] = new GUIContent(type.FullName.Replace(".", "/"));

                if (type == selectedType)
                {
                    menu.AddDisabledItem(content);
                }
                else
                {
                    menu.AddItem(content, false, OnMenuItemClick, type);
                }
            }

            menu.ShowAsContext();

            void OnMenuItemClick(object userData)
            {
                var type = (Type)userData;
                TypeSerializer.TryGetData(type, out var data);
                dataProperty.stringValue = data;
                dataProperty.serializedObject.ApplyModifiedProperties();
            }
        }

        protected abstract void OnGUI(Rect position, SerializedProperty property);
        protected abstract float GetContentHeight();
    }

    [CustomPropertyDrawer(typeof(AssetConfiguration))]
    public sealed class AssetConfigurationPropertyDrawer : ConfigurationPropertyDrawer
    {
        protected override float GetContentHeight() => EditorGUIUtility.singleLineHeight;
        
        protected override void OnGUI(Rect position, SerializedProperty property)
        {
            //position = EditorGUI.PrefixLabel(position, new GUIContent(property.displayName));
            EditorGUI.ObjectField(position, property.FindPropertyRelative("asset"));
        }
    }
}