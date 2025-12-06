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
    public float defense = 0f;
    public float expEfficiency = 1.0f;
    public float damageMultiplier = 1.0f;
    public float critChance = 0f;
    public float critDamage = 1.5f;

    [Header("--- UI References ---")]
    public Slider hpSlider;
    public TextMeshProUGUI hpText;
    public Slider expSlider;
    public TextMeshProUGUI expText;

    [Header("--- Obj Reference ---")]
    public WeaponController weaponController;
    private SpriteRenderer spriteRenderer;

    private Animator animator;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        currentExp = 0;
        lastFacingDir = Vector2.right;

        animator = GetComponent<Animator>();

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

        if (animator != null)
        {
            bool isMoving = moveDir.magnitude > 0;

            animator.SetBool("IsMoving", isMoving);

            animator.speed = 1f;
        }
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            rb.linearVelocity = moveDir * moveSpeed;
        }
    }

    public void TakeDamage(float amount)
    {
        float damageToTake = Mathf.Max(1, amount - defense);

        if (AudioManager.instance != null) AudioManager.instance.Play("Damage");

        currentHealth -= damageToTake;
        if (currentHealth < 0) currentHealth = 0;

        UpdateHealthUI();

        if (currentHealth <= 0) Die();
    }

    public void GainExp(float amount)
    {
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

        currentHealth = maxHealth;
        UpdateHealthUI();

        if (AudioManager.instance != null) AudioManager.instance.Play("LevelUp");

        UpgradeManager.instance.ShowUpgradeOptions();
    }

    void Die()
    {
        if (AudioManager.instance != null) AudioManager.instance.Play("Death");

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