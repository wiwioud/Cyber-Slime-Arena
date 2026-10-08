using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private float originalMoveSpeed;

    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 mousePos;
    public Camera cam;

    private Coroutine speedBoostCoroutine;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalMoveSpeed = moveSpeed;
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (cam != null)
        {
            mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);

        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
    }

    public void ActivateSpeedBoost(float multiplier, float duration)
    {
        if (speedBoostCoroutine != null)
        {
            StopCoroutine(speedBoostCoroutine);
        }

        speedBoostCoroutine = StartCoroutine(SpeedBoostRoutine(multiplier, duration));
    }

    IEnumerator SpeedBoostRoutine(float multiplier, float duration)
    {
        moveSpeed = originalMoveSpeed * multiplier;

        yield return new WaitForSeconds(duration);

        moveSpeed = originalMoveSpeed;
        speedBoostCoroutine = null;
    }
}