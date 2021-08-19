using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviourPunCallbacks
{
    public Text logText;
    public Text roomName;
    private bool _fourPlayersToggle = false;
    private bool _gameWithBotsToggle = false;

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 15;

        Gradients.GenGradients();

        PhotonNetwork.NickName = "Player " + Random.Range(1000, 10000);

        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = "1.2";
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public override void OnConnectedToMaster()
    {
        Log("Connected to Master");
    }

    public void CreateOrJoinRoom()
    {
        if (_gameWithBotsToggle)
        {
            PlayWithBot();
            return;
        }
        string roomNameText = roomName.text;
        if (roomNameText == string.Empty) roomNameText = PhotonNetwork.NickName;
        PhotonNetwork.JoinOrCreateRoom(roomNameText, new RoomOptions {MaxPlayers = 2}, TypedLobby.Default);
        Log(roomNameText);
    }
    
    
    public void FourPlayersToggleValueChange()
    {
        _fourPlayersToggle = !_fourPlayersToggle;
    }

    public void GameWithBotsToggleValueChange()
    {
        _gameWithBotsToggle = !_gameWithBotsToggle;
    }
    public void JoinRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void PlayWithBot()
    {
        //if (_fourPlayersToggle)
        SceneManager.LoadScene("GameWithBots1v1");
    }

    public override void OnJoinedRoom()
    {
        Log("Joined the room");
        PhotonNetwork.LoadLevel(_fourPlayersToggle ? "GameWithPhoton4" : "GameWithPhoton1v1");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Log(message);
    }
    
    public void QuitGame()
    {
        print("Quit");
        Application.Quit();
    }

    private void Log(string message)
    {
        Debug.Log(message);
        logText.text = message;
    }
}