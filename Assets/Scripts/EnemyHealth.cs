using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int scoreValue = 10;
    public GameObject deathParticles;

    public GameObject healthPickupPrefab;
    public float healthDropChance = 0.35f;

    public GameObject speedBoostPickupPrefab;
    public float speedBoostDropChance = 0.15f;

    private int currentHealth;

    private Transform healthBarFill;
    private Vector3 originalHealthBarScale;

    public int CurrentHealth
    {
        get { return currentHealth; }
    }

    void Start()
    {
        currentHealth = maxHealth;

        healthBarFill = transform.Find("HealthBar_Fill");

        if (healthBarFill != null)
        {
            originalHealthBarScale = healthBarFill.localScale;
        }

        UpdateHealthBar();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (healthBarFill == null)
            return;

        float healthPercent = (float)currentHealth / maxHealth;

        healthBarFill.localScale = new Vector3(
            originalHealthBarScale.x * healthPercent,
            originalHealthBarScale.y,
            originalHealthBarScale.z
        );
    }

    void Die()
    {
        if (deathParticles != null)
        {
            Instantiate(deathParticles, transform.position, Quaternion.identity);
        }

        TryDropPowerUp();

        GameManager gm = FindFirstObjectByType<GameManager>();
        if (gm != null)
        {
            gm.AddScore(scoreValue);
        }

        Destroy(gameObject);
    }

    void TryDropPowerUp()
    {
        float randomValue = Random.value;

        if (healthPickupPrefab != null && randomValue <= healthDropChance)
        {
            Instantiate(healthPickupPrefab, transform.position, Quaternion.identity);
            return;
        }

        if (speedBoostPickupPrefab != null && randomValue <= healthDropChance + speedBoostDropChance)
        {
            Instantiate(speedBoostPickupPrefab, transform.position, Quaternion.identity);
        }
    }
}