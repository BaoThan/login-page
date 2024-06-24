using UnityEngine;
using UnityEngine.UI;
using PlayFab.ClientModels;
using PlayFab;
using System;
using System.Text.RegularExpressions;

public class UserInterfaceManager : MonoBehaviour
{
    // Debug Flag to simulate a reset
    public bool ClearPlayerPrefs;

    // UI objects
    public InputField userEmail;
    public InputField userPassword;
    public InputField userConfirmPassword;
    public InputField userDisplayNameInputField;
    public InputField recoveryEmail;
    public Toggle RememberMe;

    public GameObject loggedinPanel;
    public GameObject loginPanel;
    public GameObject recoveryPasswordPanel;
    public GameObject loginButtonGameObject;
    public GameObject signupModeGameObject;

    public Button signupButton;
    public Button loginButton;
    public Button resetPasswordButton;
    public Button appleLoginButton;
    public Button googleLoginButton;
    public Button guestButton;
    public Button switchButton;
    public Button clearSigninButton;
    public Button resetSampleButton;
    public Button logoutButton;
    public Button backButton;
    public Button submitRecoveryButton;

    public Text switchButtonText;
    public Text displayNamePlaceholder;
    public Text userPlayFabID;
    public Text statusMessage;

    // Settings for data to get from PlayFab login.
    public GetPlayerCombinedInfoRequestParams InfoRequestParams;

    // Reference to our Authentication service
    private PlayFabAuthService _AuthService = PlayFabAuthService.Instance;

    public void Awake()
    {
        if (ClearPlayerPrefs)
        {
            _AuthService.UnlinkDeviceID();
            _AuthService.ClearRememberMe();
            _AuthService.AuthType = AuthTypes.None;
        }
        // Set Remember Me toggle to remembered state.
        RememberMe.isOn = _AuthService.RememberMe;
        RememberMe.onValueChanged.AddListener(
            (toggle) =>
            {
                _AuthService.RememberMe = toggle;
            });
    }

    void Start()
    {
        loginPanel.SetActive(true);
        loggedinPanel.SetActive(false);
        signupModeGameObject.SetActive(false);

        signupButton.onClick.AddListener(OnSignupButtonClicked);
        loginButton.onClick.AddListener(OnLoginButtonClicked);
        resetPasswordButton.onClick.AddListener(OnResetPasswordButtonClicked);
        appleLoginButton.onClick.AddListener(OnAppleLoginButtonClicked);
        googleLoginButton.onClick.AddListener(OnGoogleLoginButtonClicked);
        guestButton.onClick.AddListener(OnGuestButtonClicked);
        switchButton.onClick.AddListener(OnSwitchButtonClicked);
        clearSigninButton.onClick.AddListener(OnClearSigninButtonClicked);
        resetSampleButton.onClick.AddListener(OnResetSampleButtonClicked);
        logoutButton.onClick.AddListener(OnLogoutButtonClicked);
        switchButtonText = switchButton.GetComponentInChildren<Text>();
        submitRecoveryButton.onClick.AddListener(OnSubmitRecoveryClicked);
        backButton.onClick.AddListener(OnResetPasswordButtonClicked);

        PlayFabAuthService.OnDisplayAuthentication += OnDisplayAuthentication;
        PlayFabAuthService.OnLoginSuccess += OnLoginSuccess;
        PlayFabAuthService.OnPlayFabError += OnPlayFabError;

        _AuthService.InfoRequestParams = InfoRequestParams;

        _AuthService.Authenticate();
    }

    private void OnSignupButtonClicked()
    {
        statusMessage.text = ValidateInput(userEmail.text,
                                           userPassword.text,
                                           userConfirmPassword.text);
        if (!string.IsNullOrEmpty(statusMessage.text))
        {
            return;
        }
        statusMessage.text = string.Format("Signing Up {0} ...", userEmail.text);

        _AuthService.Email = userEmail.text;
        _AuthService.Password = userPassword.text;
        _AuthService.Authenticate(AuthTypes.SignUp);
    }

    private void OnLoginButtonClicked()
    {
        statusMessage.text = ValidateInput(userEmail.text,
                                           userPassword.text,
                                           null);
        if (!string.IsNullOrEmpty(statusMessage.text))
        {
            return;
        }
        statusMessage.text = string.Format("Logging In As {0} ... ", userEmail.text);

        _AuthService.Email = userEmail.text;
        _AuthService.Password = userPassword.text;
        _AuthService.Authenticate(AuthTypes.EmailAndPassword);
    }

