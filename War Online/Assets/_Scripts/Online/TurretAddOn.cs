using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAddOn : MonoBehaviour
{
    private TankHealth tankHealth;
    private GameObject thisTurret;
    private GameObject turretBody;
    private GameObject destroyedBody;

    private void Start()
    {
        thisTurret = this.gameObject;
        turretBody = gameObject.transform.Find(thisTurret.name + "_Body").gameObject;
        destroyedBody = gameObject.transform.Find(thisTurret.name + "_Body_D").gameObject;


        tankHealth = GetComponentInParent<TankHealth>();
        tankHealth.actualTurret = turretBody;
        tankHealth.destroyedTurret = destroyedBody;
    }
}
