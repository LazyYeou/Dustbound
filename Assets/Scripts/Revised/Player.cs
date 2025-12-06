using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class Player : MonoBehaviour
{
    public static Player Instance;

    [SerializeField]
    private float playerHP = 10f;
    void Start()
    {
        Instance = this;

    }

    void Update()
    {

    }

    public void takeDamage(float damageHit)
    {
        Debug.Log(Instance.playerHP);
        Instance.playerHP -= damageHit;
    }
}
