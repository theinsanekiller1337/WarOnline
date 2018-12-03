using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class gameManager : Photon.PunBehaviour {

    [Tooltip("To be honest, this shouldn't be changed at all. But if you do feel like a badass, this controls which scene the player exits to when quitting.")]
    [SerializeField] string exitScene = "Lobby";
    [Tooltip("The prefab to use for representing the player")]
    public GameObject newPlayerPrefab;

    [SerializeField]
    private GameObject realPlayerPrefab;
    public GameObject RealPlayerPrefab
    {
        get { return realPlayerPrefab; }
        set { realPlayerPrefab = value; }
    }
    public Transform[] spawnPoints;
  

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
        if (newPlayerPrefab == null)
        {

            Debug.LogError("<Color=Red><b>Missing</b></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
        }
        else
        {


            if (RTCTankController.LocalPlayerInstance == null)
            {
                Debug.Log("We are Instantiating LocalPlayer from " + SceneManagerHelper.ActiveSceneName);

                int spawnNumber = Random.Range(0, spawnPoints.Length);

                GameObject playerPref =(GameObject)PhotonNetwork.Instantiate(newPlayerPrefab.name, spawnPoints[spawnNumber].position, spawnPoints[spawnNumber].rotation, 0);
                playerPref = realPlayerPrefab;
                PhotonConnectEditor photonConnectEditor = this.gameObject.GetComponent<PhotonConnectEditor>();

                photonConnectEditor.SetActive(); //wrote my script so controls don't get interlinked :)" - gods
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


    #region Public Methods


    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }


    #endregion
}
