using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Card : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI TMP_CardDescription;

    public void UpdateDisplay(CardData cardData)
    {
        TMP_CardDescription.text = cardData.CardDescription;
    }
}