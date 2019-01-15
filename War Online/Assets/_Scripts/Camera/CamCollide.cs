using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamCollide : MonoBehaviour
{
    public GameObject CamFirstPlace;
    public GameObject TargetF;
    public Transform Player;
    public float smooth=5;
    public bool Collided=false;
    public float CamForwardSpeed=3;
    public float CamBackwardSpeed=10;
    //all the value is well set up!
    private float BackSpeed;

    void CamMove ()
    {
        transform.Translate (Vector3.forward * CamForwardSpeed * Time.deltaTime, Space.Self);
        Collided = false;
        BackSpeed = 0;
    }

    void CamGoBack ()
    {
        transform.position = Vector3.Lerp (transform.position, CamFirstPlace.transform.position,Time.deltaTime*BackSpeed);
        BackSpeed = CamBackwardSpeed;
    }

    void OnCollisionEnter(Collision collision)
    {
        Collided = true;
    }

    void FixedUpdate ()
    {
        transform.position = Vector3.Lerp (transform.position, TargetF.transform.position, Time.deltaTime * smooth);
        transform.LookAt (Player);

        if (Collided == true) 
        {
            CamMove ();
        }
        if (Collided == false) 
        {
            CamGoBack ();
        }
    }
}
