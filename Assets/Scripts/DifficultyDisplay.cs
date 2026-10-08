using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DifficultyDisplay : MonoBehaviour
{
    private TMP_Text difficultyTMP;
    private Text difficultyLegacyText;

    void Start()
    {
        GameObject difficultyObj = GameObject.Find("DifficultyText");

        if (difficultyObj != null)
        {
            difficultyTMP = difficultyObj.GetComponent<TMP_Text>();
            difficultyLegacyText = difficultyObj.GetComponent<Text>();
        }

        string difficulty = PlayerPrefs.GetString("Difficulty", "Normal");

        string displayText = "Difficulty: " + difficulty;

        if (difficultyTMP != null)
        {
            difficultyTMP.text = displayText;

            if (difficulty == "Easy")
                difficultyTMP.color = Color.cyan;
            else if (difficulty == "Hard")
                difficultyTMP.color = Color.red;
            else
                difficultyTMP.color = Color.yellow;
        }

        if (difficultyLegacyText != null)
        {
            difficultyLegacyText.text = displayText;
        }
    }
}