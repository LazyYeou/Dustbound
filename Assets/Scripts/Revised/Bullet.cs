using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 10f;
    public float critChance = 0.2f;
    public float critMultiplier = 2f;
    public float knockbackForce = 5f;

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
            bool isCrit = Random.value < critChance;
            float finalDamage = damage;
            if (isCrit) finalDamage = Mathf.RoundToInt(damage * critMultiplier);

            Vector2 knockbackDir = rb.linearVelocity.normalized;

            EnemyController enemy = col.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeHit(finalDamage, isCrit, knockbackDir, knockbackForce);
            }

            Destroy(gameObject);
        }
    }
}