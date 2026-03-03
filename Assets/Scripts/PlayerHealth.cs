using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 100;
    public int hp;

    void Awake()
    {
        hp = maxHP;
    }

    public void TakeDamage(int dmg)
    {
        hp -= dmg;
        Debug.Log("PLAYER HP: " + hp);

        if (hp <= 0)
        {
            Debug.Log("PLAYER DEAD");
            // prototype: disable
            gameObject.SetActive(false);
        }
    }
}