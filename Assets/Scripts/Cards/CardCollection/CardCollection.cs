using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardCollection", menuName = "CardData/CardCollection")]
public class CardCollection : ScriptableObject
{
    public List<CardData> cardDataList = new List<CardData>();
}
