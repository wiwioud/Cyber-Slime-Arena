using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class HighscoreSystem : MonoBehaviour
{
    private TMP_Text scoreTMP;
    private TMP_Text highscoreTMP;

    private Text scoreLegacyText;
    private Text highscoreLegacyText;

    private int highscore;

    void Start()
    {
        GameObject scoreObj = GameObject.Find("ScoreText");
        GameObject highscoreObj = GameObject.Find("HighscoreText");

        if (scoreObj != null)
        {
            scoreTMP = scoreObj.GetComponent<TMP_Text>();
            scoreLegacyText = scoreObj.GetComponent<Text>();
        }

        if (highscoreObj != null)
        {
            highscoreTMP = highscoreObj.GetComponent<TMP_Text>();
            highscoreLegacyText = highscoreObj.GetComponent<Text>();
        }

        highscore = PlayerPrefs.GetInt("Highscore", 0);
        UpdateHighscoreText();
    }

    void Update()
    {
        int currentScore = ReadCurrentScore();

        if (currentScore > highscore)
        {
            highscore = currentScore;
            PlayerPrefs.SetInt("Highscore", highscore);
            PlayerPrefs.Save();

            UpdateHighscoreText();
        }
    }

    int ReadCurrentScore()
    {
        string scoreString = "";

        if (scoreTMP != null)
            scoreString = scoreTMP.text;
        else if (scoreLegacyText != null)
            scoreString = scoreLegacyText.text;

        string numbersOnly = Regex.Replace(scoreString, @"[^\d]", "");

        if (int.TryParse(numbersOnly, out int result))
            return result;

        return 0;
    }

    void UpdateHighscoreText()
    {
        string text = "Highscore: " + highscore;

        if (highscoreTMP != null)
            highscoreTMP.text = text;

        if (highscoreLegacyText != null)
            highscoreLegacyText.text = text;
    }

    public void ResetHighscore()
    {
        PlayerPrefs.DeleteKey("Highscore");
        highscore = 0;
        UpdateHighscoreText();
    }
}