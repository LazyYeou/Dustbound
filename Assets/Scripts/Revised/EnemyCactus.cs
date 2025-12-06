using UnityEngine;
using System.Collections;

public class EnemyCactus : MonoBehaviour
{
    [Header("Stats")]
    public float health = 50f;
    public float maxHealth = 50f;
    public float damageToPlayer = 20f;
    public GameObject expGemPrefab;

    [Header("Mole Behavior")]
    public float hideTime = 2f; // Time underground
    public float appearTime = 3f; // Time above ground
    public float anticipationTime = 0.5f; // Warning time before appearing
    public float detectionRange = 10f; // Range to detect player
    public float attackRadius = 2f; // Damage radius when above ground

    [Header("Animation Settings")]
    public float appearDuration = 0.3f;
    public float hideDuration = 0.3f;
    public AnimationCurve appearCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve hideCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Visual Effects")]
    public GameObject warningIndicator; // Prefab that shows where mole will appear
    public GameObject dustEffectPrefab; // Dust when appearing/hiding
    public SpriteRenderer spriteRenderer;
    public GameObject shadowSprite; // Optional ground shadow
    public Material flashMaterial;
    private Material originalMaterial;

    [Header("Sprite Settings")]
    public Sprite undergroundSprite; // Optional: different sprite when underground
    public Sprite aboveGroundSprite;
    public bool flipTowardsPlayer = true;

    [Header("Death Settings")]
    public GameObject deathFxPrefab;
    public bool useDeathAnimation = true;
    public float deathAnimationDuration = 0.5f;

    [Header("Damage Popup")]
    public GameObject damagePopupPrefab;

    private Transform player;
    private bool isAboveGround = false;
    private bool isTransitioning = false;
    private bool isDying = false;
    private bool isVulnerable = false; // Can only be hit when above ground
    private Color originalColor;
    private Vector3 originalScale;
    private Vector3 undergroundOffset = new Vector3(0, -1f, 0);
    private Vector3 originalPosition;
    private GameObject currentWarning;

    private enum MoleState
    {
        Underground,
        Anticipating,
        Appearing,
        AboveGround,
        Hiding
    }
    private MoleState currentState = MoleState.Underground;

    void Start()
    {
        // Find player
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;

        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
        originalColor = spriteRenderer.color;
        originalScale = transform.localScale;
        originalPosition = transform.position;
        maxHealth = health;

        // Start underground
        StartCoroutine(MoleBehaviorLoop());
    }

    void Update()
    {
        if (isDying || player == null) return;

        // Flip sprite towards player when above ground
        if (isAboveGround && flipTowardsPlayer)
        {
            FlipTowardsPlayer();
        }

        // Check for damage to player when above ground
        if (isAboveGround && !isTransitioning)
        {
            CheckPlayerProximity();
        }
    }

    IEnumerator MoleBehaviorLoop()
    {
        while (!isDying)
        {
            // Wait underground
            currentState = MoleState.Underground;
            isVulnerable = false;
            yield return new WaitForSeconds(hideTime);

            // Check if player is in range before appearing
            if (player != null && Vector2.Distance(transform.position, player.position) <= detectionRange)
            {
                // Move to player position (underground)
                yield return StartCoroutine(MoveToPlayerPosition());

                // Show anticipation warning
                currentState = MoleState.Anticipating;
                yield return StartCoroutine(ShowAnticipation());

                // Appear from ground
                currentState = MoleState.Appearing;
                yield return StartCoroutine(AppearFromGround());

                // Stay above ground
                currentState = MoleState.AboveGround;
                isAboveGround = true;
                isVulnerable = true;
                yield return new WaitForSeconds(appearTime);

                // Hide back underground
                currentState = MoleState.Hiding;
                yield return StartCoroutine(HideUnderground());
                isAboveGround = false;
            }
            else
            {
                // Player not in range, wait a bit more
                yield return new WaitForSeconds(1f);
            }
        }
    }

    IEnumerator MoveToPlayerPosition()
    {
        if (player == null) yield break;

        // Instantly teleport to near player position while underground
        Vector3 targetPos = player.position;
        transform.position = targetPos;
        originalPosition = targetPos;
    }

