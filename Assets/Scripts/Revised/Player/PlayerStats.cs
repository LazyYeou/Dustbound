using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    [Header("--- Movement Settings ---")]
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public Vector2 moveDir;
    public Vector2 lastFacingDir;

    [Header("--- Base Stats ---")]
    public float maxHealth = 100;
    public float currentHealth;
    public float maxExp = 50;
    public float currentExp;
    public int level = 1;

    [Header("--- Advanced Stats (Upgradable) ---")]
    public float defense = 0f;              // Reduces incoming damage
    public float expEfficiency = 1.0f;      // Multiplies XP gained (1.0 = 100%)
    public float damageMultiplier = 1.0f;   // Multiplies bullet damage
    public float critChance = 0f;           // 0.1 = 10% chance
    public float critDamage = 1.5f;         // 1.5 = 150% damage on crit

    [Header("--- UI References ---")]
    public Slider hpSlider;
    public TextMeshProUGUI hpText;
    public Slider expSlider;
    public TextMeshProUGUI expText;

    [Header("--- Obj Reference ---")]
    public WeaponController weaponController;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        currentExp = 0;
        lastFacingDir = Vector2.right;

        UpdateHealthUI();
        UpdateExpUI();
    }

    void Update()
    {
        float mx = Input.GetAxisRaw("Horizontal");
        float my = Input.GetAxisRaw("Vertical");

        moveDir = new Vector2(mx, my).normalized;

        if (moveDir.magnitude > 0) lastFacingDir = moveDir;

        if (moveDir.x > 0) spriteRenderer.flipX = true;
        else if (moveDir.x < 0) spriteRenderer.flipX = false;
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            // Using velocity for compatibility
            rb.linearVelocity = moveDir * moveSpeed;
        }
    }

    // --- COMBAT & STATS LOGIC ---

    public void TakeDamage(float amount)
    {
        // Apply Defense Logic: Damage - Defense (Minimum 1 damage)
        float damageToTake = Mathf.Max(1, amount - defense);

        if (AudioManager.instance != null) AudioManager.instance.Play("Damage");

        currentHealth -= damageToTake;
        if (currentHealth < 0) currentHealth = 0;

        UpdateHealthUI();

        if (currentHealth <= 0) Die();
    }

    public void GainExp(float amount)
    {
        // Apply Exp Efficiency Logic
        currentExp += amount * expEfficiency;

        if (currentExp >= maxExp)
        {
            LevelUp();
        }
        UpdateExpUI();
    }

    void LevelUp()
    {
        level++;
        currentExp -= maxExp;
        maxExp *= 1.2f;

        // Heal on level up
        currentHealth = maxHealth;
        UpdateHealthUI();

        if (AudioManager.instance != null) AudioManager.instance.Play("LevelUp");

        // --- TRIGGER UPGRADE SYSTEM HERE ---
        UpgradeManager.instance.ShowUpgradeOptions();
    }

    void Die()
    {
        if (AudioManager.instance != null) AudioManager.instance.Play("Death");

        // Simple Time Scale stop for now
        GameOverManager gameOverManager = FindFirstObjectByType<GameOverManager>();
        gameOverManager.TriggerGameOver();
        Time.timeScale = 0;

        Debug.Log("Game Over");
    }

    void UpdateHealthUI()
    {
        if (hpSlider != null) hpSlider.value = currentHealth / maxHealth;
        if (hpText != null) hpText.text = $"HP : {currentHealth:F0} / {maxHealth:F0}";
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