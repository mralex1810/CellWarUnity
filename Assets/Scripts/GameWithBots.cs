public class GameWithBots : Game
{
    private AI _bot;

    protected override void Start()
    {
        _bot = new AI();
        gameStarted = true;
        GenField(MakeField.Common());
    }

    protected override void Update()
    {
        var actions = _bot.Process(cells, Tentacles, tick);

        foreach ((int beginCell, int endCell) in actions)
            if (Tentacles[beginCell, endCell])
                DestroyTentacle(beginCell, endCell);
            else
                AddTentacle(beginCell, endCell);
        base.Update();
    }

    public override void CellPressEvent(Cell cellController)
    {
        if (!IsFirstCellPressed && cellController.owner != 1) return;
        base.CellPressEvent(cellController);
    }

    public override void TentaclePressEvent(Tentacle tentacle)
    {
        if (tentacle.startCellController.owner != 1) return;
        base.TentaclePressEvent(tentacle);
    }
}