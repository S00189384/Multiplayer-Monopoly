using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_AuctionPlayerTurnIcon : MonoBehaviour
{
    [SerializeField] private Image IMG_PlayerIcon;
    [SerializeField] private Outline outline;

    [SerializeField] private Image IMG_PlayerFolded;

    public void UpdateDisplay(Sprite sprite) => IMG_PlayerIcon.sprite = sprite;

    public void ChangeOutlineColour(Color colour) => outline.effectColor = colour;
    public void ChangeToFolded()
    {
        IMG_PlayerFolded.gameObject.SetActive(true);
        ChangeOutlineColour(Color.red);
    }
}