using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Random = UnityEngine.Random;

public class PvpGameFourPlayers : GameWithPhoton
{
    private int _seed;
    private bool _isFieldCreated = false;
    
    protected override void Start()
    {
        player.color = ColorOfOwner(PhotonNetwork.LocalPlayer.ActorNumber);

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            Random.InitState(DateTime.Now.Millisecond + DateTime.UtcNow.Minute * DateTime.Now.Second);
            _seed = Random.Range(Int32.MinValue, Int32.MaxValue);
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
            _seed = (int)photonEvent.CustomData;
            GenField(MakeField.ForFourPlayers(_seed));
            _isFieldCreated = true;
        }
    }
}
