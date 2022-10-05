using System;

public class BTN_StartAuction : BTN_Base
{
    public static event Action<string,int> AuctionStartedEvent;

    public void RaiseAuctionStartedEvent(string playerID,int photonID)
    {
        AuctionStartedEvent?.Invoke(playerID, photonID);
    }
}