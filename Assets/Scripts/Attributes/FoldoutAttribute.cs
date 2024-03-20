using UnityEngine;

/// <summary>
/// Groups serialized fields in the inspector under a foldout.
/// </summary>
public class FoldoutAttribute : PropertyAttribute
{
    public readonly string Name;
    public readonly bool StartOpen;
    public readonly bool Stylized;
    public readonly bool ReadOnly;

    public FoldoutAttribute(string name, bool startOpen = true, bool stylized = true, bool readOnly = false)
    {
        Name = name;
        StartOpen = startOpen;
        Stylized = stylized;
        ReadOnly = readOnly;
    }
}