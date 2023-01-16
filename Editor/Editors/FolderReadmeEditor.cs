using System.IO;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Scripting;

namespace OverrideEditors.Editor.Editors
{
    [Preserve]
    public sealed class FolderReadmeEditor : OverrideEditor<DefaultAsset>
    {
        private const string ReadmeFileName = "readme~";
        private static readonly GUILayoutOption[] Options = {
            GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)
        };
        
        private bool isFolder;
        
        private string readmePath;
        private FileInfo readmeFileInfo;
        private bool existsPreviously;
        private string readme;
        private bool notSaved;
        private long lastFileVersion;

        protected override void EnableInternal()
        {
            base.EnableInternal();

            var folderPath = AssetDatabase.GetAssetPath(Target);
            isFolder = AssetDatabase.IsValidFolder(folderPath);
            
            if (!isFolder)
                return;

            readmePath = Path.Combine(folderPath, ReadmeFileName);
            readmeFileInfo = new FileInfo(readmePath);
            existsPreviously = readmeFileInfo.Exists;
            if (existsPreviously)
            {
                readme = File.ReadAllText(readmePath);
                lastFileVersion = readmeFileInfo.LastWriteTime.Ticks;
            }
        }

        protected override void DisableInternal()
        {
            base.DisableInternal();
            
            SaveIfNotSaved(true);
        }

        protected override bool OnInspectorGUIInternal()
        {
            readmeFileInfo.Refresh();
            
            // check need update readme content
            // if file newerly created
            if (!existsPreviously && readmeFileInfo.Exists)
            {
                readme = File.ReadAllText(readmePath);
                lastFileVersion = new FileInfo(readmePath).LastWriteTime.Ticks;
                existsPreviously = true;
            }
            // or file has been updated externally
            else if (readmeFileInfo.Exists && lastFileVersion < readmeFileInfo.LastWriteTime.Ticks)
            {
                readme = File.ReadAllText(readmePath);
                lastFileVersion = new FileInfo(readmePath).LastWriteTime.Ticks;
            }
            
            if (!EditorGUIUtility.editingTextField)
                SaveIfNotSaved(false);
            
            EditorGUILayout.BeginVertical(Options);
            var newReadme = EditorGUILayout.TextArea(readme, Options);
            EditorGUILayout.EndVertical();

            if (newReadme != readme)
            {
                readme = newReadme;
                notSaved = true;
            }

            return false;
        }

        private void SaveIfNotSaved(bool force)
        {
            if (!notSaved && !force)
                return;

            var hasFileContents = !string.IsNullOrEmpty(readme);
            if (hasFileContents)
            {
                File.WriteAllText(readmePath, readme);
                lastFileVersion = new FileInfo(readmePath).LastWriteTime.Ticks;
            }
            else
            {
                File.Delete(readmePath);
                lastFileVersion = -1;
            }
            
            existsPreviously = hasFileContents;
            notSaved = false;
        }
    }
}