using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// Custom editor for serializable objects that groups fields under <see cref="FoldoutAttribute" />.
/// </summary>
[CustomEditor(typeof(Object), true, isFallback = true)]
[CanEditMultipleObjects]
public class FoldoutEditor : Editor
{
    private Dictionary<string, FoldoutGroup> _foldoutGroups;
    // private Style _style;
    private bool _initialized;
    
    private void OnEnable()
    {
        Initialize();
    }
    
    private void Initialize()
    {
        _foldoutGroups = new Dictionary<string, FoldoutGroup>();
        // _style = new Style();
        FoldoutAttribute currentGroupAttr = null;
        
        var property = serializedObject.GetIterator();
        var inGroup = false;
        while (property.NextVisible(true))
        {
            var attribute = GetPropertyAttribute<FoldoutAttribute>(property);
            if (attribute != null)
            {
                currentGroupAttr = attribute;
                inGroup = true;

                if (!_foldoutGroups.ContainsKey(attribute.Name))
                    _foldoutGroups[attribute.Name] = new FoldoutGroup(attribute.Name, attribute.FoldEverything);
            }
            
            if (inGroup && currentGroupAttr != null)
            {
                if ((!_foldoutGroups[currentGroupAttr.Name].Properties.Any() || currentGroupAttr.FoldEverything))
                    _foldoutGroups[currentGroupAttr.Name].Properties.Add(property.Copy());
            }

            if (attribute == null && !currentGroupAttr?.FoldEverything == true)
                inGroup = false;
        }
        
        _initialized = true;
    }
    
