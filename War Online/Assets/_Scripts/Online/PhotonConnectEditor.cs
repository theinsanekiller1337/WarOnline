﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PhotonConnectEditor : Photon.MonoBehaviour
{

    // public GameObject playerToInstantiate;
    // public Transform[] spawnPoints;
    public GameObject SceneCamera;
    public GameObject Canvas;

    private RTCTankController tankControllerScript;
    private Particle_Emitter particleEmmiter;
    private TurretRotation turretRotator;
    private GameObject playercamera;
    private GameObject[] playerList;

    /*
    public void Start()
    {
        /*
        playerList = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject players in playerList)
        {
            tankControllerScript = GetComponent<RTCTankController>();
            tankControllerScript.enabled = false;
            if (particleEmmiter != GetComponentInChildren<Particle_Emitter>())
            {
                particleEmmiter = GetComponentInChildren<Particle_Emitter>();
                particleEmmiter.enabled = false;
            }

            turretRotator = GetComponentInChildren<TurretRotation>();
            playercamera = playerPrefab.transform.Find("Main Camera").gameObject;
            playercamera.SetActive(false);
        } */
    /*
    Connect();
}

public void Update()
{

}

private void Connect()
{
    PhotonNetwork.ConnectUsingSettings("Waronline v0.0.2");
    Debug.Log("Connected");
}
private void OnGUI()
{
    GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
}

void OnJoinedLobby()
{
    Debug.Log("At last BMC");
    PhotonNetwork.JoinRandomRoom();

}

void OnPhotonRandomJoinFailed()
{
    PhotonNetwork.CreateRoom("Harbour");

}

void OnJoinedRoom()
{
    SetActive();
} */

  /*  public void SetActive()
    {
        SceneCamera.SetActive(false);

        GameObject warCanvas = (GameObject)PhotonNetwork.Instantiate(Canvas.name, Vector3.zero, gameObject.transform.rotation, 0);
        warCanvas.SetActive(true);
        SpawnMyPlayer();

    }*/


    public void SpawnMyPlayer()
    {


        //  gameManager GameMan = this.gameObject.GetComponent<gameManager>();
        //  GameObject playerPrefab = GameMan.RealPlayerPrefab;


        //finding components to turn on in photonview




    }
}


