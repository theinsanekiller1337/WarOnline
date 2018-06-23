using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using PlayFab;
using PlayFab.ClientModels;

public class GateKeeper : MonoBehaviour {
    [Header("Login Gate")]
    [Tooltip("Contains the flashing \"Press Any Key\" graphic.")] [SerializeField] GameObject loginGateHandler;
    [Tooltip("Time in seconds before the login prompt is reset. Defaults to five minutes.")] [SerializeField] float waitTimer = 300.0f;

    [Header("Login Elements")]
    [Tooltip("Contains E-mail and Password input fields, as well as the signup and login buttons.")][SerializeField] GameObject loginElementsHandler;
    [Tooltip("E-mail input field.")] [SerializeField] InputField userEmail;
    [Tooltip("Password input field.")] [SerializeField] InputField userPassword;

    [Header("Register Elements")]
    [Tooltip("Contains Username input field as well as the actual register button.")] [SerializeField] GameObject registerElementsHandler;
    [Tooltip("Username input field.")] [SerializeField] InputField userName;

    [Header("Error Messaging")]
    [Tooltip("Contains error text.")] [SerializeField] Text errorText;

    // Private variables
    private float currentTime;
    private string PlayerIDCache;

    public void RegisterPlayer(RegisterPlayFabUserRequest playerDetails)
    {
        hideErrorMessage();
        PlayFabClientAPI.RegisterPlayFabUser(playerDetails, PhotonRequestToken, AuthError);
    }

    public void LoginPlayer()
    {
        hideErrorMessage();
        PlayFabClientAPI.LoginWithEmailAddress(new LoginWithEmailAddressRequest() { Email = userEmail.text, Password = userPassword.text, TitleId = PlayFab.PlayFabSettings.TitleId }, PhotonRequestToken, AuthError);
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
        
        // Once successful, the engine must asynchronously load the main menu scene

        // TODO: Asynchronous loadScene, loading icon and user-friendly log?
    }

    private void AuthError(PlayFabError err)
    {
        Debug.LogError("Authentication Error: " + err.GenerateErrorReport());
        showErrorMessage("AUTHENTICATION ERROR\n" + err.ErrorMessage);
    }

    private void showErrorMessage(string errorMsg)
    {
        errorText.text = errorMsg;
        errorText.gameObject.SetActive(true);
    }

    private void hideErrorMessage()
    {
        errorText.text = null;
        errorText.gameObject.SetActive(false);
    }

    IEnumerator loginReset()
    {
        currentTime = waitTimer;
        while (currentTime > 0.0f)
        {
            currentTime -= Time.deltaTime;
            yield return null;
        }
        userEmail.text = null;
        userPassword.text = null;
        userName.text = null;
        loginGateHandler.SetActive(true);
        loginElementsHandler.SetActive(false);
        registerElementsHandler.SetActive(false);
        hideErrorMessage();
    }

    void Start()
    {
        currentTime = 0.0f;
        loginGateHandler.SetActive(true);
        loginElementsHandler.SetActive(false);
        registerElementsHandler.SetActive(false);
        errorText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (loginGateHandler.activeInHierarchy)
            {
                loginGateHandler.SetActive(false);
                loginElementsHandler.SetActive(true);
                StartCoroutine("loginReset");
            }
            else currentTime = waitTimer;
        }
    }
}
