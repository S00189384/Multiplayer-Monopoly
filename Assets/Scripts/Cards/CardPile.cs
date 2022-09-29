using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Once order of cards in randomised queue is set up at start - no longer need to sync the queue across clients as 
//UI on each client grabs a card each simultaneuosly.

public class CardPile : MonoBehaviourPun
{
    [SerializeField] private CardCollection myCardCollection;
    private Queue<CardData> cardQueue;

    private void Start()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            System.Random rng = new System.Random();
            int seed = rng.Next(0, 100);
            photonView.RPC(nameof(InitialiseCardQueue), RpcTarget.All, seed);
        }
    }

    [PunRPC]
    private void InitialiseCardQueue(int seed)
    {
        List<CardData> cardDataList = new List<CardData>(myCardCollection.cardDataList);
        Shuffle(cardDataList, seed);
        cardQueue = new Queue<CardData>(cardDataList);
    }

    public CardData GrabCard()
    {
        CardData card = cardQueue.Dequeue();
        cardQueue.Enqueue(card);
        return card;
    }

    public static void Shuffle<T>(IList<T> list, int seed)
    {
        var rng = new System.Random(seed);
        int n = list.Count;

        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}