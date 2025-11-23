using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class Player : MonoBehaviour
{
    public static Player Instance;

    [SerializeField]
    private float playerHP = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance = this;

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void takeDamage(float damageHit)
    {
        Debug.Log(Instance.playerHP);
        Instance.playerHP -= damageHit;
    }
}
