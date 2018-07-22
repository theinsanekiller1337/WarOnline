using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainScript : Photon.PunBehaviour {


    public byte maxPlayersPerRoom = 8;
    public PhotonLogLevel logLevel = PhotonLogLevel.Informational;
    bool isConnecting;

    // Note: Harbour scene won't be scene 1 in build forever.
    // To prevent this, we load scene according to scene name.
    public void LoadScene(string SceneName) {
		SceneManager.LoadScene(SceneName);
	}

    private void Awake()
    {
        PhotonNetwork.autoJoinLobby = false;
        PhotonNetwork.automaticallySyncScene = false;
    }

    public void Connect()
    {
        isConnecting = true;
        if (PhotonNetwork.connected) PhotonNetwork.JoinRandomRoom();
        else PhotonNetwork.ConnectUsingSettings(Application.version);
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
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = maxPlayersPerRoom }, null);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("DemoAnimator/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
        PhotonNetwork.LoadLevel("Harbour");
    }


    #endregion
}
