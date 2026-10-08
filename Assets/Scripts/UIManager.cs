using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public Slider healthSlider;
    public Text scoreText;
    public GameObject gameOverPanel;
    public Text finalScoreText;
    public Text highscoreText;
    public Image hitFlashImage;

    void Awake() {
        if (Instance == null) Instance = this;
    }

    public void UpdateHealth(int currentHealth, int maxHealth) {
        if(healthSlider != null) {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    public void UpdateScore(int score) {
        if(scoreText != null) {
            scoreText.text = "Score: " + score;
        }
    }

    public void ShowGameOver(int finalScore) {
        if(gameOverPanel != null) {
            gameOverPanel.SetActive(true);
            if(finalScoreText != null) finalScoreText.text = "Score: " + finalScore;
            if(highscoreText != null) highscoreText.text = "Highscore: " + PlayerPrefs.GetInt("Highscore", 0);
        }
    }

    public void TriggerHitFlash() {
        if(hitFlashImage != null) {
            StartCoroutine(HitFlashRoutine());
        }
    }

    IEnumerator HitFlashRoutine() {
        Color c = hitFlashImage.color;
        c.a = 0.5f;
        hitFlashImage.color = c;
        yield return new WaitForSeconds(0.1f);
        c.a = 0f;
        hitFlashImage.color = c;
    }

    public void RestartButton() {
        if(GameManager.Instance != null) GameManager.Instance.RestartGame();
    }
}
