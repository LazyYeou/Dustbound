using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeCardUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Image iconImage;
    public Button selectButton;

    private UpgradeDefinition myUpgrade;

    public void Setup(UpgradeDefinition upgrade)
    {
        myUpgrade = upgrade;

        titleText.text = upgrade.name;
        descriptionText.text = upgrade.description;

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => UpgradeManager.instance.ApplyUpgrade(myUpgrade));
    }
}