using UnityEngine;
using UnityEngine.UI; // For Sliders
using TMPro;          // For TextMeshPro

public class PlayerStats : MonoBehaviour
{
    [Header("--- Movement Settings ---")]
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public Vector2 moveDir;
    public Vector2 lastFacingDir; // Used by Weapon to know where to shoot

    [Header("--- Stats Settings ---")]
    public float maxHealth = 100;
    public float currentHealth;
    public float maxExp = 50;
    public float currentExp;
    public int level = 1;

    [Header("--- UI References ---")]
    public Slider hpSlider;
    public TextMeshProUGUI hpText;
    public Slider expSlider;
    public TextMeshProUGUI expText;

    [Header("--- Obj Reference ---")]
    public WeaponController weaponController;

    // 1. NEW: Reference for the sprite
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // 2. NEW: Get the component
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Initialize Stats
        currentHealth = maxHealth;
        currentExp = 0;
        lastFacingDir = Vector2.right; // Default face right

        // Initialize UI
        UpdateHealthUI();
        UpdateExpUI();
    }

    void Update()
    {
        // --- MOVEMENT INPUT ---
        float mx = Input.GetAxisRaw("Horizontal");
        float my = Input.GetAxisRaw("Vertical");

        moveDir = new Vector2(mx, my).normalized;

        // Update facing direction only if moving
        if (moveDir.magnitude > 0)
        {
            lastFacingDir = moveDir;
        }

        // 3. NEW: Flip Logic
        // If moving Right, look Right (flipX unchecked)
        if (moveDir.x > 0)
        {
            spriteRenderer.flipX = true;
        }
        // If moving Left, look Left (flipX checked)
        else if (moveDir.x < 0)
        {
            spriteRenderer.flipX = false;
        }
    }

    void FixedUpdate()
    {
        // --- PHYSICS MOVEMENT ---
        if (rb != null)
        {
            // Keep your existing code (linearVelocity is for Unity 6+)
            // If using older Unity versions (2022/2023), change this to rb.velocity
            rb.linearVelocity = moveDir * moveSpeed;
        }
    }

    // --- COMBAT & STATS LOGIC ---

    public void TakeDamage(float amount)
    {
        // Make sure you have an AudioManager in the scene, otherwise this errors
        if (AudioManager.instance != null)
            AudioManager.instance.Play("Damage");

        currentHealth -= amount;

        // Clamp to 0 so we don't show negative numbers
        if (currentHealth < 0) currentHealth = 0;

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void GainExp(float amount)
    {
        currentExp += amount;

        if (currentExp >= maxExp)
        {
            LevelUp();
        }
        UpdateExpUI();
    }

    void LevelUp()
    {
        level++;
        currentExp -= maxExp; // Carry over overflow XP
        maxExp *= 1.2f;       // Increase XP needed for next level

        // Optional: Heal on level up
        currentHealth = maxHealth;
        UpdateHealthUI();

        // Decrease fire rate (make it faster)
        if (weaponController != null)
            weaponController.fireRate -= weaponController.fireRate * 0.2f;

        if (AudioManager.instance != null)
            AudioManager.instance.Play("LevelUp");

        Debug.Log("Level Up! Level: " + level);
        if (weaponController != null)
            Debug.Log($"firerate: {weaponController.fireRate}");
    }

    void Die()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.Play("Death"); // (Optional: Add this sound if you have it)

        // 2. Find the Manager and Trigger UI
        GameOverManager gm = FindFirstObjectByType<GameOverManager>();
        if (gm != null)
        {
            gm.TriggerGameOver();
        }
        else
        {
            Debug.LogWarning("No GameOverManager found in scene!");
            // Fallback if UI is missing
            Time.timeScale = 0;
        }
    }

    // --- UI UPDATES ---

    void UpdateHealthUI()
    {
        if (hpSlider != null) hpSlider.value = currentHealth / maxHealth;

        if (hpText != null)
            hpText.text = $"HP : {currentHealth:F0} / {maxHealth:F0}";
    }

    void UpdateExpUI()
    {
        if (expSlider != null) expSlider.value = currentExp / maxExp;

        if (expText != null)
        {
            float percent = (currentExp / maxExp) * 100;
            expText.text = $"Lvl {level} ({percent:F0}%)";
        }
    }
}