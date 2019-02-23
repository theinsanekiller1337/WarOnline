using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionID : MonoBehaviour
{
    public float _teamID = 1;
    public float myAccID;

    private void Start()
    {
        myAccID = GetComponent<PhotonView>().viewID;
    }

}
