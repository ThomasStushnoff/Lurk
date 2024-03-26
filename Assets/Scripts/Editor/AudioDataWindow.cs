using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Objects;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom editor window for managing <see cref="AudioData"/> objects.
/// It allows the user to drag and drop AudioData objects to group them and generate an enum reference.
/// </summary>
public class AudioDataWindow : EditorWindow
{
    private Dictionary<string, List<AudioData>> _audioDataGroups = new Dictionary<string, List<AudioData>>();
    private AudioDataGroups _audioDataGroupsObject;
    private Vector2 _scrollPosition;

    private void OnEnable()
    {
        // Load or create the AudioDataGroups object.
        _audioDataGroupsObject = Resources.Load<AudioDataGroups>("Objects/AudioData/Audio Data Group");
        if (_audioDataGroups == null)
        {
            _audioDataGroupsObject = CreateInstance<AudioDataGroups>();
            AssetDatabase.CreateAsset(_audioDataGroupsObject, "Assets/Resources/Objects/Audio Data Group.asset");
            AssetDatabase.SaveAssets();
        }
        
        // Load the data from the AudioDataGroups object.
        _audioDataGroups!.Clear();
        foreach (var audioGroup in _audioDataGroupsObject.groups)
            _audioDataGroups[audioGroup.groupName] = audioGroup.audioData;
    }

    private void OnGUI()
    {
        // Set the style for the header.
        var headerStyle = new GUIStyle(EditorStyles.label)
        {
            fontSize = 13,
            fontStyle = FontStyle.Bold,
            normal = {
                textColor = Color.white,
                background = Texture2D.blackTexture
            },
            alignment = TextAnchor.MiddleCenter
        };
        
        GUILayout.Label("Generate AudioData Enum Reference", headerStyle);

        // Display the drag area.
        var dragArea = GUILayoutUtility.GetRect(0f, 50f, GUILayout.ExpandWidth(true));
        GUI.Box(dragArea, "Drop AudioData Here");
        HandleDragAndDrop(dragArea);

        // Display the audio data by group.
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
        foreach (var group in _audioDataGroups)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(group.Key, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            
            // Check if the audio data is null and remove it from the list.
            group.Value.RemoveAll(audioData => !audioData);
            
            foreach (var audioData in group.Value)
                EditorGUILayout.ObjectField(audioData, typeof(AudioData), false);
            
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }
        
        EditorGUILayout.Separator();
        
        if (GUILayout.Button("Generate Enum"))
            GenerateData();
        
        EditorGUILayout.EndScrollView();
    }

    [MenuItem("Window/Audio Data Manager")]
    public static void ShowWindow() => GetWindow<AudioDataWindow>("Audio Data Manager").CollectAudioData();
    
    /// <summary>
    /// Handles drag and drop of AudioData objects.
    /// </summary>
    /// <param name="dropArea"></param>
    private void HandleDragAndDrop(Rect dropArea)
    {
        var currentEvent = Event.current;
        var eventType = currentEvent.type;
        
        if (!dropArea.Contains(currentEvent.mousePosition)) return;
        
        switch (eventType)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                if (eventType == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    foreach (var draggedObject in DragAndDrop.objectReferences)
                    {
                        // Add the dragged object to the list.
                        switch (draggedObject)
                        {
                            case DefaultAsset folder:
                            {
                                // If the dragged object is a folder, add all audio data assets in the folder.
                                var folderPath = AssetDatabase.GetAssetPath(folder);
                                var audioDataPaths = Directory.GetFiles(folderPath, "*.asset", 
                                    SearchOption.AllDirectories);
                                
                                foreach (var audioDataPath in audioDataPaths)
                                {
                                    var draggedAudioData = AssetDatabase.LoadAssetAtPath<AudioData>(audioDataPath);
                                    if (!draggedAudioData) continue;
                                    var groupName = draggedAudioData.mixerGroup ? 
                                        draggedAudioData.mixerGroup.name : "Uncategorized";
                                        
                                    if (!_audioDataGroups.ContainsKey(groupName))
                                        _audioDataGroups[groupName] = new List<AudioData>();
                                        
                                    if (!_audioDataGroups[groupName].Contains(draggedAudioData))
                                        _audioDataGroups[groupName].Add(draggedAudioData);
                                }
                                
                                break;
                            }
                            case AudioData draggedAudioData:
                            {
                                // If the dragged object is an AudioData asset, add it directly.
                                var groupName = draggedAudioData.mixerGroup ? 
                                    draggedAudioData.mixerGroup.name : "Uncategorized";
                                
                                if (!_audioDataGroups.ContainsKey(groupName))
                                    _audioDataGroups[groupName] = new List<AudioData>();

                                if (!_audioDataGroups[groupName].Contains(draggedAudioData))
                                    _audioDataGroups[groupName].Add(draggedAudioData);
                                
                                break;
                            }
                        }
                    }
                }
                
                break;
        }
        
        // Update the AudioDataGroups object.
        _audioDataGroupsObject.groups.Clear();
        foreach (var (key, value) in _audioDataGroups)
        {
            var audioGroup = new AudioGroup {groupName = key, audioData = value};
            _audioDataGroupsObject.groups.Add(audioGroup);
        }
        
