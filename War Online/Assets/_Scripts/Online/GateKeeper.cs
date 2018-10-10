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
    [Tooltip("Contains the login/signup selection buttons.")] [SerializeField] GameObject loginSelectHandler;
    [Tooltip("Time in seconds before the login prompt is reset. Defaults to five minutes.")] [SerializeField] float waitTimer = 300.0f;

    [Header("Login Elements")]
    [Tooltip("Contains E-mail and Password input fields, as well as the signup and login buttons.")][SerializeField] GameObject loginElementsHandler;
    [Tooltip("E-mail input field.")] [SerializeField] InputField loginUserEmail;
    [Tooltip("Password input field.")] [SerializeField] InputField loginUserPassword;

    [Header("Register Elements")]
    [Tooltip("Contains Username input field as well as the actual register button.")] [SerializeField] GameObject registerElementsHandler;
    [Tooltip("E-mail input field.")] [SerializeField] InputField registerUserEmail;
    [Tooltip("Password input field.")] [SerializeField] InputField registerUserPassword;
    [Tooltip("Username input field.")] [SerializeField] InputField registerUserName;

    [Header("Loading Elements")]
    [Tooltip("Contains loading icon and text.")] [SerializeField] GameObject loadingElementHandler;
    [Tooltip("Contains loading text.")] [SerializeField] Text loadingText;
    [Tooltip("Name of scene to load upon successful authentication")] [SerializeField] string nextScene = "StartScene";

    [Header("Miscellanous Messaging")]
    [Tooltip("Contains error text.")] [SerializeField] Text errorText;
    [Tooltip("Contains application version details")] [SerializeField] Text versionText;
    

    // Private variables
    private float currentTime;
    private string PlayerIDCache;

    public void RegisterPlayer()
    {
        hideErrorMessage();
        showLoading();
        registerElementsHandler.SetActive(false);
        updateLoadingText("Registering New User");
        PlayFabClientAPI.RegisterPlayFabUser(new RegisterPlayFabUserRequest() { Email = registerUserEmail.text, Password = registerUserPassword.text, Username = registerUserName.text, TitleId = PlayFab.PlayFabSettings.TitleId }, PhotonRequestToken, RegisterError);
    }

    public void LoginPlayer()
    {
        hideErrorMessage();
        showLoading();
        loginElementsHandler.SetActive(false);
        updateLoadingText("Attempting to Log In");
        PlayFabClientAPI.LoginWithEmailAddress(new LoginWithEmailAddressRequest() { Email = loginUserEmail.text, Password = loginUserPassword.text, TitleId = PlayFab.PlayFabSettings.TitleId }, PhotonRequestToken, AuthError);
    }

    private void PhotonRequestToken(LoginResult res)
    {
        updateLoadingText("Requesting Authentication Token");
        PlayerIDCache = res.PlayFabId;
        PlayFabClientAPI.GetPhotonAuthenticationToken(new GetPhotonAuthenticationTokenRequest() { PhotonApplicationId = PhotonNetwork.PhotonServerSettings.AppID }, PhotonAuth, AuthError);
    }

    private void PhotonRequestToken(RegisterPlayFabUserResult res)
    {
        updateLoadingText("Requesting Authentication Token");
        PlayerIDCache = res.PlayFabId;
        PlayFabClientAPI.GetPhotonAuthenticationToken(new GetPhotonAuthenticationTokenRequest() { PhotonApplicationId = PhotonNetwork.PhotonServerSettings.AppID }, PhotonAuth, RegisterError);
    }

    private void PhotonAuth(GetPhotonAuthenticationTokenResult res)
    {
        emptyLoginFields();
        emptyRegisterFields();
        updateLoadingText("Loading Game");
        // TODO: Change CustomAuthenticationType accordingly (maybe Steam?) in the future
        var authDetails = new AuthenticationValues { AuthType = CustomAuthenticationType.Custom };
        authDetails.AddAuthParameter("username", PlayerIDCache);
        authDetails.AddAuthParameter("token", res.PhotonCustomAuthenticationToken);
        PhotonNetwork.AuthValues = authDetails;
        StartCoroutine(loadNextScene());
    }

    private void AuthError(PlayFabError err)
    {
        hideLoading();
        loginElementsHandler.SetActive(true);
        Debug.LogError("Authentication Error: " + err.GenerateErrorReport());
        showErrorMessage("AUTHENTICATION ERROR\n" + err.ErrorMessage);
    }

    private void RegisterError(PlayFabError err)
    {
        hideLoading();
        registerElementsHandler.SetActive(true);
        Debug.LogError("Registration Error: " + err.GenerateErrorReport());
        showErrorMessage("REGISTRATION ERROR\n" + err.ErrorMessage);
    }

    private void updateLoadingText(string loadMsg)
    {
        loadingText.text = loadMsg;
    }

    private void showLoading()
    {
        loadingElementHandler.SetActive(true);
    }

    private void hideLoading()
    {
        loadingElementHandler.SetActive(false);
        loadingText.text = null;
    }

    private void showErrorMessage(string errorMsg)
    {
        errorText.text = errorMsg;
        errorText.gameObject.SetActive(true);
    }

    public void hideErrorMessage()
    {
        errorText.text = null;
        errorText.gameObject.SetActive(false);
    }

    public void emptyLoginFields()
    {
        loginUserEmail.text = null;
        loginUserPassword.text = null;
    }

    public void emptyRegisterFields()
    {
        registerUserEmail.text = null;
        registerUserPassword.text = null;
        registerUserName.text = null;
    }

    private void returnToGate()
    {
        emptyLoginFields();
        emptyRegisterFields();
        loginGateHandler.SetActive(true);
        loginSelectHandler.SetActive(false);
        loginElementsHandler.SetActive(false);
        registerElementsHandler.SetActive(false);
        hideErrorMessage();
        hideLoading();
    }

    IEnumerator loginReset()
    {
        currentTime = waitTimer;
        while (currentTime > 0.0f)
        {
            currentTime -= Time.deltaTime;
            yield return null;
        }
        returnToGate();
    }

    IEnumerator loadNextScene()
    {
        while (!SceneManager.LoadSceneAsync(nextScene).isDone) yield return null;
    }

    void Start()
    {
        currentTime = 0.0f;
        returnToGate();
        versionText.text += ("\nVersion " + Application.version);
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (loginGateHandler.activeInHierarchy)
            {
                loginGateHandler.SetActive(false);
                loginSelectHandler.SetActive(true);
                StartCoroutine("loginReset");
            }
            else currentTime = waitTimer;
        }
    }
}
