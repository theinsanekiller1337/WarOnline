using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PhotonConnectEditor : Photon.MonoBehaviour {

    public GameObject playerToInstantiate;
    public Transform[] spawnPoints;
    public GameObject SceneCamera;
    private RTCTankController tankControllerScript;
    private Particle_Emitter particleEmmiter;
    private TurretRotation turretRotator;
    private GameObject playercamera;
    private GameObject[] playerList;

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
        SpawnMyPlayer();
    }

    public void SpawnMyPlayer()
    {
        
        SceneCamera.SetActive(false);
        GameObject playerPrefab = (GameObject)PhotonNetwork.Instantiate(playerToInstantiate.name, spawnPoints[PhotonNetwork.room.PlayerCount - 1].position, spawnPoints[PhotonNetwork.room.PlayerCount - 1].rotation, 0);

        //finding components to turn on in photonview

        GameObject.Find("WarCanvas").SetActive(true);

        playerPrefab.GetComponent<RTCTankController>().enabled = true;

        if (GameObject.Find("Flame_Thrower") != null)
        {
            playerPrefab.GetComponentInChildren<Particle_Emitter>().enabled = true;
        }
        if (GameObject.Find("Sniper") != null)
        {
            playerPrefab.GetComponentInChildren<snipershooting>().enabled = true;
        }

        playerPrefab.GetComponent<TankHealth>().enabled = true;
        playerPrefab.GetComponentInChildren<TurretRotation>().enabled = true;
        playerPrefab.transform.Find("MainCamera").gameObject.SetActive(true);
        

        

    }

}


