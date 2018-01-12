using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelGun : MonoBehaviour
{

    private float delayTime = 0.8f;
    public GameObject barrel;
    public GameObject barrelTurret;
    public GameObject shootingPoint;
    public GameObject bullet;
    public float rollingSpeed = 4f;
    public float forceForBullet = 10f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButton("Fire"))
        {
            BarrelRoll();

            if (Input.GetButton("Fire") && Time.time >= delayTime)
            {
                BarrelFire();
            }
        }

    }

    void BarrelRoll()
    {

        barrel.transform.Rotate(0f, 1 * Time.deltaTime * rollingSpeed, 0f);

    }

    void BarrelFire()
    {
        Transform shootingPointRB = shootingPoint.GetComponent<Transform>();
        GameObject bulletNew = Instantiate(bullet, shootingPointRB.position, shootingPointRB.rotation);
        Rigidbody bulletNewRB = bulletNew.GetComponent<Rigidbody>();
        bulletNewRB.AddForce(Vector3.forward * forceForBullet, ForceMode.Force);
    }
}