    IEnumerator ShowAnticipation()
    {
        // Show warning indicator
        if (warningIndicator != null)
        {
            currentWarning = Instantiate(warningIndicator, transform.position, Quaternion.identity);
        }

        yield return new WaitForSeconds(anticipationTime);

        // Remove warning
        if (currentWarning != null)
        {
            Destroy(currentWarning);
        }
    }

    IEnumerator AppearFromGround()
    {
        isTransitioning = true;

        // Spawn dust effect
        if (dustEffectPrefab != null)
        {
            Instantiate(dustEffectPrefab, transform.position, Quaternion.identity);
        }

        // Change sprite
        if (aboveGroundSprite != null)
        {
            spriteRenderer.sprite = aboveGroundSprite;
        }

        // Animate rising from ground
        float elapsed = 0f;
        Vector3 startPos = originalPosition + undergroundOffset;
        Vector3 endPos = originalPosition;
        transform.position = startPos;

        Vector3 startScale = originalScale * 0.5f;
        transform.localScale = startScale;

        while (elapsed < appearDuration)
        {
            elapsed += Time.deltaTime;
            float t = appearCurve.Evaluate(elapsed / appearDuration);

            transform.position = Vector3.Lerp(startPos, endPos, t);
            transform.localScale = Vector3.Lerp(startScale, originalScale, t);

            yield return null;
        }

        transform.position = endPos;
        transform.localScale = originalScale;

        // Show shadow
        if (shadowSprite != null)
        {
            shadowSprite.SetActive(true);
        }

        isTransitioning = false;
    }

    IEnumerator HideUnderground()
    {
        isTransitioning = true;
        isVulnerable = false;

        // Spawn dust effect
        if (dustEffectPrefab != null)
        {
            Instantiate(dustEffectPrefab, transform.position, Quaternion.identity);
        }

        // Animate descending into ground
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Vector3 endPos = originalPosition + undergroundOffset;

        while (elapsed < hideDuration)
        {
            elapsed += Time.deltaTime;
            float t = hideCurve.Evaluate(elapsed / hideDuration);

            transform.position = Vector3.Lerp(startPos, endPos, t);
            transform.localScale = Vector3.Lerp(originalScale, originalScale * 0.5f, t);

            yield return null;
        }

        // Change sprite back
        if (undergroundSprite != null)
        {
            spriteRenderer.sprite = undergroundSprite;
        }

        // Hide shadow
        if (shadowSprite != null)
        {
            shadowSprite.SetActive(false);
        }

        isTransitioning = false;
    }

    void FlipTowardsPlayer()
    {
        if (player == null) return;

        bool shouldFaceRight = player.position.x > transform.position.x;
        spriteRenderer.flipX = !shouldFaceRight;
    }

    void CheckPlayerProximity()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRadius)
        {
            PlayerStats stats = player.GetComponent<PlayerStats>();
            if (stats != null)
            {
                stats.TakeDamage(damageToPlayer * Time.deltaTime);
            }
        }
    }

    public void TakeHit(int damage, bool isCrit, Vector2 knockbackDir, float knockbackForce)
    {
        // Can only be damaged when above ground
        if (!isVulnerable || isDying) return;

        health -= damage;

        ShowDamagePopup(damage, isCrit);
        StartCoroutine(FlashRoutine());

        if (health <= 0)
        {
            Die();
        }
    }

    void ShowDamagePopup(int amount, bool isCrit)
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
        if (isDying) return;
        isDying = true;

        StopAllCoroutines();

        // Spawn exp gem
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
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;
        Color startColor = spriteRenderer.color;

        while (elapsed < deathAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / deathAnimationDuration;

            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            spriteRenderer.color = Color.Lerp(startColor, new Color(startColor.r, startColor.g, startColor.b, 0), t);
            transform.Rotate(0, 0, 720f * Time.deltaTime);

            yield return null;
        }

        if (deathFxPrefab != null)
        {
            Instantiate(deathFxPrefab, transform.position, transform.rotation);
        }

        Destroy(gameObject);
    }

    // Visualize ranges in editor
    void OnDrawGizmosSelected()
    {
        // Detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Attack radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}