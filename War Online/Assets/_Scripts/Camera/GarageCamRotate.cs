using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageCamRotate : MonoBehaviour {

    public Transform target;//the target object
    public float speedMod = 10.0f;//a speed modifier
    private Vector3 point;//the coord to the point where the camera looks at
    public float degrees;
    public float speedx;
    public float speedy;
    public float speedz;

    void Start()
    {//Set up things on the start method
        point = target.transform.position;//get target's coords
        transform.LookAt(point);//makes the camera look to it
    }

    void FixedUpdate()
    {//makes the camera rotate around "point" coords, rotating around its Y axis, 20 degrees per second times the speed modifier
        transform.RotateAround(point, new Vector3(speedx, speedy, speedz), degrees * Time.deltaTime * speedMod);
    }
}
