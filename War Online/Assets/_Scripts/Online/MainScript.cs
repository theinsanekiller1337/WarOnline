using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainScript : Photon.PunBehaviour {


    public byte maxPlayersPerRoom = 8;
    public PhotonLogLevel logLevel = PhotonLogLevel.Informational;
    public bool isConnecting;
    public GameObject playerPrefab;
    public Transform[] spawnPoints;
    public string RoomName;

    // Note: Harbour scene won't be scene 1 in build forever.
    // To prevent this, we load scene according to scene name.
    public void LoadScene(string SceneName) {
		SceneManager.LoadScene(SceneName);
	}

    public void OnClick_CreateRoom() {

        PhotonNetwork.JoinRandomRoom();

    }
        
        
    

    private void Awake()
    {
        PhotonNetwork.autoJoinLobby = true;
        PhotonNetwork.automaticallySyncScene = false;
        Connect();
    }


    public void Connect()
    {
        isConnecting = true;
        if (PhotonNetwork.connected) PhotonNetwork.JoinRandomRoom();
        else PhotonNetwork.ConnectUsingSettings("Waronline v0.0.2");
    }

    #region Photon.PunBehaviour CallBacks
    // Note: Couldn't be bothered to read up. I just copypasted these from the Photon tutorial.


    public override void OnConnectedToMaster()
    {
        Debug.Log("DemoAnimator/Launcher: OnConnectedToMaster() was called by PUN");
        if (isConnecting) PhotonNetwork.JoinRandomRoom();
    }


    public override void OnDisconnectedFromPhoton()
    {
        Debug.LogWarning("DemoAnimator/Launcher: OnDisconnectedFromPhoton() was called by PUN");
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        Debug.Log("DemoAnimator/Launcher:OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");

        RoomOptions roomOptions = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 4 };

        if (PhotonNetwork.CreateRoom(RoomName, roomOptions, TypedLobby.Default))
        {
            print("create room successfully sent.");
        }
        else
        {
            print("create room failed to send");
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("DemoAnimator/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
        PhotonNetwork.LoadLevel("Harbour");
        //Prefab not Instantiatng in Editor, hence adding this script, temporary
        //started
      //  PhotonNetwork.Instantiate(playerPrefab.name, spawnPoints[PhotonNetwork.room.PlayerCount - 1].position, spawnPoints[PhotonNetwork.room.PlayerCount - 1].rotation, 0);
        //ended
        if (playerPrefab != null)
            Debug.Log("Success");
        else
            Debug.Log("Failed :(");
    }


    #endregion
}