        EditorUtility.SetDirty(_audioDataGroupsObject);
    }
    
    /// <summary>
    /// Collects all AudioData objects in the resources folder.
    /// </summary>
    private void CollectAudioData()
    {
        _audioDataGroups = new Dictionary<string, List<AudioData>>();
        var allAudioData = Resources.FindObjectsOfTypeAll<AudioData>();

        foreach (var audioData in allAudioData)
        {
            var groupName = audioData.mixerGroup != null ? audioData.mixerGroup.name : "Uncategorized";
            if (!_audioDataGroups.ContainsKey(groupName))
                _audioDataGroups[groupName] = new List<AudioData>();

            _audioDataGroups[groupName].Add(audioData);
        }
    }
    
    /// <summary>
    /// Generates enum and map data for all AudioData objects in the project.
    /// </summary>
    private void GenerateData()
    {
        var enumsFilePath = Path.Combine(Application.dataPath, "Scripts/Audio/AudioDataEnum.cs");
        var mapsFilePath = Path.Combine(Application.dataPath, "Scripts/Audio/AudioDataMap.cs");
        
        // Delete the files if they exist.
        if (File.Exists(enumsFilePath)) File.Delete(enumsFilePath);
        if (File.Exists(mapsFilePath)) File.Delete(mapsFilePath);
        
        // Load enum mappings from the AudioDataGroups object.
        var existingMap = new Dictionary<string, int>();
        foreach (var enumMapping in _audioDataGroupsObject.enumMappings)
            existingMap[enumMapping.enumName] = enumMapping.enumValue;
        
        // Get all valid enum names.
        var validNames = new HashSet<string>();
        foreach (var validEnumName in _audioDataGroups.SelectMany(group => group.Value.Select(audioData => 
                     audioData.name.Replace(" ", "_").Replace("-", "_"))))
            validNames.Add(validEnumName);
        
        // Initialize the string builders.
        var enumBuilder = new StringBuilder();
        var mapBuilder = new StringBuilder();
        
        // Enums header.
        enumBuilder.AppendLine("// This file is auto-generated. Do not manually modify.\n" +
                               "// To regenerate this file,\n" +
                               "// Go to Window > Audio Data Manager. Then, drag all AudioData and click Generate Enum.\n");
        enumBuilder.AppendLine("namespace Audio");
        enumBuilder.AppendLine("{");
        
        // Maps header.
        mapBuilder.AppendLine("// This file is auto-generated. Do not manually modify.\n" +
                              "// To regenerate this file,\n" +
                              "// Go to Window > Audio Data Manager. Then, drag all AudioData and click Generate Enum.\n");
        mapBuilder.AppendLine("using System.Collections.Generic;");
        mapBuilder.AppendLine("using Objects;");
        mapBuilder.AppendLine("using UnityEngine;");
        mapBuilder.AppendLine();
        mapBuilder.AppendLine("namespace Audio");
        mapBuilder.AppendLine("{");
        
        foreach (var group in _audioDataGroups)
        {
            var groupName = group.Key.Replace(" ", "");
            var enumName = $"AudioDataEnum{groupName}";
            var sortedAudioData = group.Value.OrderBy(a => a.name).ToList();
            
            // Initialize the next value.
            var nextValue = 1;
            
            // Start enum.
            enumBuilder.AppendLine($"    public enum {enumName}");
            enumBuilder.AppendLine("    {");
            
            // Add None as the first element.
            enumBuilder.AppendLine("        None = 0,");
            
            foreach (var audioData in sortedAudioData)
            {
                // Replace spaces and hyphens with underscores.
                var validEnumName = audioData.name.Replace(" ", "_").Replace("-", "_");
                if (!existingMap.ContainsKey(validEnumName))
                {
                    // Add the new enum to the map.
                    existingMap[validEnumName] = nextValue++;
                }
                else if (group.Value.Contains(audioData))
                {
                    // Update the enum value.
                    existingMap[validEnumName] = nextValue++;
                }
                
                // Append the enum value to the enum.
                enumBuilder.AppendLine($"        {validEnumName} = {existingMap[validEnumName]},");
            }
            
            // End enum.
            enumBuilder.AppendLine("    }");
            
            // Start map.
            mapBuilder.AppendLine($"    public static class AudioDataMap{groupName}");
            mapBuilder.AppendLine("    {");
            mapBuilder.AppendLine($"        public static readonly Dictionary<{enumName}, AudioData> Map = new Dictionary<{enumName}, AudioData>();");
            
            // Generate constructor
            mapBuilder.AppendLine($"        static AudioDataMap{groupName}()");
            mapBuilder.AppendLine("        {");
            
            // Populate the map.
            foreach (var audioData in sortedAudioData)
            {
                // Replace spaces and hyphens with underscores.
                var validEnumName = audioData.name.Replace(" ", "_").Replace("-", "_");
                mapBuilder.AppendLine($"            Map.Add({enumName}.{validEnumName}, Resources.Load<AudioData>(\"{AssetDatabase.GetAssetPath(audioData).Replace("Assets/Resources/", "").Replace(".asset", "")}\"));");
            }
            
            // End constructor and map.
            mapBuilder.AppendLine("        }");
            mapBuilder.AppendLine("    }");
        }
        
        // Close namespaces.
        enumBuilder.AppendLine("}");
        mapBuilder.AppendLine("}");
        
        // Update the enum mappings in the AudioDataGroups object.
        _audioDataGroupsObject.enumMappings.Clear();
        foreach (var validEnumName in validNames.Where(validEnumName => existingMap.ContainsKey(validEnumName)))
            _audioDataGroupsObject.enumMappings.Add(new AudioEnumMapping
            {
                enumName = validEnumName, enumValue = existingMap[validEnumName]
            });
        
        // Write the files.
        File.WriteAllText(enumsFilePath, enumBuilder.ToString());
        File.WriteAllText(mapsFilePath, mapBuilder.ToString());
        
        // Save the AudioDataGroups object.
        EditorUtility.SetDirty(_audioDataGroupsObject);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}