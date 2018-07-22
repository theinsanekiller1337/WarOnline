using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretRotation : Photon.PunBehaviour, IPunObservable {
    public float turnValue = 2.0f;
    public KeyCode turnLeftKey = KeyCode.Z;
    public KeyCode turnRightKey = KeyCode.X;
    float turn;

    void ProcessTurnInput()
    {
        if (Input.GetKey(turnLeftKey)) turn = -turnValue;
        else if (Input.GetKey(turnRightKey)) turn = turnValue;
        else turn = 0;
    }

    void Update (){

        /*if (photonView.isMine)*/ ProcessTurnInput();
        transform.Rotate(0, turn, 0);

    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting) stream.SendNext(turn);
        else this.turn = (float)stream.ReceiveNext();
    }
}

