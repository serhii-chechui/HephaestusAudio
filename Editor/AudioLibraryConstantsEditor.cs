using System.IO;
using System.Text;
using Hephaestus.Audio.SoundsLibrary;
using UnityEditor;
using UnityEngine;

namespace Hephaestus.Audio.Editor
{
    [CustomEditor(typeof(AudioLibraryConstants))]
    public class AudioLibraryConstantsEditor : UnityEditor.Editor
    {
        private const string EntityType = "Audio";
        
        private StringBuilder _stringBuilder;
        
        private string _enumClassName = "AudioLibraryConstants";

        private string _newConstantKey;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var mapConstants = (AudioLibraryConstants)target;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            EditorGUILayout.LabelField("Export:", EditorStyles.largeLabel);
            
            _enumClassName = EditorGUILayout.TextField("Enum Class Name:", _enumClassName);
            
            EditorGUILayout.BeginHorizontal();

            if (!string.IsNullOrEmpty(mapConstants.enumsPath))
            {
                EditorGUILayout.LabelField("Path", mapConstants.enumsPath, GUILayout.ExpandWidth(true));
            }
            
            if (GUILayout.Button("Pick", GUILayout.Width(96)))
            {
                mapConstants.enumsPath = EditorUtility.OpenFolderPanel("Pick The Folder", "", "");
            }
            
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Export to enum", GUILayout.ExpandWidth(true), GUILayout.Height(32)))
            {
                ExportKeysToEnum(mapConstants);
            }
            
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space();
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            EditorGUILayout.LabelField($"Add New {EntityType} Keys:", EditorStyles.largeLabel);
            
            _newConstantKey = EditorGUILayout.TextField("New Key:", _newConstantKey).ToUpper();

            if (GUILayout.Button($"Add New {EntityType} Key", GUILayout.ExpandWidth(true), GUILayout.Height(32)))
            {
                mapConstants.soundMapKeys.Add(_newConstantKey);
            }
            
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space();
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            EditorGUILayout.LabelField($"List of {EntityType} Keys:", EditorStyles.largeLabel);

            if (mapConstants.soundMapKeys == null) return;

            for (int i = 0; i < mapConstants.soundMapKeys.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                
                mapConstants.soundMapKeys[i] = EditorGUILayout.TextField(mapConstants.soundMapKeys[i]);
                
                if (GUILayout.Button("Remove"))
                {
                    mapConstants.soundMapKeys.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Save Config", GUILayout.ExpandWidth(true), GUILayout.Height(32))) {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
        
        private void ExportKeysToEnum(AudioLibraryConstants audioLibraryConstants)
        {
            _stringBuilder = new StringBuilder();
            
            _stringBuilder.Append($"public enum {_enumClassName} : byte\n");
            
            _stringBuilder.Append("{\n");

            for (var i = 0; i < audioLibraryConstants.soundMapKeys.Count; i++)
            {
                var coma = i < audioLibraryConstants.soundMapKeys.Count - 1 ? "," : string.Empty;
                _stringBuilder.Append($"\t{audioLibraryConstants.soundMapKeys[i].ToUpper()} = {i}{coma}\n");
            }
            
            _stringBuilder.Append("}");

            var filename = $"{_enumClassName}.cs";
            File.WriteAllText(Path.Combine(audioLibraryConstants.enumsPath, filename), _stringBuilder.ToString());
            
            AssetDatabase.Refresh();
        }
    }
}