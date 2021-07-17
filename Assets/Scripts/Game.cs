using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public GameObject cellPrefab;

    //private AI bot;  TODO : make game with bot/bots
    public bool gameStarted;
    public Text player;
    public Text greenScore;
    public Text redScore;
    private readonly Cell[] _cellsController = new Cell[45];
    private GameObject[] _cells;
    private GameObject _firstCell;
    private bool _isFirstCellPressed;
    private bool[,] _tentacles;
    private int _tick;

    private void Start()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
        {
            player.text = "Green";
            player.color = Color.green;
        }
        else
        {
            player.text = "Red";
            player.color = Color.red;
        }

        GenCells();
        for (var i = 0; i < 45; i++) _cellsController[i] = _cells[i].GetComponent<Cell>();
        _tick = 0;
        //bot = new AI();
        _isFirstCellPressed = false;
        _tentacles = new bool[_cells.Length, _cells.Length];
        Debug.Log(PhotonNetwork.LocalPlayer.ActorNumber);
    }

    private void Update()
    {
        //List<(int beginCell, int endCell)> actions = bot.Process(cells, tentacles, tick);
        //
        //foreach (var action in actions)
        //{
        //    if (tentacles[action.beginCell, action.endCell])
        //    {
        //        DestroyTentacle(action.beginCell, action.endCell);
        //    }
        //    else
        //    {
        //        AddTentacle(action.beginCell, action.endCell);
        //    }
        //}
        //tick++;
        //if (tick % 1000 == 0)
        //{
        //    print(tick);
        //}
        if (!gameStarted && PhotonNetwork.IsMasterClient)
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
                SendEventToStart();
        if (gameStarted)
        {
            var greenScoreInt = 0;
            var redScoreInt = 0;
            foreach (Cell cell in _cellsController)
                if (cell.owner == 1)
                    greenScoreInt += cell.score;
                else if (cell.owner == 2) redScoreInt += cell.score;
            greenScore.text = greenScoreInt.ToString();
            redScore.text = redScoreInt.ToString();
        }

        _tick++;
    }

    private void LateUpdate()
    {
        if (_tick % 15 == 1 && PhotonNetwork.IsMasterClient)
        {
            //SendEventToSyncTentacles();
            SendEventToSyncOwners();
            SendEventToSyncCells();
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == 100)
        {
            SyncCells((int[]) photonEvent.CustomData);
            return;
        }

        if (photonEvent.Code == 101)
        {
            SyncTentacles((bool[,]) photonEvent.CustomData);
            return;
        }

        if (photonEvent.Code == 102)
        {
            SyncOwners((byte[]) photonEvent.CustomData);
            return;
        }

        var data = (byte[]) photonEvent.CustomData;
        switch (photonEvent.Code)
        {
            case 0:
                gameStarted = true;
                _tick = 0;
                break;
            case 1:
                DoActionDestroy(data);
                break;
            case 2:
                DoActionAdd(data);
                break;
            case 3:
                DoActionDestroy(data);
                SendEventToDoAction(data[0], data[1]);
                break;
            case 4:
                DoActionAdd(data);
                SendEventToDoAction(data[0], data[1]);
                break;
        }
    }


    private void GenCells()
    {
        _cells = new[]
        {
            Instantiate(cellPrefab, new Vector3(1.00f, 1.50f), Quaternion.identity),
            Instantiate(cellPrefab, new Vector3(1.00f, 3.50f), Quaternion.identity),
            Instantiate(cellPrefab, new Vector3(1.00f, 5.00f), Quaternion.identity),
            Instantiate(cellPrefab, new Vector3(1.00f, 6.50f), Quaternion.identity),
            Instantiate(cellPrefab, new Vector3(1.00f, 8.50f), Quaternion.identity),
            Instantiate(cellPrefab, new Vector3(3.00f, 1.00f), Quaternion.identity),
            Instantiate(cellPrefab, new Vector3(3.00f, 3.00f), Quaternion.identity),
            Instantiate(cellPrefab, new Vector3(3.00f, 5.00f), Quaternion.identity),
            Instantiate(cellPrefab, new Vector3(3.00f, 7.00f), Quaternion.identity),
            Instantiate(cellPrefab, new Vector3(3.00f, 9.00f), Quaternion.identity),
            Instantiate(cellPrefab, new Vector3(5.00f, 1.50f), Quaternion.identity),
            Instantiate(cellPrefab, new Vector3(5.00f, 3.50f), Quaternion.identity),
            Instantiate(cellPrefab, new Vector3(5.00f, 5.00f), Quaternion.identity),
            Instantiate(cellPrefab, new Vector3(5.00f, 6.50f), Quaternion.identity),
            Instantiate(cellPrefab, new Vector3(5.00f, 8.50f), Quaternion.identity),
            Instantiate(cellPrefab, new Vector3(7.00f, 1.00f), Quaternion.identity),
            Instantiate(cellPrefab, new Vector3(7.00f, 3.00f), Quaternion.identity),
            Instantiate(cellPrefab, new Vector3(7.00f, 5.00f), Quaternion.identity),
            Instantiate(cellPrefab, new Vector3(7.00f, 7.00f), Quaternion.identity),
            Instantiate(cellPrefab, new Vector3(7.00f, 9.00f), Quaternion.identity),
            Instantiate(cellPrefab, new Vector3(9.50f, 0.50f), Quaternion.identity),
            Instantiate(cellPrefab, new Vector3(9.50f, 2.50f), Quaternion.identity),
            Instantiate(cellPrefab, new Vector3(9.50f, 5.00f), Quaternion.identity),
            Instantiate(cellPrefab, new Vector3(9.50f, 7.50f), Quaternion.identity),
            Instantiate(cellPrefab, new Vector3(9.50f, 9.50f), Quaternion.identity),
            Instantiate(cellPrefab, new Vector3(12.00f, 1.00f), Quaternion.identity),
            Instantiate(cellPrefab, new Vector3(12.00f, 3.00f), Quaternion.identity),
            Instantiate(cellPrefab, new Vector3(12.00f, 5.00f), Quaternion.identity),
            Instantiate(cellPrefab, new Vector3(12.00f, 7.00f), Quaternion.identity),
            Instantiate(cellPrefab, new Vector3(12.00f, 9.00f), Quaternion.identity),
            Instantiate(cellPrefab, new Vector3(14.00f, 1.50f), Quaternion.identity),
            Instantiate(cellPrefab, new Vector3(14.00f, 3.50f), Quaternion.identity),
            Instantiate(cellPrefab, new Vector3(14.00f, 5.00f), Quaternion.identity),
            Instantiate(cellPrefab, new Vector3(14.00f, 6.50f), Quaternion.identity),
            Instantiate(cellPrefab, new Vector3(14.00f, 8.50f), Quaternion.identity),
            Instantiate(cellPrefab, new Vector3(16.00f, 1.00f), Quaternion.identity),
            Instantiate(cellPrefab, new Vector3(16.00f, 3.00f), Quaternion.identity),
            Instantiate(cellPrefab, new Vector3(16.00f, 5.00f), Quaternion.identity),
            Instantiate(cellPrefab, new Vector3(16.00f, 7.00f), Quaternion.identity),
            Instantiate(cellPrefab, new Vector3(16.00f, 9.00f), Quaternion.identity),
            Instantiate(cellPrefab, new Vector3(18.00f, 1.50f), Quaternion.identity),
            Instantiate(cellPrefab, new Vector3(18.00f, 3.50f), Quaternion.identity),
            Instantiate(cellPrefab, new Vector3(18.00f, 5.00f), Quaternion.identity),
            Instantiate(cellPrefab, new Vector3(18.00f, 6.50f), Quaternion.identity),
            Instantiate(cellPrefab, new Vector3(18.00f, 8.50f), Quaternion.identity)
        };
        int[] lvl1 = {0, 1, 3, 4};
        int[] lvl2 = {2, 6, 7, 8};
        int[] lvl3 = {5, 9, 11, 12, 13};
        int[] lvl4 = {10, 14, 16, 17, 18};
        int[] lvl5 = {15, 19, 21};
        int[] lvl6 = {20, 22};
        foreach (int id in lvl1)
        {
            _cells[id].GetComponent<Cell>().lvl = 1;
            _cells[44 - id].GetComponent<Cell>().lvl = 1;
        }

        foreach (int id in lvl2)
        {
            _cells[id].GetComponent<Cell>().lvl = 2;
            _cells[44 - id].GetComponent<Cell>().lvl = 2;
        }

        foreach (int id in lvl3)
        {
            _cells[id].GetComponent<Cell>().lvl = 3;
            _cells[44 - id].GetComponent<Cell>().lvl = 3;
        }

        foreach (int id in lvl4)
        {
            _cells[id].GetComponent<Cell>().lvl = 4;
            _cells[44 - id].GetComponent<Cell>().lvl = 4;
        }

        foreach (int id in lvl5)
        {
            _cells[id].GetComponent<Cell>().lvl = 5;
            _cells[44 - id].GetComponent<Cell>().lvl = 5;
        }

        foreach (int id in lvl6)
        {
            _cells[id].GetComponent<Cell>().lvl = 6;
            _cells[44 - id].GetComponent<Cell>().lvl = 6;
        }

        _cells[2].GetComponent<Cell>().owner = 1;
        _cells[42].GetComponent<Cell>().owner = 2;
        foreach (GameObject cell in _cells) cell.GetComponent<Cell>().game = this;
    }

    public void QuitGame()
    {
        print("Quit");
        PhotonNetwork.LeaveRoom();
        Cell.CellCounter = 0;
        PhotonNetwork.LoadLevel("Menu");
    }

