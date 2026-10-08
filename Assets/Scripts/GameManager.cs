using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Transform[] spawnPoints;

    public GameObject normalEnemyPrefab;
    public GameObject fastEnemyPrefab;
    public GameObject backupEnemyPrefab;
    public GameObject shooterEnemyPrefab;

    public float timeBetweenWaves = 2f;
    public float timeBetweenSpawns = 0.6f;

    public int startEnemyCount = 3;
    public int extraEnemiesPerWave = 2;

    public int winWave = 5;

    private int score = 0;
    private int waveNumber = 0;

    private bool waveRunning = false;
    private bool gameIsOver = false;

    private TMP_Text scoreTMP;
    private TMP_Text waveTMP;

    private Text scoreLegacyText;
    private Text waveLegacyText;

    private GameObject gameOverPanel;

    void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
    }

    void Start()
    {
        ApplyDifficulty();

        FindUITexts();

        gameOverPanel = FindObjectByNameIncludingInactive("GameOverPanel");

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        UpdateScoreText();
        UpdateWaveText("Wave: 0");

        StartCoroutine(WaveLoop());
    }

    void ApplyDifficulty()
    {
        string difficulty = PlayerPrefs.GetString("Difficulty", "Normal");

        if (difficulty == "Easy")
        {
            startEnemyCount = 2;
            extraEnemiesPerWave = 1;
            timeBetweenSpawns = 0.9f;
            timeBetweenWaves = 3f;
        }
        else if (difficulty == "Hard")
        {
            startEnemyCount = 4;
            extraEnemiesPerWave = 3;
            timeBetweenSpawns = 0.4f;
            timeBetweenWaves = 1.5f;
        }
        else
        {
            startEnemyCount = 3;
            extraEnemiesPerWave = 2;
            timeBetweenSpawns = 0.6f;
            timeBetweenWaves = 2f;
        }

        Debug.Log("Difficulty: " + difficulty);
    }

    IEnumerator WaveLoop()
    {
        yield return new WaitForSeconds(1f);

        while (!gameIsOver)
        {
            if (CountAliveEnemies() == 0 && !waveRunning)
            {
                yield return StartCoroutine(StartWave());
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator StartWave()
    {
        waveRunning = true;

        waveNumber++;

        UpdateWaveText("Wave: " + waveNumber);

        int enemiesToSpawn = startEnemyCount + ((waveNumber - 1) * extraEnemiesPerWave);

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        waveRunning = false;

        while (CountAliveEnemies() > 0 && !gameIsOver)
        {
            yield return new WaitForSeconds(0.5f);
        }

        if (!gameIsOver)
        {
            if (waveNumber >= winWave)
            {
                WinGame();
                yield break;
            }

            UpdateWaveText("Wave cleared!");
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    void SpawnEnemy()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points assigned!");
            return;
        }

        if (normalEnemyPrefab == null)
        {
            Debug.LogWarning("Normal Enemy Prefab is missing!");
            return;
        }

        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject enemyToSpawn = normalEnemyPrefab;

        float randomValue = Random.value;

        if (waveNumber >= 5 && shooterEnemyPrefab != null && randomValue < 0.20f)
        {
            enemyToSpawn = shooterEnemyPrefab;
        }
        else if (waveNumber >= 4 && backupEnemyPrefab != null && randomValue < 0.35f)
        {
            enemyToSpawn = backupEnemyPrefab;
        }
        else if (waveNumber >= 3 && fastEnemyPrefab != null && randomValue < 0.60f)
        {
            enemyToSpawn = fastEnemyPrefab;
        }

        Instantiate(enemyToSpawn, randomSpawnPoint.position, Quaternion.identity);
    }

    int CountAliveEnemies()
    {
        EnemyHealth[] enemies = FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None);
        return enemies.Length;
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreText();
    }

    void FindUITexts()
    {
        GameObject scoreObj = GameObject.Find("ScoreText");
        GameObject waveObj = GameObject.Find("WaveText");

        if (scoreObj != null)
        {
            scoreTMP = scoreObj.GetComponent<TMP_Text>();
            scoreLegacyText = scoreObj.GetComponent<Text>();
        }

        if (waveObj != null)
        {
            waveTMP = waveObj.GetComponent<TMP_Text>();
            waveLegacyText = waveObj.GetComponent<Text>();
        }
    }

    void UpdateScoreText()
    {
        string text = "Score: " + score;

        if (scoreTMP != null)
            scoreTMP.text = text;

        if (scoreLegacyText != null)
            scoreLegacyText.text = text;
    }

    void UpdateWaveText(string text)
    {
        if (waveTMP != null)
            waveTMP.text = text;

        if (waveLegacyText != null)
            waveLegacyText.text = text;
    }

    public void GameOver()
    {
        gameIsOver = true;

        StopAllCoroutines();

        ShowEndPanel("GAME OVER");
    }

    public void WinGame()
    {
        gameIsOver = true;

        StopAllCoroutines();

        ShowEndPanel("YOU WIN!");
    }

    void ShowEndPanel(string title)
    {
        int savedHighscore = PlayerPrefs.GetInt("Highscore", 0);

        if (score > savedHighscore)
        {
            savedHighscore = score;
            PlayerPrefs.SetInt("Highscore", savedHighscore);
            PlayerPrefs.Save();
        }

        SetTextObject("GameOverText", title);
        SetTextObject("FinalScoreText", "Final Score: " + score);
        SetTextObject("GameOverHighscoreText", "Highscore: " + savedHighscore);

        if (gameOverPanel == null)
        {
            gameOverPanel = FindObjectByNameIncludingInactive("GameOverPanel");
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        CameraShake cameraShake = FindFirstObjectByType<CameraShake>();

        if (cameraShake != null)
        {
            cameraShake.StopShake();
        }

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void SetTextObject(string objectName, string value)
    {
        GameObject obj = FindObjectByNameIncludingInactive(objectName);

        if (obj == null)
            return;

        TMP_Text tmp = obj.GetComponent<TMP_Text>();
        Text legacyText = obj.GetComponent<Text>();

        if (tmp != null)
            tmp.text = value;

        if (legacyText != null)
            legacyText.text = value;
    }

    GameObject FindObjectByNameIncludingInactive(string objectName)
    {
        Transform[] allObjects = FindObjectsByType<Transform>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (Transform obj in allObjects)
        {
            if (obj.name == objectName)
            {
                return obj.gameObject;
            }
        }

        return null;
    }
}