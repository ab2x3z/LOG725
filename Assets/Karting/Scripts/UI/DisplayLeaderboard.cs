using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayLeaderboard : MonoBehaviour
{
    private string[] carNames; // Array of car names sorted by position

    void Start()
    {
        SetLeaderboard();
        DrawLeaderboard();
    }
    private void SetLeaderboard()
    {

        int carsCount = PlayerPrefs.GetInt("carsLeaderboardLength");
        carNames = new string[carsCount];
        for (int i = 0; i < carsCount; i++) {
            carNames[i] = PlayerPrefs.GetString("carAtPos_" + i);
        }
    }

    void DrawLeaderboard()
    {
        TMP_Text textComponent = GetComponent<TMP_Text>();

        if (textComponent != null)
        {
            string leaderboard = "Leaderboard:\n";

            for (int i = 0; i < carNames.Length; i++)
            {
                leaderboard += (i + 1) + ". " + carNames[i] + "\n";
            }

            textComponent.text = leaderboard;
        }
        else
        {
            Debug.LogError("Text component not found!");
        }
    }
}