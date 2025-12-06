using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    public float health = 50f;
    public float maxHealth = 50f;
    public float speed = 3f;
    public float damageToPlayer = 10f;
    public GameObject expGemPrefab;

    [Header("Juice References")]
    public GameObject damagePopupPrefab;
    public Material flashMaterial;
    private Material originalMaterial;

    [Header("Sprite Flipping")]
    public bool autoFlipTowardsPlayer = true;
    public bool flipX = true; // If true, flip on X axis. If false, flip using scale
    public float flipSmoothness = 0.1f; // For smooth rotation (if not using flipX)

    [Header("Collision Settings")]
    public float damageRadius = 1f;

    [Header("Death Animation Settings")]
    [SerializeField] private bool useDeathAnimation = true;
    [SerializeField] private DeathAnimationType deathAnimationType = DeathAnimationType.FadeAndShrink;
    [SerializeField] private float deathAnimationDuration = 0.5f;
    [SerializeField] private AnimationCurve deathCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Death Effects")]
    [SerializeField] private bool flashOnDeath = true;
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private int flashCount = 3;
    [SerializeField] private float flashSpeed = 0.1f;

    [SerializeField] private bool knockbackOnDeath = true;
    [SerializeField] private float deathKnockbackForce = 5f;

    [SerializeField] private bool rotateOnDeath = true;
    [SerializeField] private float rotationSpeed = 720f;

    [SerializeField] private GameObject deathFxPrefab;

    [Header("Hit Feedback")]
    [SerializeField] private bool shakeOnHit = true;
    [SerializeField] private float shakeIntensity = 0.1f;

    [Header("Attack Settings")]
    public float attackInterval = 0.5f; // How often to attack (in seconds)
    private float attackTimer = 0f;     // Internal timer to track cooldown

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isKnockedBack = false;
    private bool isDying = false;
    private Color originalColor;
    private Vector3 originalScale;
    private bool isFacingRight = true;

    public enum DeathAnimationType
    {
        FadeOnly,
        ShrinkOnly,
        FadeAndShrink,
        Explode,
        Dissolve,
        Splat
    }

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
        originalColor = spriteRenderer.color;
        originalScale = transform.localScale;

        maxHealth = health;
    }

    void FixedUpdate()
    {
        if (player == null || isDying) return;
        if (isKnockedBack) return;

        if (attackTimer > 0)
        {
            attackTimer -= Time.fixedDeltaTime;
        }

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;

        // Handle sprite flipping
        if (autoFlipTowardsPlayer)
        {
            FlipTowardsPlayer();
        }

        CheckPlayerProximity();
    }

    void FlipTowardsPlayer()
    {
        if (player == null) return;

        // Determine if player is to the right or left
        bool shouldFaceRight = player.position.x > transform.position.x;

        // Only flip if direction changed
        if (shouldFaceRight != isFacingRight)
        {
            isFacingRight = shouldFaceRight;

            if (!flipX)
            {
                // Flip using SpriteRenderer.flipX
                spriteRenderer.flipX = !isFacingRight;
            }
            else
            {
                // Flip using scale (alternative method)
                Vector3 scale = transform.localScale;
                scale.x = isFacingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
                transform.localScale = scale;
            }
        }
    }

    // Manual flip methods (if you want to control flipping from other scripts)
    public void SetFacingDirection(bool faceRight)
    {
        isFacingRight = faceRight;
        if (flipX)
        {
            spriteRenderer.flipX = !faceRight;
        }
        else
        {
            Vector3 scale = transform.localScale;
            scale.x = faceRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    // --- DAMAGE LOGIC ---
    public void TakeHit(float damage, bool isCrit, Vector2 knockbackDir, float knockbackForce)
    {
        if (isDying) return;

        health -= damage;

        ShowDamagePopup(damage, isCrit);
        Debug.Log(health);
        StartCoroutine(FlashRoutine());

        if (shakeOnHit)
        {
            StartCoroutine(HitShake());
        }

        if (knockbackForce > 0f)
            StartCoroutine(KnockbackRoutine(knockbackDir, knockbackForce));

        if (health <= 0)
            Die();
    }

    void ShowDamagePopup(float amount, bool isCrit)
    {
        if (damagePopupPrefab)
        {
            GameObject popup = Instantiate(damagePopupPrefab, transform.position, Quaternion.identity);
            DamagePopup popupScript = popup.GetComponent<DamagePopup>();
            if (popupScript != null)
            {
                popupScript.Setup(amount, isCrit);
            }
        }
    }

    IEnumerator KnockbackRoutine(Vector2 dir, float force)
    {
        isKnockedBack = true;

        rb.linearVelocity = Vector2.zero;
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

    IEnumerator HitShake()
    {
        Vector3 originalPos = transform.position;
        float elapsed = 0f;
        float duration = 0.15f;

        while (elapsed < duration)
        {
            float x = Random.Range(-shakeIntensity, shakeIntensity);
            float y = Random.Range(-shakeIntensity, shakeIntensity);
            transform.position = originalPos + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;
    }

    void Die()
    {
        if (isDying) return;
        isDying = true;

        // Stop movement
        rb.linearVelocity = Vector2.zero;
        // rb.isKinematic = true;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (expGemPrefab != null)
        {
            Instantiate(expGemPrefab, transform.position, Quaternion.identity);
        }

        if (useDeathAnimation)
        {
            StartCoroutine(PlayDeathAnimation());
        }
        else
        {
            InstantDeath();
        }
    }

    void InstantDeath()
    {
        if (deathFxPrefab != null)
        {
            Instantiate(deathFxPrefab, transform.position, transform.rotation);
        }
        Destroy(gameObject);
    }

    IEnumerator PlayDeathAnimation()
    {
        if (flashOnDeath)
        {
            yield return StartCoroutine(DeathFlash());
        }

        // Apply knockback
        if (knockbackOnDeath && player != null)
        {
            Vector2 knockbackDir = (transform.position - player.position).normalized;
            // rb.isKinematic = false;
            rb.AddForce(knockbackDir * deathKnockbackForce, ForceMode2D.Impulse);
            yield return new WaitForSeconds(0.1f);
            rb.linearVelocity = Vector2.zero;
            // rb.isKinematic = true;
        }

        switch (deathAnimationType)
        {
            case DeathAnimationType.FadeOnly:
                yield return StartCoroutine(FadeOutAnimation());
                break;
            case DeathAnimationType.ShrinkOnly:
                yield return StartCoroutine(ShrinkAnimation());
                break;
            case DeathAnimationType.FadeAndShrink:
                StartCoroutine(FadeOutAnimation());
                yield return StartCoroutine(ShrinkAnimation());
                break;
            case DeathAnimationType.Explode:
                yield return StartCoroutine(ExplodeAnimation());
                break;
            case DeathAnimationType.Dissolve:
                yield return StartCoroutine(DissolveAnimation());
                break;
            case DeathAnimationType.Splat:
                yield return StartCoroutine(SplatAnimation());
                break;
        }

        // Spawn death effect
        if (deathFxPrefab != null)
        {
            Instantiate(deathFxPrefab, transform.position, transform.rotation);
        }

        Destroy(gameObject);
    }

    IEnumerator DeathFlash()
    {
        Color currentColor = spriteRenderer.color;

        for (int i = 0; i < flashCount; i++)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashSpeed);
            spriteRenderer.color = currentColor;
            yield return new WaitForSeconds(flashSpeed);
        }
    }

    IEnumerator FadeOutAnimation()
    {
        float elapsed = 0f;
        Color startColor = spriteRenderer.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0);

        while (elapsed < deathAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = deathCurve.Evaluate(elapsed / deathAnimationDuration);
            spriteRenderer.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }
    }

    IEnumerator ShrinkAnimation()
    {
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;

        while (elapsed < deathAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = deathCurve.Evaluate(elapsed / deathAnimationDuration);
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);

            if (rotateOnDeath)
            {
                transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
            }

            yield return null;
        }
    }

    IEnumerator ExplodeAnimation()
    {
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 explosionScale = startScale * 1.5f;

        // Quick expand
        while (elapsed < deathAnimationDuration * 0.3f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (deathAnimationDuration * 0.3f);
            transform.localScale = Vector3.Lerp(startScale, explosionScale, t);
            yield return null;
        }

        // Rapid shrink with fade
        elapsed = 0f;
        Color startColor = spriteRenderer.color;
        while (elapsed < deathAnimationDuration * 0.7f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (deathAnimationDuration * 0.7f);
            transform.localScale = Vector3.Lerp(explosionScale, Vector3.zero, t);
            spriteRenderer.color = Color.Lerp(startColor, new Color(startColor.r, startColor.g, startColor.b, 0), t);

            if (rotateOnDeath)
            {
                transform.Rotate(0, 0, rotationSpeed * Time.deltaTime * 2);
            }

            yield return null;
        }
    }

    IEnumerator DissolveAnimation()
    {
        float elapsed = 0f;
        Color startColor = spriteRenderer.color;

        while (elapsed < deathAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / deathAnimationDuration;

            // Fade out
            spriteRenderer.color = Color.Lerp(startColor, new Color(startColor.r, startColor.g, startColor.b, 0), t);

            // Scale down slightly
            transform.localScale = Vector3.Lerp(originalScale, originalScale * 0.7f, t);

            // Move down
            transform.position += Vector3.down * Time.deltaTime * 2f;

            yield return null;
        }
    }

    IEnumerator SplatAnimation()
    {
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 splatScale = new Vector3(startScale.x * 1.5f, startScale.y * 0.2f, startScale.z);

        while (elapsed < deathAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = deathCurve.Evaluate(elapsed / deathAnimationDuration);

            // Squash
            transform.localScale = Vector3.Lerp(startScale, splatScale, t);

            // Fade
            Color color = spriteRenderer.color;
            spriteRenderer.color = new Color(color.r, color.g, color.b, 1 - t);

            yield return null;
        }
    }

    // --- PLAYER DAMAGE ---
    // void OnCollisionStay2D(Collision2D collision)
    // {
    //     if (isDying) return;

    //     if (collision.collider.CompareTag("Player"))
    //     {
    //         PlayerStats stats = collision.collider.GetComponent<PlayerStats>();
    //         if (stats != null)
    //         {
    //             stats.TakeDamage(damageToPlayer * Time.deltaTime);
    //         }
    //     }
    // }

    void CheckPlayerProximity()
    {
        if (player == null || isDying) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= damageRadius)
        {
            if (attackTimer <= 0)
            {
                PlayerStats stats = player.GetComponent<PlayerStats>();
                if (stats != null)
                {
                    stats.TakeDamage(damageToPlayer);

                    attackTimer = attackInterval;
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}