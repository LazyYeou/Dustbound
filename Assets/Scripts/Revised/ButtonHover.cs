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

        outline.effectColor = glowColor;
        outline.effectDistance = new Vector2(3, -3);
        outline.enabled = false;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * animationSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = originalScale * scaleAmount;
        outline.enabled = true;

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale;
        outline.enabled = false;
    }
}