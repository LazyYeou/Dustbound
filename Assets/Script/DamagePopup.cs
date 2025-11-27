using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    private TextMeshPro textMesh;
    private float disappearTimer = 1f;

    public void Setup(float damageAmount, bool isCritical)
    {
        textMesh = GetComponent<TextMeshPro>();
        textMesh.text = damageAmount.ToString();

        if (isCritical)
        {
            textMesh.fontSize = 6;
            textMesh.color = Color.yellow;
        }
        else
        {
            textMesh.fontSize = 4;
            textMesh.color = Color.white;
        }
    }

    void Update()
    {
        transform.position += new Vector3(0, 2f * Time.deltaTime, 0); // Float up

        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            Color color = textMesh.color;
            color.a -= 3f * Time.deltaTime; // Fade out
            textMesh.color = color;
            if (color.a < 0) Destroy(gameObject);
        }
    }
}