using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class TitleHeaderAttribute : PropertyAttribute
{
    public readonly string Header;
    
    public TitleHeaderAttribute(string header) => Header = header;
}