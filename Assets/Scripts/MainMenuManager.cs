using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public string gameSceneName = "CyberSlimeArena_Visual";

    public void StartEasy()
    {
        PlayerPrefs.SetString("Difficulty", "Easy");
        PlayerPrefs.Save();
        SceneManager.LoadScene(gameSceneName);
    }

    public void StartNormal()
    {
        PlayerPrefs.SetString("Difficulty", "Normal");
        PlayerPrefs.Save();
        SceneManager.LoadScene(gameSceneName);
    }

    public void StartHard()
    {
        PlayerPrefs.SetString("Difficulty", "Hard");
        PlayerPrefs.Save();
        SceneManager.LoadScene(gameSceneName);
    }
}