using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurretChange : MonoBehaviour {
    public Button btn;
    public GameObject FlameThrower;
    public GameObject MachineGun;
    public GameObject Acidton;
    public GameObject Sniper;
    public GameObject parent;

	// Use this for initialization
	void Start ()
    {
        
        //parent = GameObject.Find("HullSpawn");
        //Scorpion = parent.FindObject("Scorpion");
        //Dominator = parent.Find("Dominator");
        //Falcon = GameObject.Find("Falcon");
        //Panzer = GameObject.Find("Panzer");
        
        Button btnclick = btn.GetComponent<Button>();
        btnclick.onClick.AddListener(TaskOnClick);
    }
    void disableAll()
    {
        FlameThrower.SetActive(false);
        MachineGun.SetActive(false);
        Acidton.SetActive(false);
        Sniper.SetActive(false);
    }
    void TaskOnClick()
    {
        
        if (gameObject.name == "FlameBtn")
        {
            disableAll();
            FlameThrower.SetActive(true);
        }
        else if (gameObject.name == "MachineGunBtn")
        {
            disableAll();
            MachineGun.SetActive(true);
        }
        else if (gameObject.name == "SniperBtn")
        {
            disableAll();
            Sniper.SetActive(true);
        }
        else if (gameObject.name == "AcidtonBtn")
        {
            disableAll();
            Acidton.SetActive(true);
        }
    }
}
