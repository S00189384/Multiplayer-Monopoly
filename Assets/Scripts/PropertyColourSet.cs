using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PropertyColourSet",menuName = "PropertyColourSet",order = 1)]
public class PropertyColourSet : ScriptableObject
{
    public Color propertyColour;
}

public enum PropertyColour
{
    Brown,
    None
}