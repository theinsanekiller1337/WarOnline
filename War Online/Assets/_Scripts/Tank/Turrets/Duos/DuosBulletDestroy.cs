using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuosBulletDestroy : MonoBehaviour
{
    public GameObject Sp1;
    public GameObject Sp2;
    public GameObject Ref;
    public float distance;
    public float damage = 30f;
    Duos ts;
    // Use this for initialization
    void Start()
    {
        Ref = GameObject.Find("Cube");
        Sp1 = GameObject.Find("GameObject");
        Sp2 = GameObject.Find("GameObject1");
        ts = Ref.GetComponent<Duos>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ts.right == true)
        {
            distance = Vector3.Distance(this.transform.position, Sp1.transform.position);

        }
        if (ts.left == true)
        {
            distance = Vector3.Distance(this.transform.position, Sp2.transform.position);

        }
        if (distance > 50f && distance < 50.5f)
        {
            damage -= 5;
        }
        else if (distance > 100 && distance < 100.1f)
        {
            damage -= 5;

        }
        else if (distance > 200 && distance < 200.1f)
        {
            damage -= 10;
        }
        Destroy(this.gameObject, 3f);
    }
}

