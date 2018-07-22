using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Particle_Emitter : Photon.PunBehaviour, IPunObservable {


    public ParticleSystem particleFire;
    public ParticleSystem particleSmoke;
    public float radius = 4f;
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
        Physics.OverlapCapsule(transform.position, newPos, radius);
        /*if (photonView.isMine)*/ ProcessFireInput();
        
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
