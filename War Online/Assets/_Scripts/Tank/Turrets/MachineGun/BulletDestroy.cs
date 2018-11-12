using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDestroy : MonoBehaviour {

    public float time = 15f;
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject, time);
   
    }

}
