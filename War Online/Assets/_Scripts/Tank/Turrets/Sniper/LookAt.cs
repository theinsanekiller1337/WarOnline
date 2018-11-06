using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour {

  
    private snipershooting SniperScript;
    public Vector3 toLookAt;

    private void Start()
    {
        SniperScript = GetComponentInParent<snipershooting>(); 
    }


    private void LateUpdate()
    {

        RaycastHit raySniper = SniperScript.lookAtRaycast;
        raySniper.point = toLookAt;

        Debug.Log(toLookAt);
        
    }
}
