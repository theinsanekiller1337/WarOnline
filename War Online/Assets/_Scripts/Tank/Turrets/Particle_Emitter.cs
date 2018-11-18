using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Particle_Emitter : Photon.PunBehaviour, IPunObservable {


    public ParticleSystem particleFire;
    public ParticleSystem particleSmoke;
    public float radius = 4f;
    public float damage = 4f;
    public GameObject secondObject;
    bool isFiring;

    void ProcessFireInput()
    {
        if (Input.GetButton("Fire")) isFiring = true;
        else isFiring = false;
    }

	// Use this for initialization
	void Start () {
        particleFire.Stop();
        particleSmoke.Stop();
        
	}
	
	// Update is called once per frame
	void Update () {

        Vector3 newPos = secondObject.GetComponent<Transform>().position;
        Collider[] colliders = Physics.OverlapCapsule(transform.position, newPos, radius);

        for (int i = 0; i < colliders.Length; i++)
        {
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();
            if (!targetRigidbody)
                continue;

            TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>();
            if (!targetHealth)
            {
                continue;
            }
            else if(targetHealth)
            targetHealth.TakeDamage(damage);

        }


            /*if (photonView.isMine)*/
            ProcessFireInput();
        
        if (isFiring)
        {
            particleFire.Play();
            particleSmoke.Play();
            if (particleFire.isPlaying) Debug.Log("Working!!");
        }
        else
        {
            particleFire.Stop();
            particleSmoke.Stop();
        }

       
	}

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting) stream.SendNext(isFiring);
        else this.isFiring = (bool)stream.ReceiveNext();
    }
}
