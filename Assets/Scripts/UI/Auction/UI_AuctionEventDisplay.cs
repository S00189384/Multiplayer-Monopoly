using TMPro;
using UnityEngine;
using UnityEngine.UI;

//In an auction panel, there is a section for reporting on auction events such as a player bid, fold, disconnect, win etc.
//This script is for a notification in that section which is just a message with a background colour.

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