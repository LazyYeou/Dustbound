using UnityEngine;

public class ExpGem : MonoBehaviour
{
    public float expAmount = 10f;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            col.GetComponent<PlayerStats>().GainExp(expAmount);
            AudioManager.instance.Play("Exp");
            Destroy(gameObject);
        }
    }
}