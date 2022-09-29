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
