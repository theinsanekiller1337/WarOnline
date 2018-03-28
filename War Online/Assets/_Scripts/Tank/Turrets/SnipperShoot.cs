using UnityEngine;

public class SnipperShoot : MonoBehaviour {

	public float damage = 10f;
	public float impactForce = 100f;

	public GameObject fpsCam;
	public ParticleSystem muzzleFlash;
	public GameObject impactEffect;
	
	// Update is called once per frame
	void Update () {
		
		if(Input.GetButtonUp("Fire")){
		
		  Shoot();

		}

	}

	void Shoot () {

	muzzleFlash.Play();
	
	 RaycastHit hit;

	 if(Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit)){
	 
	    Debug.Log(hit.transform.name);

		TankHealth target = hit.transform.GetComponent<TankHealth>();

		if(target != null){
		
		target.TakeDamage(damage);

		}

		if (hit.rigidbody != null){
		
		hit.rigidbody.AddForce(-hit.normal * impactForce);

		}

		GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(-hit.normal * impactForce ));
		Destroy(impactGO, 2f);

	 }

	}
}