    private void OnResetPasswordButtonClicked()
    {
        if (!string.IsNullOrEmpty(userEmail.text))
        {
            recoveryEmail.text = userEmail.text;
        }
        recoveryPasswordPanel.SetActive(!recoveryPasswordPanel.activeSelf);
    }

    private void OnSubmitRecoveryClicked()
    {
        string validationMessage = ValidateInput(recoveryEmail.text, null, null);
        if (!string.IsNullOrEmpty(validationMessage))
        {
            statusMessage.text = validationMessage;
            return;
        }

        _AuthService.Email = recoveryEmail.text;
        _AuthService.SendRecoveryEmail(
            successMessage => statusMessage.text = successMessage,
            errorMessage => statusMessage.text = errorMessage
        );
    }


    private void OnAppleLoginButtonClicked()
    {
        statusMessage.text = "Initiating Apple Login ...";
        return;
    }

    private void OnGoogleLoginButtonClicked()
    {
        throw new NotImplementedException();
    }

    void OnSwitchButtonClicked()
    {
        if (signupModeGameObject.activeSelf)
        {
            signupModeGameObject.SetActive(false);
            loginButtonGameObject.SetActive(true);
            switchButtonText.text = "Signup";
        }
        else
        {
            loginButtonGameObject.SetActive(false);
            signupModeGameObject.SetActive(true);
            switchButtonText.text = "Login";
        }
    }

    void OnGuestButtonClicked()
    {
        statusMessage.text = "Logging In As Guest ...";
        _AuthService.Authenticate(AuthTypes.AutoAuthenticate);
    }

    private void OnClearSigninButtonClicked()
    {
        _AuthService.ClearRememberMe();
        statusMessage.text = "Sigin info cleared";
    }

    private void OnResetSampleButtonClicked()
    {
        ResetAuthenticationState();
        _AuthService.Authenticate();
    }

    private void OnLogoutButtonClicked()
    {
        ResetAuthenticationState();
        _AuthService.ClearRememberMe();
        statusMessage.text = "Logged out successfully.";
        loggedinPanel.SetActive(false);
        loginPanel.SetActive(true);
    }

    private void ResetAuthenticationState()
    {
        PlayFabClientAPI.ForgetAllCredentials();
        _AuthService.Email = string.Empty;
        _AuthService.Password = string.Empty;
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.LogFormat("Logged In as: {0}", result.PlayFabId);
        statusMessage.text = "";
        loginPanel.SetActive(false);
        loggedinPanel.SetActive(true);
        userPlayFabID.text = result.PlayFabId;
        string displayName = result.InfoResultPayload?.AccountInfo?.TitleInfo?.DisplayName;
        if (displayName != null)
        {
            userDisplayNameInputField.text = displayName;
        }
        else
        {
            displayNamePlaceholder.text = result.PlayFabId;
        }
    }

    private void OnPlayFabError(PlayFabError error)
    {
        switch (error.Error)
        {
            case PlayFabErrorCode.InvalidEmailAddress:
            case PlayFabErrorCode.InvalidPassword:
            case PlayFabErrorCode.InvalidEmailOrPassword:
                statusMessage.text = "Invalid Email or Password";
                break;
            case PlayFabErrorCode.AccountNotFound:
                statusMessage.text = "Email not found. Please check or create a new account.";
                loginButtonGameObject.SetActive(false);
                signupModeGameObject.SetActive(true);
                switchButtonText.text = "Login";
                return;
            default:
                statusMessage.text = error.GenerateErrorReport();
                break;
        }
        Debug.Log(error.Error);
        Debug.Log(error.GenerateErrorReport());
        ResetAuthenticationState();
    }

    private void OnDisplayAuthentication()
    {
        // When AuthType is None
        loggedinPanel.SetActive(false);
        loginPanel.SetActive(true);
        statusMessage.text = "";

        /* Optionally we could do AutoLogin and skip any authentication above
         *
         * _AuthService.Authenticate(Authtypes.AutoLogin);
         *
         * This would auto log player in by device ID
         * and player would never see any authentication UI
         */
    }

    private string ValidateInput(string email, string password, string confirmPassword)
    {
        if (string.IsNullOrEmpty(email))
        {
            return "Email address is required.";
        }

        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            return "Invalid email address.";
        }

        if (password != null && string.IsNullOrEmpty(password))
        {
            return "Password is required.";
        }

        if (confirmPassword != null && string.IsNullOrEmpty(confirmPassword))
        {
            return "Please confirm your password";
        }

        if (confirmPassword != null && confirmPassword != password)
        {
            return "Passwords do not match";
        }

        return string.Empty; // Valid Input, no Error
    }
}
