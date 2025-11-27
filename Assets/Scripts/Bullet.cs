using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 10f;
    public float critChance = 0.2f; // 20% Chance
    public float critMultiplier = 2f;
    public float knockbackForce = 5f; // How hard to push

    private Rigidbody2D rb;

    public void Setup(Vector2 dir)
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = dir * speed;
        Destroy(gameObject, 3f);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            // 1. Calculate Crit
            bool isCrit = Random.value < critChance;
            float finalDamage = damage;
            if (isCrit) finalDamage = Mathf.RoundToInt(damage * critMultiplier);

            // 2. Calculate Knockback Direction (Direction bullet is moving)
            Vector2 knockbackDir = rb.linearVelocity.normalized;

            // 3. Apply to Enemy
            EnemyController enemy = col.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeHit(finalDamage, isCrit, knockbackDir, knockbackForce);
            }

            Destroy(gameObject);
        }
    }
}