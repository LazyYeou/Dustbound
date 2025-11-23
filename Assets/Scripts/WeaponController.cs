using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float fireRate = 0.5f;
    private float timer;
    private PlayerStats player; // Make sure your PlayerStats has 'public Vector2 lastFacingDir'

    public bool autoFire = true;

    void Start()
    {
        timer = fireRate;
        player = GetComponent<PlayerStats>() ?? GetComponentInParent<PlayerStats>();

        if (player == null)
            Debug.LogWarning($"WeaponController on '{gameObject.name}' couldn't find PlayerStats.");
        if (bulletPrefab == null)
            Debug.LogWarning($"WeaponController on '{gameObject.name}' has no bulletPrefab assigned.");
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            // AutoFire logic
            bool shouldShoot = autoFire || Input.GetButton("Fire1");

            if (shouldShoot)
            {
                Shoot();
                timer = fireRate;
            }
            else
            {
                timer = 0f; // Ready to shoot instantly when button is pressed
            }
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null) return;

        // 1. Determine Direction FIRST
        Vector2 dir = Vector2.right; // Default
        if (player != null)
        {
            // Ensure this variable is public in your PlayerStats/PlayerController script
            dir = player.lastFacingDir;
        }

        // 2. Calculate Rotation (Mathf.Atan2 returns radians, so we convert to degrees)
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        // 3. Instantiate with the calculated rotation
        GameObject b = Instantiate(bulletPrefab, transform.position, rotation);

        // 4. Setup the Bullet Component
        Bullet bulletComp = b.GetComponent<Bullet>();
        if (bulletComp != null)
        {

            AudioManager.instance.Play("Shoot");

            bulletComp.Setup(dir);
        }
        else
        {
            Destroy(b); // Safety check
        }
    }
}