using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrefabChange : MonoBehaviour {

    public Dropdown dropdown;
    public string prefabToSpawn;

    private gameManager connectionScript;
    

	// Use this for initialization
	void Start () {
        GameObject gameMan = GameObject.Find("GameManager");
        connectionScript = gameMan.GetComponent<gameManager>();
       
	}
	
	// Update is called once per frame
	void Update () {
        if (dropdown.value == 0)
        {
            prefabToSpawn = "FalconFT";
        }
        else if (dropdown.value == 1)
        {
            prefabToSpawn = "DominatorMG";
        }
        else if(dropdown.value == 2)
        {
            prefabToSpawn = "DominatorSP";
        }
    }
}
