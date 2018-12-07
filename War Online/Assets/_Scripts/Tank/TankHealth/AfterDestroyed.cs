using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterDestroyed : MonoBehaviour {

    private bool spawnCalled;
    private gameManager photonScript;

    void Start () {

        spawnCalled = false;
        photonScript = GameObject.Find("GameManager").GetComponent<gameManager>();
    }

    

    private void OnDestroy()
    {        
        if (!spawnCalled)
        {   
            photonScript.SpawnTank();
            spawnCalled = true;
        }
    }
}
