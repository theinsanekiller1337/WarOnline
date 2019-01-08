using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretRotation : Photon.PunBehaviour, IPunObservable
{

    public float MouseRotateSpeed = 2.5f;

    public float KeyRotateSpeed = 8.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    public float min = -30f;
    public float max = 15f;

    void Start()
    {
     
    }

    void Update()
    {
        yaw += MouseRotateSpeed * Input.GetAxis("Mouse X");
   
        if (Input.GetKey("x"))
        {
            yaw += KeyRotateSpeed;
            transform.rotation = Quaternion.Euler(Vector3.up * yaw);
        }
        if (Input.GetKey("z"))
        {
            yaw -= KeyRotateSpeed;
            transform.rotation = Quaternion.Euler(Vector3.up * yaw);
        }

        
    }
    void DoLockMouse()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting) stream.SendNext(yaw);
        else this.yaw = (float)stream.ReceiveNext();
    }

    //this feature is to reveal locked mouse mouse for certain purpose maybe like pause etc... then configure an input of maybe esc then simply call this function
    /*void DoUnlockMouse ()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }*/
}

