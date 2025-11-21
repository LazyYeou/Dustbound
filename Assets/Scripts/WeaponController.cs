using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float fireRate = 0.5f;
    public float nextFireTime;

    private PlayerController player;

    void Start()
    {
        player = GetComponent<PlayerController>();
        nextFireTime = fireRate;
    }

    void Update()
    {
        if (Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Fire()
    {
        GameObject bullet = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().Setup(player.lastDirection);
    }
}
