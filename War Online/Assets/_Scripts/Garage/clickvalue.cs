using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class clickvalue : MonoBehaviour

{
    public GameObject scripttochange;
    public int value;
    public bool hullorturret;

    public void execute()
    {
        if (!hullorturret)
        {
            scripttochange.GetComponent<HullChange>().selection = value;
            //Debug.Log("1");
        }
        else
        {
            scripttochange.GetComponent<TurretChange>().selection = value;
            //Debug.Log("2");
        }
    }
}
