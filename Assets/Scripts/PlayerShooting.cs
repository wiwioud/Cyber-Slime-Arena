using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class PlayerShooting : MonoBehaviour
{
    public Transform firePoint;
    public GameObject projectilePrefab;
    public float projectileForce = 20f;
    public AudioSource shootSound;

    public int level2Score = 50;
    public int level3Score = 120;
    public int level4Score = 250;

    private int weaponLevel = 1;
    private float nextFireTime = 0f;
    private float fireCooldown = 0.35f;

    private TMP_Text scoreTMP;
    private TMP_Text weaponTMP;

    private Text scoreLegacyText;
    private Text weaponLegacyText;

    void Start()
    {
        GameObject scoreObj = GameObject.Find("ScoreText");
        GameObject weaponObj = GameObject.Find("WeaponText");

        if (scoreObj != null)
        {
            scoreTMP = scoreObj.GetComponent<TMP_Text>();
            scoreLegacyText = scoreObj.GetComponent<Text>();
        }

        if (weaponObj != null)
        {
            weaponTMP = weaponObj.GetComponent<TMP_Text>();
            weaponLegacyText = weaponObj.GetComponent<Text>();
        }

        UpdateWeaponLevel();
        UpdateWeaponText();
    }

    void Update()
    {
        UpdateWeaponLevel();

        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireCooldown;
        }
    }

    void UpdateWeaponLevel()
    {
        int score = ReadCurrentScore();
        int newLevel = 1;

        if (score >= level4Score)
        {
            newLevel = 4;
        }
        else if (score >= level3Score)
        {
            newLevel = 3;
        }
        else if (score >= level2Score)
        {
            newLevel = 2;
        }

        if (newLevel != weaponLevel)
        {
            weaponLevel = newLevel;
            UpdateWeaponText();
        }

        if (weaponLevel == 1)
        {
            fireCooldown = 0.35f;
        }
        else if (weaponLevel == 2)
        {
            fireCooldown = 0.22f;
        }
        else if (weaponLevel == 3)
        {
            fireCooldown = 0.25f;
        }
        else if (weaponLevel == 4)
        {
            fireCooldown = 0.18f;
        }
    }

    int ReadCurrentScore()
    {
        string scoreString = "";

        if (scoreTMP != null)
        {
            scoreString = scoreTMP.text;
        }
        else if (scoreLegacyText != null)
        {
            scoreString = scoreLegacyText.text;
        }

        string numbersOnly = Regex.Replace(scoreString, @"[^\d]", "");

        if (int.TryParse(numbersOnly, out int result))
        {
            return result;
        }

        return 0;
    }

    void Shoot()
    {
        if (shootSound != null)
        {
            shootSound.Play();
        }

        if (weaponLevel == 1)
        {
            CreateProjectile(0f);
        }
        else if (weaponLevel == 2)
        {
            CreateProjectile(0f);
        }
        else if (weaponLevel == 3)
        {
            CreateProjectile(-8f);
            CreateProjectile(8f);
        }
        else if (weaponLevel == 4)
        {
            CreateProjectile(-15f);
            CreateProjectile(0f);
            CreateProjectile(15f);
        }
    }

    void CreateProjectile(float angleOffset)
    {
        Quaternion rotation = firePoint.rotation * Quaternion.Euler(0f, 0f, angleOffset);

        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            rotation
        );

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            Vector2 shootDirection = rotation * Vector2.up;
            rb.AddForce(shootDirection * projectileForce, ForceMode2D.Impulse);
        }
    }

    void UpdateWeaponText()
    {
        string text = "Weapon: Level " + weaponLevel;

        if (weaponTMP != null)
        {
            weaponTMP.text = text;

            if (weaponLevel == 1)
                weaponTMP.color = Color.cyan;
            else if (weaponLevel == 2)
                weaponTMP.color = Color.yellow;
            else if (weaponLevel == 3)
                weaponTMP.color = new Color(1f, 0.5f, 0f);
            else if (weaponLevel == 4)
                weaponTMP.color = Color.magenta;
        }

        if (weaponLegacyText != null)
        {
            weaponLegacyText.text = text;
        }
    }
}