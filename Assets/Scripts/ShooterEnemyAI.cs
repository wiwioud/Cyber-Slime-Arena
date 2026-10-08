using UnityEngine;
using TMPro;

public class ShooterEnemyAI : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float detectionRange = 7f;
    public float shootingRange = 5.5f;
    public float keepDistance = 3f;

    public float shootCooldown = 1.5f;
    public float projectileForce = 10f;

    public GameObject enemyProjectilePrefab;

    private Transform player;
    private SpriteRenderer spriteRenderer;
    private TextMeshPro stateIcon;

    private float lastShootTime;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        spriteRenderer = GetComponent<SpriteRenderer>();

        Transform iconObj = transform.Find("StateIcon");
        if (iconObj != null)
        {
            stateIcon = iconObj.GetComponent<TextMeshPro>();
        }
    }

    void Update()
    {
        if (player == null)
            return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > detectionRange)
        {
            SetVisual("", new Color(0.4f, 0.4f, 1f));
            return;
        }

        if (distance < keepDistance)
        {
            SetVisual("BACK", Color.cyan);
            MoveAwayFromPlayer();
        }
        else if (distance <= shootingRange)
        {
            SetVisual(">", Color.magenta);
            ShootAtPlayer();
        }
        else
        {
            SetVisual("!", Color.yellow);
            MoveTowardsPlayer();
        }
    }

    void MoveTowardsPlayer()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            moveSpeed * Time.deltaTime
        );
    }

    void MoveAwayFromPlayer()
    {
        Vector2 directionAway = ((Vector2)transform.position - (Vector2)player.position).normalized;

        if (directionAway == Vector2.zero)
        {
            directionAway = Random.insideUnitCircle.normalized;
        }

        transform.position += (Vector3)(directionAway * moveSpeed * Time.deltaTime);
    }

    void ShootAtPlayer()
    {
        if (enemyProjectilePrefab == null)
            return;

        if (Time.time < lastShootTime + shootCooldown)
            return;

        Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;

        GameObject projectile = Instantiate(
            enemyProjectilePrefab,
            transform.position,
            Quaternion.identity
        );

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.AddForce(direction * projectileForce, ForceMode2D.Impulse);
        }

        lastShootTime = Time.time;
    }

    void SetVisual(string icon, Color color)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }

        if (stateIcon != null)
        {
            stateIcon.text = icon;
            stateIcon.color = color;
        }
    }
}