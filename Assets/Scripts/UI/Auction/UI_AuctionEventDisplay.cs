using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_AuctionEventDisplay : MonoBehaviour
{
    private Image IMG_background;
    private TextMeshProUGUI TMP_EventDescription;

    private void Awake()
    {
        IMG_background = GetComponent<Image>();
        TMP_EventDescription = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void UpdateDisplay(Color backgroundColour,string eventDescription)
    {
        IMG_background.color = backgroundColour;
        TMP_EventDescription.text = eventDescription;
    }
}
