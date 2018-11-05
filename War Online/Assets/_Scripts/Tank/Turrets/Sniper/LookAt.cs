using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour {

    private RaycastHit _raySniper;
    private snipershooting SniperScript;
    public Vector3 toLookAt;
    
    private void Update()
    {
        Debug.Log(SniperScript.lookAtRaycast);

        RaycastHit raySniper = SniperScript.lookAtRaycast;
        raySniper = _raySniper;
        toLookAt = _raySniper.point;
    }
}
