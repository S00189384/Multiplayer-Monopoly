using System.Collections.Generic;
using UnityEngine;

//Scriptable object for a collection of cards.
//Two are in this project - chance and community chest.

[CreateAssetMenu(fileName = "CardCollection", menuName = "CardData/CardCollection")]
public class CardCollection : ScriptableObject
{
    public List<CardData> cardDataList = new List<CardData>();
}
