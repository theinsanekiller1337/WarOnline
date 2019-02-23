using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuosBulletDestroy : MonoBehaviour
{
    public float destroyTime;
    public ParticleSystem particle;
    public ParticleSystem destroyedParticle;
    public float damage;

    private TankHealth enemy;

    private void Start()
    {
        particle.Play();
        Destroy();
    }

    private void Update()
    {
        if (particle.isStopped == true)
        {
            particle.Play();
        }
        
    }

    private void Destroy()
    {
        Destroy(gameObject, destroyTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 collisionPoint = collision.GetContact(0).point;
        GameObject destroyedpart = Instantiate(destroyedParticle, collisionPoint, Quaternion.identity).gameObject;
        Destroy(destroyedpart, 0.5f);
        Destroy(gameObject);

        if (collision.gameObject.GetComponentInParent<TankHealth>() != null)
        {
            FactionID fID = collision.gameObject.GetComponentInParent<FactionID>();
            FactionID myID = gameObject.GetComponent<FactionID>();

            if (fID == null || fID._teamID == 1 || myID._teamID == null || myID._teamID == 1 || fID._teamID != myID._teamID)
            {
                if (fID.myAccID != myID.myAccID)
                {
                    Damage();
                    enemy = collision.gameObject.GetComponentInParent<TankHealth>();
                }
            }
        }
    }

    
    void Damage()
    {
        if(enemy != null)
        enemy.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", enemy.GetComponent<PhotonView>().owner, damage);
    }
}

