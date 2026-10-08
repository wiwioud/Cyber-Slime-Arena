using UnityEngine;

public class SpeedBoostPickup : MonoBehaviour
{
    public float speedMultiplier = 1.8f;
    public float boostDuration = 5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();

            if (playerMovement != null)
            {
                playerMovement.ActivateSpeedBoost(speedMultiplier, boostDuration);
                Destroy(gameObject);
            }
        }
    }
}