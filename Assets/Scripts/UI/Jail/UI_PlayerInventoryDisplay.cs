using System.Collections.Generic;
using UnityEngine;

//Shows player how many get out of jail free cards they own.
//Spawns a card display when a player receives a get out of jail free card.

public class UI_PlayerInventoryDisplay : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject jailCardPrefab;
    [SerializeField] private GameObject GO_noCardsDisplay;
    [SerializeField] private Transform T_HorizontalLayout;

    private List<GameObject> spawnedJailCardsList = new List<GameObject>();

    private void Awake()
    {
        PlayerInventory.PlayerReceivedGetOutOfJailFreeCard += OnPlayerReceivedGetOutOfJailFreeCard;
        PlayerInventory.PlayerNoLongerOwnsGetOfJailFreeCard += OnPlayerNoLongerOwnsGetOutOfJailFreeCard;
        PlayerInventory.PlayerUsedGetOutOfJailFreeCard += OnPlayerUsedGetOutOfJailFreeCard;
    }

    private void OnPlayerUsedGetOutOfJailFreeCard()
    {
        Destroy(spawnedJailCardsList[spawnedJailCardsList.Count - 1]);
        spawnedJailCardsList.RemoveAt(spawnedJailCardsList.Count - 1);
    }

    private void OnPlayerNoLongerOwnsGetOutOfJailFreeCard()
    {
        GO_noCardsDisplay.SetActive(true);
    }


    private void OnPlayerReceivedGetOutOfJailFreeCard()
    {
        GameObject spawnedJailCard = Instantiate(jailCardPrefab, T_HorizontalLayout);
        spawnedJailCardsList.Add(spawnedJailCard);

        GO_noCardsDisplay.SetActive(false);
    }

    private void OnDestroy()
    {
        PlayerInventory.PlayerReceivedGetOutOfJailFreeCard -= OnPlayerReceivedGetOutOfJailFreeCard;
        PlayerInventory.PlayerNoLongerOwnsGetOfJailFreeCard -= OnPlayerNoLongerOwnsGetOutOfJailFreeCard;
        PlayerInventory.PlayerUsedGetOutOfJailFreeCard -= OnPlayerUsedGetOutOfJailFreeCard;
    }
}