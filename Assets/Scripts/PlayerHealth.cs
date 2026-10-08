using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public Slider healthSlider;

    public float shakeDuration = 0.15f;
    public float shakeMagnitude = 0.08f;

    private int currentHealth;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthSlider == null)
        {
            GameObject healthObj = GameObject.Find("HealthBar");

            if (healthObj != null)
            {
                healthSlider = healthObj.GetComponent<Slider>();
            }
        }

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        UpdateHealthBar();

        CameraShake cameraShake = FindFirstObjectByType<CameraShake>();

        if (cameraShake != null)
        {
            cameraShake.TriggerShake(shakeDuration, shakeMagnitude);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (isDead)
            return;

        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }

    void Die()
    {
        isDead = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }
}