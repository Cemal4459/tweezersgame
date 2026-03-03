using System.Collections.Generic;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public int damage = 10;
    public float knockbackForce = 6f;
    public LayerMask targetMask;

    private readonly HashSet<Collider2D> hit = new HashSet<Collider2D>();
    private Collider2D col;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        col.enabled = false;
    }

    public void EnableHitbox()
    {
        hit.Clear();
        col.enabled = true;
        Debug.Log("HITBOX ENABLED");
    }

    public void DisableHitbox()
    {
        col.enabled = false;
        hit.Clear();
        Debug.Log("HITBOX DISABLED");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("TRIGGER HIT: " + other.name + " layer=" + LayerMask.LayerToName(other.gameObject.layer));
        Debug.Log("Hitbox touched: " + other.name);
         Debug.Log("TRIGGER: " + other.name + " layer=" + other.gameObject.layer);

        if (((1 << other.gameObject.layer) & targetMask) == 0) return;
        Debug.Log("Layer check passed, trying damage");

        if (hit.Contains(other)) return;
        hit.Add(other);

        var enemy = other.GetComponentInParent<EnemyDummy>();
        Debug.Log("EnemyDummy found? " + (enemy != null));

        if (enemy == null) return;

        Vector2 dir = (other.transform.position - transform.position).normalized;
        Vector2 knock = dir * knockbackForce;

        Debug.Log("Calling TakeHit()");
        enemy.TakeHit(damage, knock, transform.position);
        HitStopManager.Instance.Stop(0.06f);
    }
}