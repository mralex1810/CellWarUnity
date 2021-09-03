using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameWithPhoton : Game, IOnEventCallback
{
    protected override void Start()
    {
        player.color = ColorOfOwner(PhotonNetwork.LocalPlayer.ActorNumber);

        GenField(MakeField.Common());
    }

    protected override void Update()
    {
        if (!gameStarted && PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount >= 2)
            {
                if (tick >= 15 * 10) SendEventToStart();
            }
            else
            {
                tick = 0;
            }
        }

        base.Update();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
        if (gameStarted && PhotonNetwork.IsMasterClient)
        {
            //SendEventToSyncTentacles();
            SendEventToSyncOwners();
            SendEventToSyncCells();
        }
    }

    public virtual void OnEvent(EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            case 0:
                gameStarted = true;
                tick = 0;
                break;
            case 1:
                DoActionDestroy((byte[]) photonEvent.CustomData);
                break;
            case 2:
                DoActionAdd((byte[]) photonEvent.CustomData);
                break;
            case 100:
                SyncCells((int[]) photonEvent.CustomData);
                break;
            case 101:
                SyncTentacles((bool[,]) photonEvent.CustomData);
                break;
            case 102:
                SyncOwners((byte[]) photonEvent.CustomData);
                break;
        }
    }

    protected override void AddTentacle(int idBegin, int idEnd)
    {
        if (cellsController[idBegin].owner == PhotonNetwork.LocalPlayer.ActorNumber)
            SendEventToDoAction(idBegin, idEnd);
        base.AddTentacle(idBegin, idEnd);
    }

    public override void StartDestroyTentacle(int idBegin, int idEnd)
    {
        if (cellsController[idBegin].owner == PhotonNetwork.LocalPlayer.ActorNumber)
            SendEventToDoAction(idBegin, idEnd);
        base.StartDestroyTentacle(idBegin, idEnd);
    }

    public override void CellPressEvent(Cell cellController)
    {
        if (!IsFirstCellPressed && cellController.owner != PhotonNetwork.LocalPlayer.ActorNumber) return;
        base.CellPressEvent(cellController);
    }

    public override void TentaclePressEvent(Tentacle tentacle)
    {
        if (tentacle.startCellController.owner != PhotonNetwork.LocalPlayer.ActorNumber) return;
        base.TentaclePressEvent(tentacle);
    }

    public override void QuitGame()
    {
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
        var owners = new byte[cells.Length];
        for (var i = 0; i < cells.Length; i++) owners[i] = (byte) cellsController[i].owner;
        PhotonNetwork.RaiseEvent(102, owners, options, sendOptions);
    }

    private void SendEventToSyncCells()
    {
        var options = new RaiseEventOptions {Receivers = ReceiverGroup.Others};
        var sendOptions = new SendOptions {Reliability = true};
        var cellsScore = new int[45];
        for (var i = 0; i < cells.Length; i++) cellsScore[i] = cellsController[i].score;
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
        PhotonNetwork.RaiseEvent(Tentacles[idBegin, idEnd] ? (byte) 1 : (byte) 2, new[] {(byte) idBegin, (byte) idEnd},
            options, sendOptions);
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
        if (!Tentacles[action[0], action[1]]) AddTentacle(action[0], action[1]);
    }

    private void DoActionDestroy(byte[] action)
    {
        if (Tentacles[action[0], action[1]]) StartDestroyTentacle(action[0], action[1]);
    }

    private void SyncCells(int[] cellsScore)
    {
        for (var i = 0; i < 45; i++)
        {
            cellsController[i].score = cellsScore[i];
            cellsController[i].NewLvl();
        }
    }

    private void SyncTentacles(bool[,] data)
    {
        for (var i = 0; i < cells.Length; i++)
        for (var j = 0; j < cells.Length; j++)
            if (Tentacles[i, j] && !data[i, j])
            {
                cellsController[i].DestroyTentacle(j);
                Tentacles[i, j] = false;
            }
            else if (!Tentacles[i, j] && data[i, j])
            {
                cellsController[i].AddTentacle(cells[j], Tentacles[j, i]);
                Tentacles[i, j] = true;
            }
    }

    private void SyncOwners(byte[] owners)
    {
        for (var i = 0; i < cells.Length; i++)
            if (cellsController[i].owner != owners[i])
            {
                cellsController[i].owner = owners[i];
                cellsController[i].CheckOwner();
            }
    }
}