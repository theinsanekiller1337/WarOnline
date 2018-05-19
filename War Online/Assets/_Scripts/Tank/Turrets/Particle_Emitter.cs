using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Particle_Emitter : MonoBehaviour {


    public ParticleSystem particleFire;
    public ParticleSystem particleSmoke;
    public float radius = 4f;
    public GameObject secondObject;

	// Use this for initialization
	void Start () {
        particleFire.Stop();
        particleSmoke.Stop();
	}
	
	// Update is called once per frame
	void Update () {

        Vector3 newPos = secondObject.GetComponent<Transform>().position;
        Physics.OverlapCapsule(transform.position, newPos, radius);
        
        if(Input.GetButton("Fire"))
        {

            particleFire.Play();
            particleSmoke.Play();
            if (particleFire.isPlaying)
            {
                Debug.Log("Working!!");
            }
        } else
        {
            particleFire.Stop();
            particleSmoke.Stop();

        }
	}
}
