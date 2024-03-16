using System.Collections.Generic;
using Objects;
using UnityEditor;
using UnityEngine;

// Work in progress.
// Goal is to generate enum reference of all AudioData objects in the project.
// This will allow for easy access to AudioData in the AudioManager and other scripts.
//
// TODO:
// AudioDataWindow.cs
// Get all AudioData objects in the project. (DONE)
// Display them in a window. (DONE)
// Allow for drag and drop of AudioData objects. (DONE)
// Group AudioData objects by their AudioMixerGroup. (DONE)
// Allow for easy access to AudioData objects in the AudioManager and other scripts. (TODO)
// Use tree view or nested lists to display AudioData objects. (TODO)
// Allow for easy access to AudioData objects in the AudioManager and other scripts. (TODO)
//
// AudioManager.cs
// Use the enum reference to easily access AudioData objects. (TODO)
public class AudioDataWindow : EditorWindow
{
    private readonly Dictionary<string, List<AudioData>> _audioDataGroups = new Dictionary<string, List<AudioData>>();
    private Vector2 _scrollPosition;

    private void OnGUI()
    {
        GUILayout.Label("Drag AudioData objects here:", EditorStyles.boldLabel);
        
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
            foreach (var audioData in group.Value)
                EditorGUILayout.ObjectField(audioData, typeof(AudioData), false);

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndScrollView();
    }

    [MenuItem("Window/Audio Data Manager")]
    public static void ShowWindow() => GetWindow<AudioDataWindow>("Audio Data Manager");

    private void HandleDragAndDrop(Rect dropArea)
    {
        var currentEvent = Event.current;
        var eventType = currentEvent.type;

        if (!dropArea.Contains(currentEvent.mousePosition))
            return;

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
                        var draggedAudioData = draggedObject as AudioData;
                        if (draggedAudioData != null)
                        {
                            var groupName = draggedAudioData.mixerGroup != null ? draggedAudioData.mixerGroup.name : "Uncategorized";
                            if (!_audioDataGroups.ContainsKey(groupName))
                                _audioDataGroups[groupName] = new List<AudioData>();
                            
                            if (!_audioDataGroups[groupName].Contains(draggedAudioData))
                                _audioDataGroups[groupName].Add(draggedAudioData);
                        }
                    }
                    
                    Repaint();
                }
                
                break;
        }
    }
}