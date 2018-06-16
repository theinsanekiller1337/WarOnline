using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class PlayfabManager : MonoBehaviour {
    public static PlayfabManager Instance;
    private string PlayerIDCache;

    public void RegisterPlayer(RegisterPlayFabUserRequest playerDetails)
    {
        PlayFabClientAPI.RegisterPlayFabUser(playerDetails, PhotonRequestToken, AuthError);
    }

    public void LoginPlayer(LoginWithEmailAddressRequest playerDetails)
    {
        PlayFabClientAPI.LoginWithEmailAddress(playerDetails, PhotonRequestToken, AuthError);
    }

    private void PhotonRequestToken(LoginResult res)
    {
        PlayerIDCache = res.PlayFabId;
        PlayFabClientAPI.GetPhotonAuthenticationToken(new GetPhotonAuthenticationTokenRequest() { PhotonApplicationId = PhotonNetwork.PhotonServerSettings.AppID }, PhotonAuth, AuthError);
    }

    private void PhotonRequestToken(RegisterPlayFabUserResult res)
    {
        PlayerIDCache = res.PlayFabId;
        PlayFabClientAPI.GetPhotonAuthenticationToken(new GetPhotonAuthenticationTokenRequest() { PhotonApplicationId = PhotonNetwork.PhotonServerSettings.AppID }, PhotonAuth, AuthError);
    }

    private void PhotonAuth(GetPhotonAuthenticationTokenResult res)
    {
        // TODO: Change CustomAuthenticationType accordingly (maybe Steam?) in the future
        var authDetails = new AuthenticationValues { AuthType = CustomAuthenticationType.Custom };
        authDetails.AddAuthParameter("username", PlayerIDCache);
        authDetails.AddAuthParameter("token", res.PhotonCustomAuthenticationToken);
        PhotonNetwork.AuthValues = authDetails;
    }

    private void AuthError(PlayFabError err)
    {
        Debug.LogError("Authentication Error: " + err.GenerateErrorReport());
    }
}
