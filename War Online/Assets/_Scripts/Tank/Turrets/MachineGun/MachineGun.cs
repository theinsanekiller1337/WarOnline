using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGun : MonoBehaviour {

    [Header("Damages")]
    public float damage;
    public float bulletSec = 2f;
    private float delayTime;
    private Transform target;

    [Header("Reload")]
    public float reloadDuration;
    private float waitTime;

    [Header("Range")]
    public float radius = 4f;
    public GameObject startPoint;
    public GameObject endPoint;

    [Header("Game Objects")]
    public GameObject bulletStartPoint;
    public GameObject barrel;

    [Header("Bullet Material Effect")]
    public float XAxis;
    public float YAxis;

    private Material bulletMaterial;
    private RaycastHit bulletCast;
    private LineRenderer bulletRenderer;
    private TankHealth enemy;

    public float rollingSpeed;

    private void Start()
    {
        if (bulletRenderer == null)
        {
            bulletRenderer = GetComponent<LineRenderer>();
        }

        bulletRenderer = GetComponent<LineRenderer>();
        bulletMaterial = bulletRenderer.material;
    }


    private void LateUpdate()
    {
        bool attack = Input.GetButton("Fire");
        bool ceaseFire = Input.GetButtonUp("Fire");

        if (attack)// && Time.time >= waitTime)
        {

            waitTime = Time.time + 1 / reloadDuration;

                BarrelRoll();

            RaycastHit hit;
            if (Physics.Raycast(bulletStartPoint.transform.position, bulletStartPoint.transform.forward, out hit))
            {
                bulletCast = hit;
                HitDamage();
            }
            Debug.Log(bulletCast.transform.position);

            Vector3 newStartPos = startPoint.GetComponent<Transform>().position;
            Vector3 newPos = endPoint.GetComponent<Transform>().position;
            Collider[] colliders = Physics.OverlapCapsule(newStartPos, newPos, radius);
            Debug.Log("Collider" + colliders[0]);

            for (int i = 0; i < colliders.Length; i++)
            {
                Transform targetTransform = colliders[i].GetComponent<Transform>();
               
                if (!targetTransform)
                    continue;
                else if (targetTransform)
                {
                    TankHealth targetHealth = targetTransform.GetComponentInParent<TankHealth>();
                    if (!targetHealth)
                    {
                        bulletRenderer.enabled = true;
                        bulletRenderer.SetPosition(0, bulletStartPoint.transform.position);
                        bulletRenderer.SetPosition(1, bulletCast.point);
                        
                    }
                        
                    else if (targetHealth)
                    {
                        target = targetTransform;
                        bulletRenderer.enabled = true;
                        bulletRenderer.SetPosition(0, bulletStartPoint.transform.position);
                        bulletRenderer.SetPosition(1, target.position);
                    }
                }
                    
            }


            bulletMaterial.SetTextureOffset("_MainTex", new Vector2(XAxis * Time.deltaTime, YAxis * Time.deltaTime));

        }
        else if (ceaseFire || Time.time < waitTime)
        {
            bulletRenderer.enabled = false;
        }
    }

    private void BarrelRoll()
    {

        barrel.transform.Rotate(0f, 0f, 1 * Time.deltaTime * rollingSpeed);

    }

    private void HitDamage()
    {
        TankHealth targetHealth = bulletCast.transform.gameObject.GetComponent<TankHealth>();

        if (targetHealth && Time.time >= delayTime)
        {
            FactionID fID = targetHealth.gameObject.GetComponent<FactionID>();
            FactionID myID = gameObject.GetComponent<FactionID>();

            if (fID == null || fID._teamID == 1 || myID._teamID == null || myID._teamID == 1 || fID._teamID != myID._teamID)
            {
                if (fID.myAccID != myID.myAccID)
                {
                    Damage();
                    enemy = targetHealth;
                }
            }
            

            delayTime = Time.time + 1 / bulletSec;

        } else if (!targetHealth && Time.time >= delayTime)
        {
            TankHealth targetH = bulletCast.transform.gameObject.GetComponentInParent<TankHealth>();
            if (targetH)
            {
                FactionID fID = targetHealth.gameObject.GetComponent<FactionID>();
                FactionID myID = gameObject.GetComponent<FactionID>();

                if (fID == null || fID._teamID == 0 || myID._teamID == null || myID._teamID == 0 || fID._teamID != myID._teamID)
                {
                    if (fID.myAccID != myID.myAccID)
                    {
                        Damage();
                        enemy = targetHealth;
                    }
                }
                delayTime = Time.time + 1 / bulletSec;
            }
        }
    }
     void Damage()
    {
        if(enemy != null)
        enemy.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", enemy.GetComponent<PhotonView>().owner, damage);
    }
}
