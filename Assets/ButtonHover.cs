using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Visual Settings")]
    public Color glowColor = Color.yellow;
    public float scaleAmount = 1.1f;
    public float animationSpeed = 10f;

    [Header("Components")]
    private Vector3 originalScale;
    private Vector3 targetScale;
    private Outline outline;

    void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;

        outline = GetComponent<Outline>();
        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
        }

        // 3. Configure the Outline to look like a "Glow"
        outline.effectColor = glowColor;
        outline.effectDistance = new Vector2(3, -3);
        outline.enabled = false;
    }

    void Update()
    {
        // Smoothly animate the size (Lerp)
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * animationSpeed);
    }

    // Called when Mouse Enters the button area
    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = originalScale * scaleAmount; // Set target to bigger
        outline.enabled = true;                    // Turn on Glow

        // Optional: Play a sound if you have an AudioManager
        // AudioManager.instance.Play("HoverSound");
    }

    // Called when Mouse Exits the button area
    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale; // Set target back to normal
        outline.enabled = false;     // Turn off Glow
    }
}