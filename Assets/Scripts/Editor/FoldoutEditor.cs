// WIP
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// Custom editor for serializable objects that groups fields under <see cref="FoldoutAttribute" />.
/// </summary>
[CustomEditor(typeof(Object), true, isFallback = true)]
[CanEditMultipleObjects]
public class GroupFoldoutEditor : Editor
{
    private Dictionary<string, bool> _foldoutStates = new Dictionary<string, bool>();
    private Dictionary<string, List<SerializedProperty>> _groupedProperties = new Dictionary<string, List<SerializedProperty>>();
    private List<SerializedProperty> _ungroupedProperties = new List<SerializedProperty>();
    private GUIStyle _style;
    private bool _initialized;
    
    /// <summary>
    /// Invoked when the editor is reloaded.
    /// Fixes the issue where all fields are not shown after scripts are reloaded.
    /// </summary>
    public static event Action OnEditorReload;
    
    private void OnEnable() => OnEditorReload += Reinitialize;
    
    private void OnDisable() => OnEditorReload -= Reinitialize;
    
    /// <summary>
    /// Reinitializes the editor.
    /// </summary>
    private void Reinitialize()
    {
        _initialized = false;
        _style = null;
    }
    
    /// <summary>
    /// Initializes the style for the foldout.
    /// </summary>
    private void InitializeStyle()
    {
        _style = new GUIStyle(EditorStyles.foldout)
        {
            fontSize = 13,
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.white },
        };
    }
    
    /// <summary>
    /// Initializes the editor.
    /// </summary>
    private void Initialize()
    {
        _groupedProperties.Clear();
        _ungroupedProperties.Clear();
        _foldoutStates.Clear();
        
        var property = serializedObject.GetIterator();
        var exploreChildren = true;
        while (property.NextVisible(exploreChildren))
        {
            exploreChildren = false;
            var attribute = GetPropertyAttribute<FoldoutAttribute>(property);
            if (attribute != null)
            {
                if (!_groupedProperties.ContainsKey(attribute.Name))
                {
                    _groupedProperties.Add(attribute.Name, new List<SerializedProperty>());
                    _foldoutStates[attribute.Name] = attribute.StartOpen;
                }
                
                _groupedProperties[attribute.Name].Add(property.Copy());
            }
            else
            {
                _ungroupedProperties.Add(property.Copy());
            }
        }
        
        _initialized = true;
    }
    
    public override void OnInspectorGUI()
    {
        if (!_initialized) Initialize();
        if (_style == null) InitializeStyle();
        
        serializedObject.Update();
        
        // Draw ungrouped properties.
        foreach (var prop in _ungroupedProperties)
            EditorGUILayout.PropertyField(prop, true);
        
        // Draw grouped properties.
        foreach (var (key, value) in _groupedProperties)
        {
            _foldoutStates[key] = EditorGUILayout.Foldout(_foldoutStates[key], new GUIContent(key), true , _style);
            
            if (_foldoutStates[key])
            {
                EditorGUI.indentLevel++;
                foreach (var prop in value)
                    EditorGUILayout.PropertyField(prop, true);
                EditorGUI.indentLevel--;
            }
        }
        
        serializedObject.ApplyModifiedProperties();
    }
    
    /// <summary>
    /// Invoked when scripts are reloaded in the editor.
    /// </summary>
    [DidReloadScripts]
    private static void OnScriptReload() => OnEditorReload?.Invoke();
    
    /// <summary>
    /// Gets the attribute of type <typeparamref name="T"/> from the serialized property.
    /// </summary>
    /// <param name="property">The serialized property to get the attribute from.</param>
    /// <typeparam name="T">The type of the attribute to get.</typeparam>
    /// <returns>The attribute of type <typeparamref name="T"/>. </returns>
    private T GetPropertyAttribute<T>(SerializedProperty property) where T : Attribute
    {
        var field = property.serializedObject.targetObject.GetType()
            .GetField(property.name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        return field != null ? (T)Attribute.GetCustomAttribute(field, typeof(T)) : null;
    }
}