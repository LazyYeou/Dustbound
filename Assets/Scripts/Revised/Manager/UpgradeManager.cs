using UnityEngine;
using System.Collections.Generic;

public enum UpgradeType
{
    MaxHP,
    ExpEfficiency,
    BulletDamage,
    AttackSpeed,
    MovementSpeed,
    Defense,
    BulletAmount,
    CritChance
}

[System.Serializable]
public class UpgradeDefinition
{
    public string name;
    public UpgradeType type;
    public float value; // e.g., 10 for HP, 0.1 for 10% speed
    [TextArea] public string description;
}

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance;

    [Header("UI References")]
    public GameObject upgradePanel;     // The whole screen/panel
    public Transform cardContainer;     // The Horizontal Layout Group
    public GameObject cardPrefab;       // The prefab with UpgradeCardUI

    [Header("Configuration")]
    public List<UpgradeDefinition> allUpgrades; // Fill this in Inspector
    public PlayerStats player;
    public WeaponController weapon;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        upgradePanel.SetActive(false);
        // Auto-find references if not assigned
        if (player == null) player = FindFirstObjectByType<PlayerStats>();
        if (weapon == null) weapon = FindFirstObjectByType<WeaponController>();
    }

    public void ShowUpgradeOptions()
    {
        Time.timeScale = 0f; // Pause Game
        upgradePanel.SetActive(true);

        // Clear old cards
        foreach (Transform child in cardContainer)
        {
            Destroy(child.gameObject);
        }

        // Pick 3 Random Unique Upgrades
        List<UpgradeDefinition> choices = new List<UpgradeDefinition>();
        List<UpgradeDefinition> pool = new List<UpgradeDefinition>(allUpgrades);

        for (int i = 0; i < 3; i++)
        {
            if (pool.Count == 0) break;
            int rnd = Random.Range(0, pool.Count);
            choices.Add(pool[rnd]);
            pool.RemoveAt(rnd); // Prevent picking exact same one twice
        }

        // Spawn Cards
        foreach (var upgrade in choices)
        {
            GameObject cardObj = Instantiate(cardPrefab, cardContainer);
            cardObj.GetComponent<UpgradeCardUI>().Setup(upgrade);
        }
    }

    public void ApplyUpgrade(UpgradeDefinition upgrade)
    {
        switch (upgrade.type)
        {
            case UpgradeType.MaxHP:
                player.maxHealth += upgrade.value;
                player.currentHealth += upgrade.value; // Heal the amount gained
                break;

            case UpgradeType.ExpEfficiency:
                player.expEfficiency += upgrade.value; // e.g. 0.2
                break;

            case UpgradeType.BulletDamage:
                player.damageMultiplier += upgrade.value; // e.g. 0.1
                break;

            case UpgradeType.AttackSpeed:
                // Lower fireRate means faster shooting
                // e.g., reduce delay by 10%: weapon.fireRate * (1 - 0.1)
                weapon.fireRate -= (weapon.fireRate * upgrade.value);
                if (weapon.fireRate < 0.05f) weapon.fireRate = 0.05f; // Cap speed
                break;

            case UpgradeType.MovementSpeed:
                player.moveSpeed += upgrade.value;
                break;

            case UpgradeType.Defense:
                player.defense += upgrade.value;
                break;

            case UpgradeType.BulletAmount:
                weapon.burstCount += (int)upgrade.value;
                // If using single shot, switch to burst automatically if count > 1
                if (weapon.spawnPattern == WeaponController.SpawnPattern.Single && weapon.burstCount > 1)
                {
                    weapon.spawnPattern = WeaponController.SpawnPattern.Burst;
                }
                break;

            case UpgradeType.CritChance:
                player.critChance += upgrade.value;
                break;
        }

        ClosePanel();
    }

    void ClosePanel()
    {
        upgradePanel.SetActive(false);
        Time.timeScale = 1f; // Resume Game
    }
}