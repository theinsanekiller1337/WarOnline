using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duos : MonoBehaviour
{
    public GameObject Sp1;
    public GameObject Bullet;
    public GameObject Sp2;
    public bool right = true;
    public bool left = false;
    public float force;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire"))
        {
            if (right == true)
            {
                StartCoroutine(Waiting());

            }
            else if (left == true)
            {
                StartCoroutine(Waiting2());

            }


        }
    }

    IEnumerator Waiting()
    {
        GameObject G = Instantiate(Bullet, Sp1.transform.position, Sp1.transform.rotation) as GameObject;
 
        G.GetComponent<Rigidbody>().AddForce(G.transform.forward * force);

        right = false;
        yield return new WaitForSeconds(0.5f);
        left = true;

    }
    IEnumerator Waiting2()
    {
        GameObject G = Instantiate(Bullet, Sp2.transform.position, Sp2.transform.rotation) as GameObject;
       
        G.GetComponent<Rigidbody>().AddForce(G.transform.forward * force);
        left = false;

        yield return new WaitForSeconds(0.5f);
        right = true;

    }
}


