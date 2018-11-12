using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGunRotation : Photon.PunBehaviour, IPunObservable
{
    private GameObject enemy;
    
    private bool lookingEnemy = false;

    public float mouseRotateSpeed = 2.5f;

    public float keyRotateSpeed = 8.0f;

    public float autoAimSpeed = 0.0f;

    private float rotationX = 0.0f;
    private float rotationY = 0.0f;

    private float pitch = 0.0f;

    public float min = -30f;
    public float max = 15f;

    void Start()
    {     
    }

    void Update()
    {
       // rotationY += MouseRotateSpeed * Input.GetAxis("Mouse X");
      //  pitch -= MouseRotateSpeed * Input.GetAxis("Mouse Y");
        if (Input.GetKey("x"))
        {
            rotationY += keyRotateSpeed;
        }
        if (Input.GetKey("z"))
        {
            rotationY -= keyRotateSpeed;
        }

        pitch = Mathf.Clamp(pitch, min, max);

        if (Input.GetKey(KeyCode.Space) && lookingEnemy == true)
        {
            var targetRotation = Quaternion.LookRotation(enemy.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, autoAimSpeed * Time.deltaTime);

            rotationY = targetRotation.eulerAngles.y;
            rotationX = targetRotation.eulerAngles.x;
        }
        else
        {
            transform.eulerAngles = new Vector3(rotationX, rotationY, 0);
        }


    }

    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            enemy = col.gameObject;
            lookingEnemy = true;
            
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            lookingEnemy = false;
        }
    }

    void DoLockMouse()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting) stream.SendNext(rotationY);
        else this.rotationY = (float)stream.ReceiveNext();
    }

    

    //this feature is to reveal locked mouse mouse for certain purpose maybe like pause etc... then configure an input of maybe esc then simply call this function
    /*void DoUnlockMouse ()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }*/
}

