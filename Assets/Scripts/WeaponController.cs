using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public float fireRate = 0.5f;
    private float timer;
    private PlayerStats player;

    [Header("Firing Mode")]
    public bool autoFire = true;

    [Header("Spawn Enhancement")]
    public SpawnPattern spawnPattern = SpawnPattern.Single;
    public int burstCount = 3;
    public float burstSpread = 15f; // Angle spread for burst/spread shots
    public float spiralAngleIncrement = 30f; // For spiral pattern
    private float currentSpiralAngle = 0f;

    [Header("Visual Effects")]
    public GameObject muzzleFlashPrefab;
    public float muzzleFlashDuration = 0.1f;
    public Transform[] firePoints; // Multiple spawn points for variety
    private int currentFirePointIndex = 0;

    [Header("Screen Shake")]
    public bool enableScreenShake = true;
    public float shakeIntensity = 0.1f;
    public float shakeDuration = 0.1f;

    [Header("Bullet Spawn Animation")]
    public bool scaleInBullets = true;
    public float spawnScale = 0.3f;
    public float scaleInDuration = 0.15f;

    public enum SpawnPattern
    {
        Single,
        Burst,
        Spread,
        Spiral,
        Alternating,
        Wave
    }

    void Start()
    {
        timer = fireRate;
        player = GetComponent<PlayerStats>() ?? GetComponentInParent<PlayerStats>();

        if (player == null)
            Debug.LogWarning($"WeaponController on '{gameObject.name}' couldn't find PlayerStats.");
        if (bulletPrefab == null)
            Debug.LogWarning($"WeaponController on '{gameObject.name}' has no bulletPrefab assigned.");

        // If no fire points assigned, use this transform
        if (firePoints == null || firePoints.Length == 0)
        {
            firePoints = new Transform[] { transform };
        }
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            bool shouldShoot = autoFire || Input.GetButton("Fire1");

            if (shouldShoot)
            {
                Shoot();
                timer = fireRate;
            }
            else
            {
                timer = 0f;
            }
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null) return;

        // Get direction
        Vector2 dir = Vector2.right;
        if (player != null)
        {
            dir = player.lastFacingDir;
        }

        // Execute pattern-specific shooting
        switch (spawnPattern)
        {
            case SpawnPattern.Single:
                SpawnSingleBullet(dir);
                break;
            case SpawnPattern.Burst:
                SpawnBurstBullets(dir);
                break;
            case SpawnPattern.Spread:
                SpawnSpreadBullets(dir);
                break;
            case SpawnPattern.Spiral:
                SpawnSpiralBullet(dir);
                break;
            case SpawnPattern.Alternating:
                SpawnAlternatingBullet(dir);
                break;
                // case SpawnPattern.Wave:
                //     SpawnWaveBullet(dir);
                //     break;
        }

        // Spawn muzzle flash
        if (muzzleFlashPrefab != null)
        {
            SpawnMuzzleFlash(dir);
        }

        // Screen shake
        if (enableScreenShake)
        {
            StartCoroutine(ScreenShake());
        }

        AudioManager.instance?.Play("Shoot");
    }

    void SpawnSingleBullet(Vector2 dir)
    {
        Vector3 spawnPos = GetCurrentFirePoint().position;
        CreateBullet(spawnPos, dir, 0f);
    }

    void SpawnBurstBullets(Vector2 dir)
    {
        float baseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float startAngle = -(burstCount - 1) * burstSpread * 0.5f;

        for (int i = 0; i < burstCount; i++)
        {
            float angleOffset = startAngle + (i * burstSpread);
            float totalAngle = baseAngle + angleOffset;

            Vector2 bulletDir = new Vector2(
                Mathf.Cos(totalAngle * Mathf.Deg2Rad),
                Mathf.Sin(totalAngle * Mathf.Deg2Rad)
            );

            Vector3 spawnPos = GetCurrentFirePoint().position;
            CreateBullet(spawnPos, bulletDir, angleOffset, i * 0.05f); // Slight delay per bullet
        }
    }

    void SpawnSpreadBullets(Vector2 dir)
    {
        float baseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        for (int i = 0; i < burstCount; i++)
        {
            float randomAngle = Random.Range(-burstSpread, burstSpread);
            float totalAngle = baseAngle + randomAngle;

            Vector2 bulletDir = new Vector2(
                Mathf.Cos(totalAngle * Mathf.Deg2Rad),
                Mathf.Sin(totalAngle * Mathf.Deg2Rad)
            );

            Vector3 spawnPos = GetCurrentFirePoint().position;
            // Add slight random offset to spawn position
            spawnPos += (Vector3)Random.insideUnitCircle * 0.2f;
            CreateBullet(spawnPos, bulletDir, randomAngle);
        }
    }

    void SpawnSpiralBullet(Vector2 dir)
    {
        float baseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float totalAngle = baseAngle + currentSpiralAngle;

        Vector2 bulletDir = new Vector2(
            Mathf.Cos(totalAngle * Mathf.Deg2Rad),
            Mathf.Sin(totalAngle * Mathf.Deg2Rad)
        );

        Vector3 spawnPos = GetCurrentFirePoint().position;
        CreateBullet(spawnPos, bulletDir, currentSpiralAngle);

        currentSpiralAngle += spiralAngleIncrement;
        if (currentSpiralAngle >= 360f) currentSpiralAngle -= 360f;
    }

    void SpawnAlternatingBullet(Vector2 dir)
    {
        currentFirePointIndex = (currentFirePointIndex + 1) % firePoints.Length;
        Vector3 spawnPos = firePoints[currentFirePointIndex].position;
        CreateBullet(spawnPos, dir, 0f);
    }

    // void SpawnWaveBullet(Vector2 dir)
    // {
    //     // Create a bullet that will move in a wave pattern
    //     Vector3 spawnPos = GetCurrentFirePoint().position;
    //     GameObject bullet = CreateBullet(spawnPos, dir, 0f);

    //     // Add a wave component (you'll need to create this script)
    //     WaveBullet waveBullet = bullet.AddComponent<WaveBullet>();
    //     if (waveBullet != null)
    //     {
    //         waveBullet.waveAmplitude = 0.5f;
    //         waveBullet.waveFrequency = 2f;
    //     }
    // }

    GameObject CreateBullet(Vector3 position, Vector2 direction, float angleOffset, float delay = 0f)
    {
        if (delay > 0f)
        {
            StartCoroutine(DelayedBulletSpawn(position, direction, angleOffset, delay));
            return null;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        GameObject b = Instantiate(bulletPrefab, position, rotation);

        // --- NEW: APPLY STATS TO BULLET ---
        Bullet bulletComp = b.GetComponent<Bullet>();
        if (bulletComp != null && player != null)
        {
            // Calculate Crit
            bool isCrit = Random.value < player.critChance;
            float finalMult = player.damageMultiplier * (isCrit ? player.critDamage : 1f);

            // Assuming your Bullet script has a 'damage' and 'Setup' method
            // If not, you need to add public float damage to your Bullet script
            bulletComp.damage *= finalMult;
            bulletComp.Setup(direction);
        }
        // ----------------------------------

        if (scaleInBullets) StartCoroutine(ScaleInBullet(b.transform));

        return b;
    }

    System.Collections.IEnumerator DelayedBulletSpawn(Vector3 position, Vector2 direction, float angleOffset, float delay)
    {
        yield return new WaitForSeconds(delay);
        CreateBullet(position, direction, angleOffset, 0f);
    }

    System.Collections.IEnumerator ScaleInBullet(Transform bulletTransform)
    {
        if (bulletTransform == null) yield break;

        Vector3 originalScale = bulletTransform.localScale;
        bulletTransform.localScale = originalScale * spawnScale;

        float elapsed = 0f;
        while (elapsed < scaleInDuration)
        {
            if (bulletTransform == null) yield break;

            elapsed += Time.deltaTime;
            float t = elapsed / scaleInDuration;
            bulletTransform.localScale = Vector3.Lerp(originalScale * spawnScale, originalScale, t);
            yield return null;
        }

        if (bulletTransform != null)
            bulletTransform.localScale = originalScale;
    }

    void SpawnMuzzleFlash(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        GameObject flash = Instantiate(muzzleFlashPrefab, GetCurrentFirePoint().position, rotation);
        Destroy(flash, muzzleFlashDuration);
    }

    System.Collections.IEnumerator ScreenShake()
    {
        Vector3 originalPos = Camera.main.transform.position;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeIntensity;
            float y = Random.Range(-1f, 1f) * shakeIntensity;

            Camera.main.transform.position = originalPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        Camera.main.transform.position = originalPos;
    }

    Transform GetCurrentFirePoint()
    {
        return firePoints[currentFirePointIndex];
    }
}