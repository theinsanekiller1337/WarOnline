using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Duos : MonoBehaviour
{
    public GameObject Sp1;
    public GameObject Bullet;
    public GameObject Sp2;
    public bool right = true;
    public bool left = false;
    public float force;
    public float LerpValue;

    private Slider coolDownSlider;
    private Image fillImage;
    private TankHealth tankHealth;
    private GameObject mainCanvas;
    

    // Use this for initialization
    void Start()
    {
        tankHealth = GetComponentInParent<TankHealth>();
        mainCanvas = tankHealth.warCanvas;
        GameObject coolDownUI = mainCanvas.transform.Find("CoolDownUI").gameObject;
        coolDownSlider = coolDownUI.GetComponent<Slider>();
        GameObject coolDown = coolDownUI.transform.Find("CoolDown").gameObject;
        fillImage = coolDown.GetComponentInChildren<Image>();
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
        coolDownSlider.value = Mathf.Lerp(1f, 0f, LerpValue);
        GameObject G = Instantiate(Bullet, Sp1.transform.position, Sp1.transform.rotation) as GameObject;
 
        G.GetComponent<Rigidbody>().AddForce(G.transform.forward * force);

        right = false;
        
        yield return new WaitForSeconds(0.5f);
        coolDownSlider.value = Mathf.Lerp(0f, 1f, LerpValue);
        left = true;

    }
    IEnumerator Waiting2()
    {
        coolDownSlider.value = Mathf.Lerp(1f, 0f, LerpValue);
        GameObject G = Instantiate(Bullet, Sp2.transform.position, Sp2.transform.rotation) as GameObject;
       
        G.GetComponent<Rigidbody>().AddForce(G.transform.forward * force);


        left = false;

        yield return new WaitForSeconds(0.5f);
        coolDownSlider.value = Mathf.Lerp(0f, 1f, LerpValue);
        right = true;

    }
}


