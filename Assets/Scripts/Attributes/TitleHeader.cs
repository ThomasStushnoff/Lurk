using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class TitleHeader : PropertyAttribute
{
    public readonly string Header;
    
    public TitleHeader(string header) => Header = header;
}