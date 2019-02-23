using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Particle_Emitter : Photon.PunBehaviour, IPunObservable {


    public ParticleSystem particleFire;
    public ParticleSystem particleSmoke;
    public float radius = 4f;
    public float damage = 4f;
    public GameObject startPoint;
    public GameObject endPoint;
    bool isFiring;

    private TankHealth enemy;


	// Use this for initialization
	void Start () {
        particleFire.Stop();
        particleSmoke.Stop(); 
	}
	
	// Update is called once per frame
	void Update () {

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
                    FactionID fID = targetHealth.gameObject.GetComponent<FactionID>();
                    FactionID myID = gameObject.GetComponentInParent<FactionID>();
                    
                    if (fID == null || fID._teamID == 1 || myID._teamID == null || myID._teamID == 1 || fID._teamID != myID._teamID)
                    {
                        if(fID.myAccID != myID.myAccID)
                        {
                            Damage();
                            enemy = targetHealth;
                        } 
                    }
                }
                    
            }
        }
            /*if (photonView.isMine)*/
            ProcessFireInput();
        
        if (isFiring)
        {
            particleFire.Play();
            particleSmoke.Play();
            
        }
        else if(!isFiring)
        {
            particleFire.Stop();
            particleSmoke.Stop();
        }

       
	}

    void Damage()
    {
        if (enemy != null)
        {
            enemy.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", enemy.GetComponent<PhotonView>().owner, damage);
        }
    }


    void ProcessFireInput()
    {
        if (Input.GetButton("Fire"))
            isFiring = true;

        else if (Input.GetButtonUp("Fire"))
        {
            isFiring = false;
        }
        else
        {
            isFiring = false;
        }
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting) stream.SendNext(isFiring);
        else this.isFiring = (bool)stream.ReceiveNext();
    }
}
