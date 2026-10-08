using UnityEngine;
using TMPro;

public class EnemyAI : MonoBehaviour
{
    public float speed = 2f;
    public float detectionRange = 6f;
    public float stopDistance = 0.8f;
    public float patrolRadius = 3f;

    public int damage = 10;
    public float attackCooldown = 1f;

    public float fleeHealthPercent = 0.5f;
    public float fleeSpeedMultiplier = 4f;

    public float enrageDistance = 4f;
    public float enragedSpeedMultiplier = 2f;
    public int enragedDamage = 20;

    public GameObject backupPrefab;
    public float backupSpawnDistance = 1.5f;

    public float searchDuration = 2.5f;

    private Transform player;
    private PlayerHealth playerHealth;
    private EnemyHealth enemyHealth;
    private SpriteRenderer spriteRenderer;
    private TextMeshPro stateIcon;

    private Vector2 startPosition;
    private Vector2 patrolTarget;

    private Vector2 lastKnownPlayerPosition;
    private bool hasLastKnownPosition = false;
    private float searchTimer;

    private float lastAttackTime;
    private bool isEnraged = false;
    private bool hasCalledBackup = false;

    enum EnemyState
    {
        Patrol,
        Chase,
        Attack,
        Flee,
        EnragedChase,
        Search
    }

    private EnemyState currentState;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
            playerHealth = playerObj.GetComponent<PlayerHealth>();
        }

        enemyHealth = GetComponent<EnemyHealth>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        Transform iconObj = transform.Find("StateIcon");
        if (iconObj != null)
        {
            stateIcon = iconObj.GetComponent<TextMeshPro>();
            stateIcon.text = "";
        }

        startPosition = transform.position;
        PickNewPatrolTarget();

        currentState = EnemyState.Patrol;
    }

    void Update()
    {
        if (player == null || enemyHealth == null)
            return;

        float distance = Vector2.Distance(transform.position, player.position);
        float healthPercent = (float)enemyHealth.CurrentHealth / enemyHealth.maxHealth;

        bool playerDetected = distance <= detectionRange;

        if (playerDetected)
        {
            lastKnownPlayerPosition = player.position;
            hasLastKnownPosition = true;
        }

        if (!isEnraged && healthPercent <= fleeHealthPercent)
        {
            currentState = EnemyState.Flee;

            if (!hasCalledBackup)
            {
                CallBackup();
                hasCalledBackup = true;
            }

            if (distance >= enrageDistance)
            {
                isEnraged = true;
                damage = enragedDamage;
                currentState = EnemyState.EnragedChase;
            }
        }
        else if (isEnraged)
        {
            if (distance <= stopDistance + 0.2f)
                currentState = EnemyState.Attack;
            else
                currentState = EnemyState.EnragedChase;
        }
        else if (distance <= stopDistance + 0.2f)
        {
            currentState = EnemyState.Attack;
        }
        else if (playerDetected)
        {
            currentState = EnemyState.Chase;
        }
        else
        {
            if (currentState != EnemyState.Search && hasLastKnownPosition)
            {
                currentState = EnemyState.Search;
                searchTimer = searchDuration;
            }
            else if (currentState != EnemyState.Search)
            {
                currentState = EnemyState.Patrol;
            }
        }

        switch (currentState)
        {
            case EnemyState.Patrol:
                SetColor(Color.green);
                SetStateIcon("", Color.white);
                Patrol();
                break;

            case EnemyState.Chase:
                SetColor(Color.yellow);
                SetStateIcon("!", Color.yellow);
                ChasePlayer();
                break;

            case EnemyState.Attack:
                SetColor(isEnraged ? Color.magenta : Color.red);
                SetStateIcon("X", Color.red);
                AttackPlayer();
                break;

            case EnemyState.Flee:
                SetColor(Color.blue);
                SetStateIcon("RUN", Color.cyan);
                FleeFromPlayer();
                break;

            case EnemyState.EnragedChase:
                SetColor(Color.magenta);
                SetStateIcon("!!", Color.magenta);
                EnragedChasePlayer();
                break;

            case EnemyState.Search:
                SetColor(new Color(1f, 0.5f, 0f));
                SetStateIcon("?", new Color(1f, 0.5f, 0f));
                SearchLastKnownPosition();
                break;
        }
    }

    void Patrol()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            patrolTarget,
            speed * 0.5f * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, patrolTarget) < 0.2f)
        {
            PickNewPatrolTarget();
        }
    }

    void ChasePlayer()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            speed * Time.deltaTime
        );
    }

    void EnragedChasePlayer()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            speed * enragedSpeedMultiplier * Time.deltaTime
        );
    }

    void AttackPlayer()
    {
        if (playerHealth == null)
            return;

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            playerHealth.TakeDamage(damage);
            lastAttackTime = Time.time;
        }
    }

    void FleeFromPlayer()
    {
        Vector2 fleeDirection = ((Vector2)transform.position - (Vector2)player.position).normalized;

        if (fleeDirection == Vector2.zero)
        {
            fleeDirection = Random.insideUnitCircle.normalized;
        }

        transform.position += (Vector3)(fleeDirection * speed * fleeSpeedMultiplier * Time.deltaTime);
    }

    void SearchLastKnownPosition()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            lastKnownPlayerPosition,
            speed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, lastKnownPlayerPosition) < 0.2f)
        {
            searchTimer -= Time.deltaTime;

            if (searchTimer <= 0)
            {
                hasLastKnownPosition = false;
                PickNewPatrolTarget();
                currentState = EnemyState.Patrol;
            }
        }
    }

    void CallBackup()
    {
        if (backupPrefab == null)
            return;

        Vector2 randomOffset = Random.insideUnitCircle.normalized * backupSpawnDistance;
        Vector2 spawnPosition = (Vector2)transform.position + randomOffset;

        Instantiate(backupPrefab, spawnPosition, Quaternion.identity);
    }

    void PickNewPatrolTarget()
    {
        Vector2 randomDirection = Random.insideUnitCircle * patrolRadius;
        patrolTarget = startPosition + randomDirection;
    }

    void SetColor(Color color)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }
    }

    void SetStateIcon(string icon, Color color)
    {
        if (stateIcon != null)
        {
            stateIcon.text = icon;
            stateIcon.color = color;
        }
    }
}