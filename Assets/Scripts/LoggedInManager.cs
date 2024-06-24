using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class LoggedInManager : MonoBehaviour
{
    public Text statusMessage;
    public Button changeDisplayNameButton;
    public InputField playerDisplayName;

    void Start()
    {
        changeDisplayNameButton.onClick.AddListener(OnChangeDisplayNameClicked);
    }

    private void OnChangeDisplayNameClicked()
    {
        statusMessage.text = "";
        if (string.IsNullOrEmpty(playerDisplayName.text))
        {
            statusMessage.text = "Please enter a name to change";
            return;
        }

        PlayFabClientAPI.UpdateUserTitleDisplayName(
            new UpdateUserTitleDisplayNameRequest
            {
                DisplayName = playerDisplayName.text
            },
            result =>
            {
                statusMessage.text = "Display name updated successfully to: " + result.DisplayName;
                Debug.LogFormat("Changed current user name to: {0}", result.DisplayName);
            },
            error =>
            {
                statusMessage.text = "Failed to update display name: " + error.GenerateErrorReport();
                Debug.LogError("Error updating display name: " + error.GenerateErrorReport());
            }
        );
    }
}
