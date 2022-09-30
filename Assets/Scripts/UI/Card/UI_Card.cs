using TMPro;
using UnityEngine;

//Script attached to a chance / community chest card to be shown to the player.

public class UI_Card : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI TMP_CardDescription;

    public void UpdateDisplay(CardData cardData)
    {
        TMP_CardDescription.text = cardData.CardDescription;
    }
}