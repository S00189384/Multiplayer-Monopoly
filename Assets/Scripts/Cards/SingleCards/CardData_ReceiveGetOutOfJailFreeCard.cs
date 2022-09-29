using UnityEngine;

[CreateAssetMenu(fileName = "ReceiveGetOutOfJailFreeCard", menuName = "CardData/ReceiveGetOutOfJailFreeCard")]
public class CardData_ReceiveGetOutOfJailFreeCard : CardData
{
    public override void Execute(string playerID)
    {
        GameManager.Instance.GetPlayerPieceByID(playerID).GetComponent<PlayerInventory>().ReceiveGetOutOfJailFreeCard();
    }
}