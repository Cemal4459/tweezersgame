using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 100;
    public int hp;

    [Header("UI (optional)")]
    public Slider hpSlider;

    void Awake()
    {
        hp = maxHP;
        UpdateUI();
    }

    public void TakeDamage(int dmg)
    {
        hp -= dmg;
        if (hp < 0) hp = 0;
        UpdateUI();

        if (hp == 0)
        {
            Debug.Log("Player Dead");
            // buraya ölme / respawn sonra
        }
    }

    void UpdateUI()
    {
        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHP;
            hpSlider.value = hp;
        }
    }
}