using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Counter : MonoBehaviour
{
    public Playfabmanager playfabManager;
    public Text point;
    public Button plusBtn;
    public Button minusBtn;
    public Button sendScoreBtn;

    private int counter = 0;

    // Start is called before the first frame update
    void Start()
    {
        UpdateCounterText();

        plusBtn.onClick.AddListener(IncrementCounter);
        minusBtn.onClick.AddListener(DecrementCounter);
        sendScoreBtn.onClick.AddListener(SendScoreToLeaderboard);
    }

    void IncrementCounter() {
        counter++;
        UpdateCounterText();
    }

    void DecrementCounter() {
        counter--;
        UpdateCounterText();
    }

    void UpdateCounterText() {
        point.text = counter.ToString();
    }

    void SendScoreToLeaderboard() {
        playfabManager.SendLeaderboard(counter);
    }
}


