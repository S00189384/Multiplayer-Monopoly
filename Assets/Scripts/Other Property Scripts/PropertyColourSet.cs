using UnityEngine;

//Scriptable object for a property colour type.

[CreateAssetMenu(fileName = "PropertyColourSet",menuName = "PropertyColourSet",order = 1)]
public class PropertyColourSet : ScriptableObject
{
    public Color propertyColour;
}