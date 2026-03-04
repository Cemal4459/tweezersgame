using UnityEngine;

public class EnemyAttackHitbox : MonoBehaviour
{
    public int damage = 10;
    public LayerMask targetMask; // Player layer'ını seç

    bool active;

    void Awake()
    {
        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
    }

    public void EnableHitbox()
    {
        active = true;
        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = true;
    }

    public void DisableHitbox()
    {
        active = false;
        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!active) return;

        // layer kontrol
        if (((1 << other.gameObject.layer) & targetMask) == 0) return;

        var hp = other.GetComponentInParent<PlayerHealth>();
        if (hp != null)
        {
            hp.TakeDamage(damage);
            // tek vuruşta 1 kez hasar versin:
            DisableHitbox();
        }
    }
}