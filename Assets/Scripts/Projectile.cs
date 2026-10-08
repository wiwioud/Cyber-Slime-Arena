using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 10;
    public GameObject hitEffect;

    void OnTriggerEnter2D(Collider2D hitInfo) {
        if(hitInfo.CompareTag("Player") || hitInfo.CompareTag("Projectile")) return;
        
        EnemyHealth enemy = hitInfo.GetComponent<EnemyHealth>();
        if (enemy != null) {
            enemy.TakeDamage(damage);
        }

        if(hitEffect != null) {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }
        
        Destroy(gameObject);
    }
}
