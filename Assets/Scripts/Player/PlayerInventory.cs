using Photon.Pun;
using System;
using UnityEngine;

//Called inventory but only holds any get out of jail free cards the player has.
//Maybe in future player can hold more than this type of card so calling this script inventory.

public class PlayerInventory : MonoBehaviourPun
{
    [SerializeField] private int numGetOutOfJailFreeCards;

    public bool HasAGetOutOfJailFreeCard { get { return numGetOutOfJailFreeCards > 0; } }
    public bool HasMaxNumberOfGetOutOfJailFreeCards { get { return numGetOutOfJailFreeCards >= GameDataSlinger.MAX_NUM_GET_OUT_OF_JAIL_FREE_CARDS_PLAYER_INVENTORY; } }
    public int NumGetOutOfJailFreeCards => numGetOutOfJailFreeCards;

    public static event Action PlayerReceivedGetOutOfJailFreeCard;
    public static event Action PlayerUsedGetOutOfJailFreeCard;
    public static event Action PlayerNoLongerOwnsGetOfJailFreeCard;

    public void ReceiveGetOutOfJailFreeCard()
    {
        if (HasMaxNumberOfGetOutOfJailFreeCards)
            return;

        numGetOutOfJailFreeCards++;
        PlayerReceivedGetOutOfJailFreeCard?.Invoke();

        photonView.RPC(nameof(RemoteReceiveGetOutOfJailFreeCard), RpcTarget.Others);
    }

    public void UseGetOutOfJailFreeCard()
    {
        numGetOutOfJailFreeCards--;
        PlayerUsedGetOutOfJailFreeCard?.Invoke();
        if (!HasAGetOutOfJailFreeCard)
            PlayerNoLongerOwnsGetOfJailFreeCard?.Invoke();

        photonView.RPC(nameof(RemoteRemoveGetOutOfJailFreeCard), RpcTarget.Others);
    }

    [PunRPC]
    private void RemoteReceiveGetOutOfJailFreeCard()
    {
        numGetOutOfJailFreeCards++;
    }
    [PunRPC]
    private void RemoteRemoveGetOutOfJailFreeCard()
    {
        numGetOutOfJailFreeCards++;
    }
}