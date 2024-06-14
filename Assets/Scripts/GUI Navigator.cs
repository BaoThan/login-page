using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GUINavigator : MonoBehaviour
{
    public GameObject loginPanel;
    public GameObject signupPanel;
    public GameObject counterPanel;
    public Button signUpButton;
    public Button backButton;
    public Button guestButton;

    // Start is called before the first frame update
    void Start()
    {
        loginPanel.SetActive(true);
        signupPanel.SetActive(false);
        counterPanel.SetActive(false);

        signUpButton.onClick.AddListener(SwitchPanel);
        backButton.onClick.AddListener(SwitchPanel);
        guestButton.onClick.AddListener(ShowCounter);
    }

    void SwitchPanel() {
        if (loginPanel.activeSelf) {
            loginPanel.SetActive(false);
            signupPanel.SetActive(true);
        } else {
            signupPanel.SetActive(false);
            loginPanel.SetActive(true);
        }
        counterPanel.SetActive(false);

    }

    void ShowCounter() {
        loginPanel.SetActive(false);
        signupPanel.SetActive(false);
        counterPanel.SetActive(true);
    }

}