/*
    private void SendEventToSyncTentacles()
    {
        RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(101, _tentacles, options, sendOptions);
    }
*/

    private void SendEventToSyncOwners()
    {
        var options = new RaiseEventOptions {Receivers = ReceiverGroup.Others};
        var sendOptions = new SendOptions {Reliability = true};
        var owners = new byte[_cells.Length];
        for (var i = 0; i < _cells.Length; i++) owners[i] = (byte) _cellsController[i].owner;
        PhotonNetwork.RaiseEvent(102, owners, options, sendOptions);
    }

    private void SendEventToSyncCells()
    {
        var options = new RaiseEventOptions {Receivers = ReceiverGroup.Others};
        var sendOptions = new SendOptions {Reliability = true};
        var cellsScore = new int[45];
        for (var i = 0; i < _cells.Length; i++) cellsScore[i] = _cellsController[i].score;
        PhotonNetwork.RaiseEvent(100, cellsScore, options, sendOptions);
    }

    private static void SendEventToStart()
    {
        var options = new RaiseEventOptions {Receivers = ReceiverGroup.All};
        var sendOptions = new SendOptions {Reliability = true};
        PhotonNetwork.RaiseEvent(0, new byte[0], options, sendOptions);
    }

    private void SendEventToDoAction(int idBegin, int idEnd)
    {
        var options = new RaiseEventOptions {Receivers = ReceiverGroup.Others};
        var sendOptions = new SendOptions {Reliability = true};
        PhotonNetwork.RaiseEvent(_tentacles[idBegin, idEnd] ? (byte) 1 : (byte) 2, new[] {(byte) idBegin, (byte) idEnd},
            options, sendOptions);
    }

    private void AddTentacle(int idBegin, int idEnd)
    {
        if (_cellsController[idBegin].owner == PhotonNetwork.LocalPlayer.ActorNumber)
            SendEventToDoAction(idBegin, idEnd);
        var scoreTentacle =
            (int) (Vector3.Distance(_cells[idBegin].transform.position, _cells[idEnd].transform.position) * 10);
        if (scoreTentacle <= _cellsController[idBegin].score
            && _cellsController[idBegin].tentaclesCount < _cellsController[idBegin].tentaclesMax
            && !_tentacles[idBegin, idEnd] && !_tentacles[idEnd, idBegin])
        {
            _cellsController[idBegin].AddTentacle(_cells[idEnd]);
            _tentacles[idBegin, idEnd] = true;
        }
    }

    public void DestroyTentacle(int idBegin, int idEnd)
    {
        if (_cellsController[idBegin].owner == PhotonNetwork.LocalPlayer.ActorNumber)
            SendEventToDoAction(idBegin, idEnd);
        _cellsController[idBegin].DestroyTentacle(idEnd);
        _tentacles[idBegin, idEnd] = false;
    }

    public void CellPressEvent(GameObject cell)
    {
        if (!_isFirstCellPressed)
        {
            if (cell.GetComponent<Cell>().owner != PhotonNetwork.LocalPlayer.ActorNumber) return;
            _isFirstCellPressed = true;
            _firstCell = cell;
            cell.GetComponent<Cell>().CircleActive(true);
        }
        else
        {
            if (_firstCell.GetComponent<Cell>().id == cell.GetComponent<Cell>().id)
            {
                _firstCell.GetComponent<Cell>().CircleActive(false);
                _firstCell = null;
                _isFirstCellPressed = false;
                return;
            }

            AddTentacle(_firstCell.GetComponent<Cell>().id, cell.GetComponent<Cell>().id);
            _firstCell.GetComponent<Cell>().CircleActive(false);
            _firstCell = null;
            _isFirstCellPressed = false;
        }
    }

    public int CellOverEvent(Vector3 pos)
    {
        if (!_isFirstCellPressed) return 0;
        var scoreTentacle = (int) (Vector3.Distance(pos, _firstCell.transform.position) * 10);
        if (scoreTentacle > _firstCell.GetComponent<Cell>().score) return -scoreTentacle;
        return scoreTentacle;
    }

    public override void OnLeftLobby()
    {
        SceneManager.LoadScene(0);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.LogFormat("Player {0} entered room", newPlayer.ActorNumber);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.LogFormat("Player {0} left room", otherPlayer.ActorNumber);
    }

