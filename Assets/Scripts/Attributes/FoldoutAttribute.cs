using UnityEngine;

/// <summary>
/// Groups serialized fields in the inspector under a foldout.
/// </summary>
public class FoldoutAttribute : PropertyAttribute
{
    public readonly string Name;
    public readonly bool FoldEverything;
    
    public FoldoutAttribute(string name, bool startOpen = true )
    {
        Name = name;
        FoldEverything = startOpen;
    }
}