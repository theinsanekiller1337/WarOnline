using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelGun : MonoBehaviour
{

    private float delayTime = 0f;
    public GameObject barrel;
    public GameObject shootingPoint;
    public GameObject bullet;
    public float reloadDuration = 4f;
    public float rollingSpeed = 4f;
    public float forceForBullet = 10f;
    public float ammo = 10f;
    public float bulletSec = 2f;
    public float fullAmmo = 10f;
    private bool reloading;
    

    // Use this for initialization
    void Start()
    {
        reloading = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButton("Fire"))
        {
            BarrelRoll();

            if (Input.GetButton("Fire") && Time.time >= delayTime)
            {
                delayTime = Time.time + 1 / reloadDuration;
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
        if (ammo >= 1)
        {
            for (int i = 0; i < bulletSec; i++)
            {
             
                var bulletPrefab = Instantiate(bullet, shootingPoint.transform.position, gameObject.transform.rotation);
                bulletPrefab.AddComponent<TimeOut>();
                // Add velocity to the bullet
                bulletPrefab.GetComponent<Rigidbody>().velocity = -transform.forward * Time.deltaTime * forceForBullet * 1000;
            } ammo--;

        }  else if (ammo == 0)
        {
          
            reloading = true;
            
        }

    }
  
}
