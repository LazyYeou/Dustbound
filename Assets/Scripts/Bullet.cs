using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 2f;
    public float lifetime = 5f;
    public int damage = 1;

    public void Setup(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        Destroy(gameObject, lifetime);

        GetComponent<Rigidbody2D>().linearVelocity = dir * bulletSpeed;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            Debug.Log("HIT");
            Destroy(gameObject);
        }
    }
}
