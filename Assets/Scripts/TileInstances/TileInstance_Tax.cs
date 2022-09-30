using Photon.Pun;
using UnityEngine;

//Tile instance for tax.
//Player that lands has to pay tax amount to the bank.

public class TileInstance_Tax : TileInstance, iTileDataRecievable,iPlayerProcessable
{
    [SerializeField] private TileData_Tax tileDataTax;

    public void ProcessPlayer(string playerID)
    {
        UI_NotificationManager.Instance.RPC_ShowNotification($"{GameManager.Instance.GetPlayerNicknameFromID(playerID)} paid ${tileDataTax.AmountToPay} in tax", RpcTarget.All);
        Bank.Instance.RemoveMoneyFromAccount(playerID, tileDataTax.AmountToPay);
    }

    public void RecieveTileData(TileData tileData)
    {
        tileDataTax = (TileData_Tax)tileData;
    }
}