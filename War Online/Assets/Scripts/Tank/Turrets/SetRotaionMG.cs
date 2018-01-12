using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRotaionMG : MonoBehaviour {

    public GameObject mgTurret;
    public GameObject thisobject;
	
	// Update is called once per frame
	void Update () {

        FollowRotation();

	}

    private void FollowRotation()
    {

        Quaternion mgTurretRotation = mgTurret.GetComponent<Transform>().rotation;
        thisobject.transform.rotation = mgTurretRotation;

    }
}
