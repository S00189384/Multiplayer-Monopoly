using UnityEngine;
using UnityEngine.UI;

//Icon which represents a player in an auction.
//When its the players turn this icons outline turns to green.
//When the player folds the icon turns to red.

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