//Tile instance for the start tile.
//Listens for a player move passed go event and adds money to the players account.

public class TileInstance_Start : TileInstance, iPlayerProcessable
{
    private void Awake()
    {
        PlayerMove.MovePassedGoEvent += OnPlayerPassedGo;
    }

    private void OnPlayerPassedGo(PlayerMove playerMove)
    {
        Bank.Instance.AddMoneyToAccount(playerMove.PlayerID, GameDataSlinger.PASS_GO_MONEY_RECIEVED);
    }

    public void ProcessPlayer(string playerID) { }

    private void OnDestroy()
    {
        PlayerMove.MovePassedGoEvent -= OnPlayerPassedGo;
    }
}
