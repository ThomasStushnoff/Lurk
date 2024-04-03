using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using World;
using Assembly = System.Reflection.Assembly;

public class LurkHelperEditor : EditorWindow
{
    private List<Type> _typeList;
    private string[] _typeNames;
    private int _selectedTypeIndex;
    private bool _prefabOnly;
    
    [MenuItem("Lurk/Lurk Helper")]
    public static void ShowWindow()
    {
        var window = GetWindow<LurkHelperEditor>();
        window.titleContent = new GUIContent("Lurk Helper");
        window.Show();
        window.InitTypes();
    }
    
    private void InitTypes()
    {
        _typeList = Assembly.GetAssembly(typeof(BaseObject)).GetTypes()
            .Where(t => t.IsClass &&  (t.Namespace == null || 
                                       !t.Namespace.StartsWith("UnityEngine") && 
                                       !t.Namespace.StartsWith("Unity") &&
                                       !t.Namespace.StartsWith("Microsoft") &&
                                       !t.Namespace.StartsWith("JetBrains") &&
                                       !t.Namespace.StartsWith("System")))
            .ToList();
        
        _typeList.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));
        _typeNames = _typeList.Select(t => t.Name).ToArray();
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
        
        GUILayout.Label("Lurk Helper", headerStyle);
        GUILayout.Space(10.0f);
        
        GUILayout.BeginVertical(new Style().Box);

        GUILayout.Label("Add Component To All Light Objects");
        _selectedTypeIndex = EditorGUILayout.Popup("Select Type", _selectedTypeIndex, _typeNames);
        _prefabOnly = EditorGUILayout.Toggle("Prefab Only", _prefabOnly);
        if (GUILayout.Button("Add Component"))
            AddComponentHelper(_typeList[_selectedTypeIndex]);
        
        GUILayout.EndVertical();
    }

    private void AddComponentHelper(Type selectedType)
    {
        if (_prefabOnly)
        {
            var objs = FindObjectsOfType<Light>(true);
            foreach (var obj in objs)
            {
                if (obj.GetComponent(selectedType) == null && PrefabUtility.IsPartOfPrefabInstance(obj))
                {
                    obj.gameObject.AddComponent(selectedType);
                    Debug.Log($"Added {selectedType.Name} to {obj.name}");
                    
                    PrefabUtility.ApplyPrefabInstance(obj.gameObject, InteractionMode.AutomatedAction);
                }
            }
        }
    }

    private class Style
    {
        public readonly GUIStyle Box = new GUIStyle(GUI.skin.box)
        {
            padding = new RectOffset(0, 0, 5, 5),
            border = new RectOffset(2, 2, 2, 2),
            normal =
            {
                background = MakeTex(2, 2, new Color(0.1f, 0.1f, 0.1f, 0.85f))
            }
        };
        
        public readonly GUIStyle BoxChild = new GUIStyle(GUI.skin.box)
        {
            padding = new RectOffset(10, 10, 5, 5),
            margin = new RectOffset(5, 5, 3, 3),
            normal =
            {
                background = MakeTex(2, 2, new Color(0.15f, 0.15f, 0.15f, 1f))
            }
        };
        
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
}