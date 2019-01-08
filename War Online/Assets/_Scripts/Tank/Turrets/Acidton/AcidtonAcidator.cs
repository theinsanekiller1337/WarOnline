using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class AcidtonAcidator : Photon.PunBehaviour, IPunObservable
{
    [Header("Acid Colors")]
    public ObiEmitter acidEmitter;
    public float speed;
    public float lerpTime;
    public float inverseLerpTime;
    

    [Header("Damages")]
    public float damage;
    public GameObject startPoint;
    public GameObject endPoint;
    public float radius;

    
    private bool isFiring;


    void Start()
    {
        
    }

    void ProcessFireInput()
    {
        if (Input.GetButton("Fire")) isFiring = true;
        else isFiring = false;
    }

    void Update()
    {
        ProcessFireInput();

        if (isFiring)
        {
            Vector3 newStartPos = startPoint.GetComponent<Transform>().position;
            Vector3 newPos = endPoint.GetComponent<Transform>().position;
            Collider[] colliders = Physics.OverlapCapsule(newStartPos, newPos, radius);



            for (int i = 1; i < colliders.Length; i++)
            {

                Rigidbody targetRigidbody = colliders[i].GetComponentInParent<Rigidbody>();
                if (!targetRigidbody)
                    continue;


                TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>();
                if (!targetHealth)
                    continue;

                else if (targetHealth)
                {

                    if (targetHealth.GetComponentInParent<Transform>().gameObject.GetComponentInChildren<Particle_Emitter>().gameObject == this.gameObject.GetComponentInParent<Transform>().gameObject)
                    {
                        targetHealth.TakeDamage(-damage);
                        continue;
                    }
                    targetHealth.TakeDamage(damage);
                }


            }
            //mechanism for Particle Control
            if (acidEmitter.speed >= speed)
            {
                acidEmitter.speed = speed;

            } else if(acidEmitter.speed < speed) {
                
                acidEmitter.speed = Mathf.Lerp(acidEmitter.speed, speed, lerpTime);

            }

        } else
        {
            if (acidEmitter.speed <= 0f)
            {
                acidEmitter.speed = 0f;

            }
            else if (acidEmitter.speed > 0f)
            {

                acidEmitter.speed = Mathf.Lerp(acidEmitter.speed, 0f, inverseLerpTime);

            }
        }
        
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting) stream.SendNext(isFiring);
        else this.isFiring = (bool)stream.ReceiveNext();
    }
}