    public override void OnInspectorGUI()
    {
        if (!_initialized) Initialize();
        
        serializedObject.Update();
        
        // Draw each foldout group.
        foreach (var group in _foldoutGroups.Values)
        {
            // Begin the Box styled group.
            EditorGUILayout.BeginVertical(StyleFramework.Box);
            
            // Draw the foldout header.
            group.Expanded = EditorGUILayout.Foldout(group.Expanded, group.Name, true, StyleFramework.FoldoutHeader);
            var rect = GUILayoutUtility.GetLastRect();
            rect.x -= 20;
            rect.y -= 5;
            rect.height += 10;
            rect.width += 24;
            EditorGUI.LabelField(rect, GUIContent.none, EditorStyles.helpBox);
            
            // Begin the BoxChild styled group if expanded.
            if (group.Expanded)
            {
                EditorGUILayout.BeginVertical(StyleFramework.BoxChild);
                // Draw the properties in the group.
                foreach (var property in group.Properties)
                    EditorGUILayout.PropertyField(property, true);
                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.EndVertical();
            // EditorGUILayout.Separator();
        }
        
        serializedObject.ApplyModifiedProperties();
    }
    
    /// <summary>
    /// Gets the attribute of type <typeparamref name="T"/> from the serialized property.
    /// </summary>
    /// <param name="property">The serialized property to get the attribute from.</param>
    /// <typeparam name="T">The type of the attribute to get.</typeparam>
    /// <returns>The attribute of type <typeparamref name="T"/>. </returns>
    private static T GetPropertyAttribute<T>(SerializedProperty property) where T : Attribute
    {
        var field = property.serializedObject.targetObject.GetType()
            .GetField(property.name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        return field != null ? (T)Attribute.GetCustomAttribute(field, typeof(T)) : null;
    }
    
    /// <summary>
    /// Represents a group of properties under a foldout.
    /// </summary>
    private class FoldoutGroup
    {
        public readonly string Name;
        public readonly List<SerializedProperty> Properties = new List<SerializedProperty>();
        
        private bool _expanded;
        public bool Expanded
        {
            get => _expanded;
            set
            {
                if (_expanded != value)
                {
                    _expanded = value;
                    EditorPrefs.SetBool(Name + "_expanded", value);
                }
            }
        }
        
        public FoldoutGroup(string name, bool defaultExpanded)
        {
            Name = name;
            Expanded = EditorPrefs.GetBool(name + "_expanded", defaultExpanded);
        }
    }
    
    // private class Style
    // {
    //     // Customizing the Box (parent group container).
    //     public readonly GUIStyle Box = new GUIStyle(GUI.skin.box)
    //     {
    //         // padding = new RectOffset(1, 1, 1, 1),
    //         // margin = new RectOffset(10, 10, 5, 5),
    //         padding = new RectOffset(20, 0, 5, 5),
    //         border = new RectOffset(2, 2, 2, 2),
    //         normal =
    //         {
    //             background = MakeTex(2, 2, new Color(0.1f, 0.1f, 0.1f, 0.85f))
    //         }
    //     };
    //     
    //     // Customizing the BoxChild (child group container).
    //     public readonly GUIStyle BoxChild = new GUIStyle(GUI.skin.box)
    //     {
    //         // padding = new RectOffset(10, 10, 5, 5),
    //         // margin = new RectOffset(5, 5, 3, 3),
    //         // normal =
    //         // {
    //         //     background = MakeTex(2, 2, new Color(0.15f, 0.15f, 0.15f, 1f))
    //         // }
    //     };
    //     
    //     // Customizing the FoldoutHeader (group header).
    //     public readonly GUIStyle FoldoutHeader = new GUIStyle(EditorStyles.foldout)
    //     {
    //         fontStyle = FontStyle.Bold,
    //         fontSize = 13,
    //         normal =
    //         {
    //             textColor = Color.white
    //         },
    //         // overflow = new RectOffset(-10, 0, 3, 0),
    //         padding = new RectOffset(20, 0, 5, 5),
    //         border = new RectOffset(2, 2, 2, 2)
    //     };
    //     
    //     /// <summary>
    //     /// Creates a texture with a solid color.
    //     /// </summary>
    //     /// <param name="width">The width of the texture.</param>
    //     /// <param name="height">The height of the texture.</param>
    //     /// <param name="col">The color of the texture.</param>
    //     /// <returns>The generated texture.</returns>
    //     private static Texture2D MakeTex(int width, int height, Color col)
    //     {
    //         var pix = new Color[width * height];
    //         for (var i = 0; i < pix.Length; ++i)
    //             pix[i] = col;
    //         
    //         var result = new Texture2D(width, height);
    //         result.SetPixels(pix);
    //         result.Apply();
    //         
    //         return result;
    //     }
    // }
}

/// <summary>
/// Contains the custom styles used by the FoldoutEditor.
/// </summary>
static class StyleFramework
{
    public static readonly GUIStyle Box;
    public static readonly GUIStyle BoxChild;
    public static readonly GUIStyle FoldoutHeader;
    
    static StyleFramework()
    {
        // Customizing the FoldoutHeader (group header).
        FoldoutHeader = new GUIStyle(EditorStyles.foldout)
        {
            fontStyle = FontStyle.Bold,
            fontSize = 13,
            normal =
            {
                textColor = Color.white
            },
            focused =
            {
                textColor = Color.white
            },
            hover =
            {
                textColor = Color.white
            },
            active =
            {
                textColor = Color.white
            },
            onNormal =
            {
                textColor = Color.white
            },
            onFocused =
            {
                textColor = Color.white
            },
            onHover =
            {
                textColor = Color.white
            },
            onActive =
            {
                textColor = Color.white
            },
            // overflow = new RectOffset(-10, 0, 3, 0),
            padding = new RectOffset(20, 0, 5, 5),
            border = new RectOffset(2, 2, 2, 2)
        };

        // Customizing the Box (parent group container).
        Box = new GUIStyle(GUI.skin.box)
        {
            // padding = new RectOffset(1, 1, 1, 1),
            // margin = new RectOffset(10, 10, 5, 5),
            padding = new RectOffset(20, 0, 5, 5),
            border = new RectOffset(2, 2, 2, 2),
            normal =
            {
                background = MakeTex(2, 2, new Color(0.1f, 0.1f, 0.1f, 0.85f))
            }
        };

        // Customizing the BoxChild (child group container).
        BoxChild = new GUIStyle(GUI.skin.box)
        {
            // padding = new RectOffset(10, 10, 5, 5),
            // margin = new RectOffset(5, 5, 3, 3),
            // normal =
            // {
            //     background = MakeTex(2, 2, new Color(0.15f, 0.15f, 0.15f, 1f))
            // }
        };
    }
    
    /// <summary>
    /// Creates a texture with a solid color.
    /// </summary>
    /// <param name="width">The width of the texture.</param>
    /// <param name="height">The height of the texture.</param>
    /// <param name="col">The color of the texture.</param>
    /// <returns>The generated texture.</returns>
    private static Texture2D MakeTex(int width, int height, Color col)
    {
        var pix = new Color[width * height];
        for (var i = 0; i < pix.Length; ++i)
            pix[i] = col;

        var result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        
        return result;
    }
}