using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class CardData : ScriptableObject
{
    public string CardDescription;
    public abstract void Execute(string playerID);

}