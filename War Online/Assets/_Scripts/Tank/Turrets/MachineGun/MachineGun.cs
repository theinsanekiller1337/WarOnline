using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGun : MonoBehaviour {

    private RaycastHit bulletCast;
    public LineRenderer bulletRenderer;
    public GameObject bulletStartPoint;
    public GameObject barrel;
    public float rollingSpeed;

    private void Start()
    {
        if (bulletRenderer == null)
        {
            bulletRenderer = GetComponent<LineRenderer>();
        }

    }


    private void Update()
    {
        bool attack = Input.GetButton("Fire");
        bool ceaseFire = Input.GetButtonUp("Fire");

        if (attack)
        {

            BarrelRoll();

            RaycastHit hit;
            if (Physics.Raycast(bulletStartPoint.transform.position, bulletStartPoint.transform.forward, out hit))
            {
                bulletCast = hit;
            }

            bulletRenderer.enabled = true;
            bulletRenderer.SetPosition(0, bulletStartPoint.transform.position);
            bulletRenderer.SetPosition(1, bulletCast.point);

        }
        else if (ceaseFire)
        {
            bulletRenderer.enabled = false;
        }
    }

    private void BarrelRoll()
    {

        barrel.transform.Rotate(0f, 0f, 1 * Time.deltaTime * rollingSpeed);

    }

    private void BarrelFire()
    {

    }
}
