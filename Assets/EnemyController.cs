using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    public float health = 50f;
    public float speed = 3f;
    public float damageToPlayer = 10f;  // Damage per second
    public GameObject expGemPrefab;

    [Header("Juice References")]
    public GameObject damagePopupPrefab;
    public Material flashMaterial;
    private Material originalMaterial;

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private bool isKnockedBack = false;

    void Start()
    {
        // --- FIND PLAYER ---
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;
        else
            Debug.LogError("ENEMY: Player NOT found. Make sure Player has tag 'Player'.");

        // --- COMPONENTS ---
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
    }

    void FixedUpdate()
    {
        if (player == null) return;
        if (isKnockedBack) return;

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;
    }

    // --- DAMAGE LOGIC ---
    public void TakeHit(int damage, bool isCrit, Vector2 knockbackDir, float knockbackForce)
    {
        health -= damage;

        ShowDamagePopup(damage, isCrit);
        StartCoroutine(FlashRoutine());

        if (knockbackForce > 0f)
            StartCoroutine(KnockbackRoutine(knockbackDir, knockbackForce));

        if (health <= 0)
            Die();
    }

    void ShowDamagePopup(int amount, bool isCrit)
    {
        if (damagePopupPrefab)
        {
            GameObject popup = Instantiate(damagePopupPrefab, transform.position, Quaternion.identity);
            popup.GetComponent<DamagePopup>().Setup(amount, isCrit);
        }
    }

    IEnumerator KnockbackRoutine(Vector2 dir, float force)
    {
        isKnockedBack = true;

        rb.linearVelocity = Vector2.zero;  // stop movement
        rb.AddForce(dir * force, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.15f);

        isKnockedBack = false;
    }

    IEnumerator FlashRoutine()
    {
        if (flashMaterial != null)
        {
            spriteRenderer.material = flashMaterial;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.material = originalMaterial;
        }
    }

    void Die()
    {
        if (expGemPrefab != null)
            Instantiate(expGemPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    // --- PLAYER DAMAGE ---
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            PlayerStats stats = collision.collider.GetComponent<PlayerStats>();
            if (stats != null)
            {
                // Damage per second
                stats.TakeDamage(damageToPlayer * Time.deltaTime);
            }
        }
    }
}
