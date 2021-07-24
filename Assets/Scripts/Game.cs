using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviourPunCallbacks
{
    public GameObject cellPrefab;
    public bool gameStarted;
    public Text player;
    public Text greenScore;
    public Text redScore;
    public GameObject[] cells;
    public int tick;
    public Cell[] cellsController;
    private Cell _firstCell;
    protected bool IsFirstCellPressed;
    protected bool[,] Tentacles;

    protected virtual void Update()
    {
        if (gameStarted)
        {
            var greenScoreInt = 0;
            var redScoreInt = 0;
            foreach (Cell cell in cellsController)
                if (cell.owner == 1)
                    greenScoreInt += cell.score;
                else if (cell.owner == 2) redScoreInt += cell.score;
            greenScore.text = greenScoreInt.ToString();
            redScore.text = redScoreInt.ToString();
        }

        tick++;
    }

    protected virtual void LateUpdate()
    {
    }

    public virtual void QuitGame()
    {
        Cell.CellCounter = 0;
        SceneManager.LoadScene("Menu");
    }

    protected void GenField(FieldProperties ans)
    {
        cells = new GameObject[ans.pos.Length];
        for (var index = 0; index < ans.pos.Length; index++)
            cells[index] = Instantiate(cellPrefab, ans.pos[index], Quaternion.identity);

        Tentacles = new bool[cells.Length, cells.Length];
        cellsController = new Cell[cells.Length];
        for (var i = 0; i < cells.Length; i++) cellsController[i] = cells[i].GetComponent<Cell>();

        for (var i = 0; i < ans.lvl.Length; i++)
        for (var j = 0; j < ans.lvl[i].Length; j++)
            cellsController[ans.lvl[i][j]].lvl = i + 1;

        cellsController[ans.greenCell].owner = 1;
        cellsController[ans.redCell].owner = 2;
        foreach (GameObject cell in cells) cell.GetComponent<Cell>().game = this;
    }

    protected virtual void AddTentacle(int idBegin, int idEnd)
    {
        var scoreTentacle =
            (int) (Vector3.Distance(cells[idBegin].transform.position, cells[idEnd].transform.position) * 10);
        if (Tentacles[idBegin, idEnd]) return;
        if (scoreTentacle <= cellsController[idBegin].score
            && cellsController[idBegin].tentaclesCount < cellsController[idBegin].tentaclesMax
            && !Tentacles[idEnd, idBegin])
        {
            cellsController[idBegin].AddTentacle(cells[idEnd], false);
            Tentacles[idBegin, idEnd] = true;
        }
        else if (scoreTentacle / 2 <= cellsController[idBegin].score
                 && cellsController[idBegin].tentaclesCount < cellsController[idBegin].tentaclesMax
                 && Tentacles[idEnd, idBegin])
        {
            Tentacle oppositeTentacle = cellsController[idEnd].FindTentacleByEndId(idBegin);
            cellsController[idBegin].AddTentacle(cells[idEnd], true, oppositeTentacle);
            cellsController[idEnd].score += oppositeTentacle.score / 2;
            oppositeTentacle.DoBilateral();
            Tentacles[idBegin, idEnd] = true;
        }
    }

    public virtual void DestroyTentacle(int idBegin, int idEnd)
    {
        cellsController[idBegin].DestroyTentacle(idEnd);
        if (Tentacles[idEnd, idBegin])
        {
            Tentacle oppositeTentacle = cellsController[idEnd].FindTentacleByEndId(idBegin);
            if (cellsController[idEnd].score < oppositeTentacle.score / 2) DestroyTentacle(idEnd, idBegin);
            cellsController[idEnd].score -= oppositeTentacle.score / 2;
            oppositeTentacle.oppositeTentacle = null;
            oppositeTentacle.DoUniliteral();
        }

        Tentacles[idBegin, idEnd] = false;
    }

    public virtual void CellPressEvent(Cell cellController)
    {
        if (!IsFirstCellPressed)
        {
            IsFirstCellPressed = true;
            _firstCell = cellController;
            cellController.CircleActive(true);
        }
        else
        {
            if (_firstCell.id == cellController.id)
            {
                _firstCell.CircleActive(false);
                _firstCell = null;
                IsFirstCellPressed = false;
                return;
            }

            AddTentacle(_firstCell.id, cellController.id);
            _firstCell.CircleActive(false);
            _firstCell = null;
            IsFirstCellPressed = false;
        }
    }

    public virtual void TentaclePressEvent(Tentacle tentacle)
    {
        DestroyTentacle(tentacle.startCellController.id, tentacle.endCellController.id);
    }

    public virtual int CellOverEvent(Vector3 pos)
    {
        if (!IsFirstCellPressed) return 0;
        var scoreTentacle = (int) (Vector3.Distance(pos, _firstCell.transform.position) * 10);
        if (scoreTentacle > _firstCell.GetComponent<Cell>().score) return -scoreTentacle;
        return scoreTentacle;
    }

    public static Color ColorOfOwner(int owner)
    {
        Color result = owner switch
        {
            0 => Color.gray,
            1 => Color.green,
            2 => Color.red,
            _ => Color.blue
        };
        return result;
    }
}