using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Random = UnityEngine.Random;

public class PvpGameFourPlayers : GameWithPhoton
{
    private bool _isFieldCreated;
    private int _seed;

    protected override void Start()
    {
        player.color = ColorOfOwner(PhotonNetwork.LocalPlayer.ActorNumber);

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            Random.InitState(DateTime.Now.Millisecond + DateTime.UtcNow.Minute * DateTime.Now.Second);
            _seed = Random.Range(int.MinValue, int.MaxValue);
            GenField(MakeField.ForFourPlayers(_seed));
        }
    }

    protected override void Update()
    {
        if (!gameStarted && PhotonNetwork.LocalPlayer.IsMasterClient) SendEventToSyncSeed();
        base.Update();
    }

    private void SendEventToSyncSeed()
    {
        var options = new RaiseEventOptions {Receivers = ReceiverGroup.Others};
        var sendOptions = new SendOptions {Reliability = true};
        PhotonNetwork.RaiseEvent(50, _seed, options, sendOptions);
    }

    public override void OnEvent(EventData photonEvent)
    {
        base.OnEvent(photonEvent);
        Debug.Log(photonEvent.Code);
        if (photonEvent.Code == 50)
        {
            if (_isFieldCreated) return;
            _seed = (int) photonEvent.CustomData;
            GenField(MakeField.ForFourPlayers(_seed));
            _isFieldCreated = true;
        }
    }
}