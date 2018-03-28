using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperAnimation : MonoBehaviour {

	public Animator animator;
	private bool isScoped = false;
	public GameObject scopeOverlay;
	public Camera mainCamera;
	public float newFOV = 10f;
	public float normalFOV;
	public GameObject tank;
	

	void Update () {

        if (Input.GetButtonDown("Fire")) {

            isScoped = !isScoped;
            animator.SetBool("Scoped", isScoped);
            if (isScoped)
                StartCoroutine(OnScoped());
            

        } else if (Input.GetButtonUp("Fire"))
        {

            onUnscoped();
            return;

        } 
	}

	void onUnscoped() {
	
	  scopeOverlay.SetActive(false);
	  mainCamera.fieldOfView = normalFOV;
	  mainCamera.GetComponent<CamTest>().enabled = true;
	  tank.GetComponent<RTCTankController>().enabled = true;

	}
	IEnumerator OnScoped() {
	
	  yield return new WaitForSeconds(0.15f);

	  scopeOverlay.SetActive(true);
	  normalFOV = mainCamera.fieldOfView;
	  mainCamera.fieldOfView = newFOV;
	  mainCamera.GetComponent<CamTest>().enabled = false;
	  tank.GetComponent<RTCTankController>().enabled = false;

	}

}

