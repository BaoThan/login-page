using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Counter : MonoBehaviour
{
    public Playfabmanager playfabManager;
    public Text point;
    public Button plusButton;
    public Button minusButton;
    public Button sendButton;

    private int counter = 0;


    // Start is called before the first frame update
    void Start()
    {
        UpdateCounterText();

        plusButton.onClick.AddListener(AddScore);
        minusButton.onClick.AddListener(MinusScore);
        sendButton.onClick.AddListener(SendScoreToLeaderboard);
    }

    void AddScore() {
        counter++;
        UpdateCounterText();
    }

    void MinusScore() {
        counter --;
        UpdateCounterText();
    }

    void UpdateCounterText() {
        point.text = counter.ToString();
    }

    void SendScoreToLeaderboard() {
        playfabManager.SendLeaderboard(counter);
    }
}
