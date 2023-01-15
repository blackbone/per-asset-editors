using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace B.PerAssetEditors.PropertyDrawers
{
    [UsedImplicitly]
    public abstract class ConfigurationPropertyDrawer : PropertyDrawer
    {
        private static readonly GUIContent None = new("None");
        private static readonly Dictionary<Type, GUIContent> Contents = new();
        private static Dictionary<Type, Type[]> typesCache = new();

        public sealed override float GetPropertyHeight(SerializedProperty property, GUIContent label) => GetContentHeight() + EditorGUIUtility.singleLineHeight;

        public sealed override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var contentRect = new Rect(position.x, position.y, position.width, position.height - EditorGUIUtility.singleLineHeight);
            OnGUI(contentRect, property);

            position.y += position.height -= EditorGUIUtility.singleLineHeight;
            DrawTypeSelector<OverrideEditor>(position, property, "editorTypeData", "Handler Type");
        }

        protected void DrawTypeSelector<T>(Rect position, SerializedProperty property, string propertyName, string displayName)
        {
            var typeDataProperty = property.FindPropertyRelative(propertyName);
            var selected = TypeSerializer.TryGetType(typeDataProperty.stringValue, out var type)
                ? (type, new GUIContent(type.Name))
                : (null, None);

            position = EditorGUI.PrefixLabel(position, new GUIContent(displayName));
            if (EditorGUI.DropdownButton(position, selected.Item2, FocusType.Passive))
                ShowDropdown<T>(selected.Item1, typeDataProperty);
        }
        
        private void ShowDropdown<T>(Type selectedType, SerializedProperty dataProperty)
        {
            var menu = new GenericMenu();

            // none selection
            if (selectedType == null) menu.AddDisabledItem(None);
            else menu.AddItem(None, false, OnMenuItemClick, null);

            if (!typesCache.TryGetValue(typeof(T), out var types))
            {
                typesCache[typeof(T)] = types = TypeCache.GetTypesDerivedFrom(typeof(T))
                                                      .Where(type => !type.IsAbstract)
                                                      .Where(type => !type.IsInterface)
                                                      .OrderBy(type => type.FullName)
                                                      .ToArray();
            }

            // type selection
            foreach (var type in types)
            {
                if (!Contents.TryGetValue(type, out var content))
                    content = Contents[type] = new GUIContent(type.FullName.Replace(".", "/"));

                if (type == selectedType) menu.AddDisabledItem(content);
                else menu.AddItem(content, false, OnMenuItemClick, type);
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
}