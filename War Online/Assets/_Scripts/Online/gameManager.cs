using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class gameManager : Photon.PunBehaviour {

    [Tooltip("To be honest, this shouldn't be changed at all. But if you do feel like a badass, this controls which scene the player exits to when quitting.")]
    [SerializeField] string exitScene = "Lobby";
    [Tooltip("The prefab to use for representing the player")]
    public string newPlayerPrefab;
    public GameObject sceneCam;

    private GameObject Canvas;
    public Transform[] spawnPoints;
    private GameObject playerPrefab;
    private PrefabChange prefabChange;

    private bool firstTimeCalled;
  

    // Why are we using a static gameManager instance? The tutorial says nothing about this.
    // "It's a surprise tool that will help us later." -- Mickey Mouse
    public static gameManager _instance;
    private void Awake()
    {
        /*if (_instance != null) Destroy(gameObject);
        else if (_instance == null) _instance = this;
        DontDestroyOnLoad(this);*/
    }

    private void Start()
    {
        firstTimeCalled = false;

        Canvas = Resources.Load<GameObject>("WarCanvas");
        if (newPlayerPrefab == null)
        {

            Debug.LogError("<Color=Red><b>Missing</b></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
        }
        else
        {


            if (RTCTankController.LocalPlayerInstance == null)
            {
                SpawnTank();
            }
            else
            {
                Debug.Log("Ignoring scene load for " + SceneManagerHelper.ActiveSceneName);
            }


        }
    }

    
    // Note: It's 2:30am and I just want to flex on Srijan and the boys before I sleep for work later.
    #region Photon Messages

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(exitScene);
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer other)
    {
        Debug.Log("OnPhotonPlayerConnected() " + other.NickName); // not seen if you're the player connecting


        if (PhotonNetwork.isMasterClient)
        {
            Debug.Log("OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient); // called before OnPhotonPlayerDisconnected
            LoadLevel();
        }
    }


    public override void OnPhotonPlayerDisconnected(PhotonPlayer other)
    {
        Debug.Log("OnPhotonPlayerDisconnected() " + other.NickName); // seen when other disconnects


        if (PhotonNetwork.isMasterClient)
        {
            Debug.Log("OnPhotonPlayerDisonnected isMasterClient " + PhotonNetwork.isMasterClient); // called before OnPhotonPlayerDisconnected
            LoadLevel();
        }
    }

    #endregion

    #region Private Methods


    void LoadLevel()
    {
        if (!PhotonNetwork.isMasterClient)
        {
            Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
        }
        Debug.Log("PhotonNetwork : Loading Level");
        // Note: I will only rig the Harbour scene for multiplayer for now.
        PhotonNetwork.LoadLevel("Harbour");
    }


    #endregion

    //^^gods was here
    #region Instantiating Method

      public void SpawnTank()
    {

        prefabChange = GameObject.Find("Dropdown").GetComponent<PrefabChange>();

        newPlayerPrefab = prefabChange.prefabToSpawn;
        Debug.Log(newPlayerPrefab);

        Debug.Log("We are Instantiating LocalPlayer from " + SceneManagerHelper.ActiveSceneName);

        int spawnNumber = Random.Range(0, spawnPoints.Length);

        GameObject playerPrefab = (GameObject)PhotonNetwork.Instantiate(newPlayerPrefab, spawnPoints[spawnNumber].position, spawnPoints[spawnNumber].rotation, 0);
        Debug.Log(playerPrefab);
        sceneCam.SetActive(false);
        //Activating Objects

        playerPrefab.GetComponent<RTCTankController>().enabled = true;

        if (playerPrefab.transform.Find("FlameThrower") != null)
        {
            
            playerPrefab.GetComponentInChildren<Particle_Emitter>().enabled = true;
            GameObject flameThrower = playerPrefab.transform.Find("FlameThrower").gameObject;
            flameThrower.transform.Find("MainCamera").gameObject.SetActive(true);
        }
        else if (playerPrefab.transform.Find("Sniper") != null)
        {
            playerPrefab.GetComponentInChildren<snipershooting>().enabled = true;
            GameObject sniper = playerPrefab.transform.Find("Sniper").gameObject;
            sniper.transform.Find("MainCamera").gameObject.SetActive(true);
        }
        else if (playerPrefab.transform.Find("MachineGun") != null)
        {
            playerPrefab.GetComponentInChildren<MachineGun>().enabled = true;
            GameObject machineGun = playerPrefab.transform.Find("MachineGun").gameObject;
            machineGun.transform.Find("MainCamera").gameObject.SetActive(true);
        }

        playerPrefab.GetComponent<TankHealth>().enabled = true;
        playerPrefab.GetComponentInChildren<TurretRotation>().enabled = true;

        if (!firstTimeCalled)
        {
            CanvasInstantiate();
            firstTimeCalled = true;
        }
    }

    private void CanvasInstantiate()
    {
        GameObject warCanvas = (GameObject)PhotonNetwork.Instantiate(Canvas.name, Vector3.zero, gameObject.transform.rotation, 0);
        warCanvas.SetActive(true);
    }
 #endregion

    #region Public Methods


    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }


    #endregion
}