/*
    private void DoAction(byte[] action)
    {
        if (_tentacles[action[0], action[1]])
        {
            DoActionDestroy(action);
        }
        else
        {
            DoActionAdd(action);
        }
    }
*/
    private void DoActionAdd(byte[] action)
    {
        if (!_tentacles[action[0], action[1]]) AddTentacle(action[0], action[1]);
    }

    private void DoActionDestroy(byte[] action)
    {
        if (_tentacles[action[0], action[1]]) DestroyTentacle(action[0], action[1]);
    }

    private void SyncCells(int[] cellsScore)
    {
        for (var i = 0; i < 45; i++)
        {
            _cellsController[i].score = cellsScore[i];
            _cellsController[i].NewLvl();
        }
    }

    private void SyncTentacles(bool[,] data)
    {
        for (var i = 0; i < _cells.Length; i++)
        for (var j = 0; j < _cells.Length; j++)
            if (_tentacles[i, j] && !data[i, j])
            {
                _cellsController[i].DestroyTentacle(j);
                _tentacles[i, j] = false;
            }
            else if (!_tentacles[i, j] && data[i, j])
            {
                _cellsController[i].AddTentacle(_cells[j]);
                _tentacles[i, j] = true;
            }
    }

    private void SyncOwners(byte[] owners)
    {
        for (var i = 0; i < _cells.Length; i++)
            if (_cellsController[i].owner != owners[i])
            {
                _cellsController[i].owner = owners[i];
                _cellsController[i].CheckOwner();
            }
    }
}