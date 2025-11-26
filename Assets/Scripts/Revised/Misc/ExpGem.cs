using UnityEngine;

public class ExpGem : MonoBehaviour
{
    [Header("Experience Settings")]
    public float expAmount = 10f;

    [Header("Pickup Animation")]
    public float pickupRange = 3f; // Distance at which gem starts moving toward player
    public float moveSpeed = 8f; // Speed of movement toward player
    public float accelerationRate = 1.5f; // How much faster it gets as it approaches
    public AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Visual Effects")]
    public bool scaleOnApproach = true;
    public float minScale = 0.8f;
    public float maxScale = 1.2f;
    public float scaleSpeed = 3f;

    public bool rotateWhileMoving = true;
    public float rotationSpeed = 360f;


    [Header("Spawn Animation")]
    public bool spawnWithAnimation = true;
    public float spawnDuration = 0.3f;
    public float spawnHeight = 1f;

    private Transform player;
    private bool isMovingToPlayer = false;
    private bool isCollected = false;
    private Vector3 originalScale;
    private float currentSpeed;
    private float distanceToPlayer;

    void Start()
    {
        originalScale = transform.localScale;
        currentSpeed = moveSpeed;

        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null || isCollected) return;

        distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Check if player is in range
        if (distanceToPlayer <= pickupRange && !isMovingToPlayer)
        {
            StartMovingToPlayer();
        }

        // Move toward player
        if (isMovingToPlayer)
        {
            MoveTowardPlayer();

            if (scaleOnApproach)
            {
                AnimateScale();
            }

            if (rotateWhileMoving)
            {
                transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
            }
        }
    }

    void StartMovingToPlayer()
    {
        isMovingToPlayer = true;
    }

    void MoveTowardPlayer()
    {
        // Calculate direction to player
        Vector2 direction = (player.position - transform.position).normalized;

        // Accelerate as we get closer
        float normalizedDistance = distanceToPlayer / pickupRange;
        float curveValue = movementCurve.Evaluate(1f - normalizedDistance);
        currentSpeed = moveSpeed + (moveSpeed * accelerationRate * (1f - normalizedDistance));

        // Move toward player
        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            currentSpeed * Time.deltaTime
        );

        // Check if close enough to collect
        if (distanceToPlayer < 0.3f)
        {
            CollectGem();
        }
    }

    void AnimateScale()
    {
        float scale = Mathf.Lerp(minScale, maxScale,
            Mathf.PingPong(Time.time * scaleSpeed, 1f));
        transform.localScale = originalScale * scale;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player") && !isCollected)
        {
            // If player touches it directly without being in pickup range
            if (!isMovingToPlayer)
            {
                CollectGem();
            }
        }
    }

    void CollectGem()
    {
        if (isCollected) return;
        isCollected = true;

        // Add experience to player
        PlayerStats playerStats = player.GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.GainExp(expAmount);
        }

        // Play collection sound
        if (AudioManager.instance != null)
        {
            AudioManager.instance.Play("Exp");
        }




        // Destroy the gem
        Destroy(gameObject);
    }


    // Visualize pickup range in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}