using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HullChange : MonoBehaviour {
    public Button btn;
    public GameObject Scorpion;
    public GameObject Dominator;
    public GameObject Falcon;
    public GameObject Panzer;
    

	// Use this for initialization
	void Start ()
    {
        Scorpion = GameObject.Find("Scorpion");
        Dominator = GameObject.Find("Dominator");
        Falcon = GameObject.Find("Falcon");
        Panzer = GameObject.Find("Panzer");
        
        Button btnclick = btn.GetComponent<Button>();
        btnclick.onClick.AddListener(TaskOnClick);
    }
    void disableAll()
    {
        Scorpion.SetActive(false);
        Falcon.SetActive(false);
        Panzer.SetActive(false);
        Dominator.SetActive(false);
    }
    void TaskOnClick()
    {
        Debug.Log("You have clicked me cunt");
        if (gameObject.name == "DominatorBtn")
        {
            disableAll();
            Dominator.SetActive(true);
        }
        else if (gameObject.name == "FalconBtn")
        {
            disableAll();
            Falcon.SetActive(true);
        }
        else if (gameObject.name == "PanzerBtn")
        {
            disableAll();
            Panzer.SetActive(true);
        }
        else if (gameObject.name == "ScorpionBtn")
        {
            disableAll();
            Scorpion.SetActive(true);
        }
    }
}
