using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 2f;
    public float lifetime = 5f;
    public int damage = 1;

    void Setup(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        Destroy(gameObject, lifetime);

        GetComponent<Rigidbody2D>().linearVelocity = dir * bulletSpeed;
    }

    void OollisionEnter2D(Collision2D collision, Collider2D collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}
