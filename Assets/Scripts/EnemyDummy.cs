using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyDummy : MonoBehaviour
{
    [Header("HP")]
    public int maxHP = 100;

    [Header("Knockback (fixed step)")]
    public float knockbackStep = 0.6f;      // 1 adım mesafe
    public float knockbackTime = 0.08f;     // ne kadar sürede geri gitsin
    public LayerMask obstacleMask;          // duvar/engel (Ground layer'ını buraya ver)
    public float obstacleCheckDistance = 0.65f;

    [Header("Options")]
    public bool lockMovementDuringKnockback = true;

    private int hp;
    private Rigidbody2D rb;
    private Coroutine kbRoutine;

    void Awake()
    {
        hp = maxHP;
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeHit(int damage, Vector2 knockback, Vector2 hitPoint)
    {
        hp -= damage;

        // Kayma birikmesin
        rb.linearVelocity = Vector2.zero;

        // Knockback yönü (sadece X)
        float dir = Mathf.Sign((transform.position.x - hitPoint.x));
        if (knockback.x != 0f) dir = Mathf.Sign(knockback.x);

        if (kbRoutine != null) StopCoroutine(kbRoutine);
        kbRoutine = StartCoroutine(KnockbackStep(dir));

        if (hp <= 0)
            gameObject.SetActive(false);
    }

    private System.Collections.IEnumerator KnockbackStep(float dir)
    {
        // Duvar kontrolü (hedefe giderken duvara çarpmayalım)
        Vector2 start = rb.position;

        float step = knockbackStep;
        Vector2 checkDir = new Vector2(dir, 0f);

        // Eğer hemen arkamızda duvar varsa adımı küçült (hatta 0 yap)
        RaycastHit2D hit = Physics2D.Raycast(start, checkDir, obstacleCheckDistance, obstacleMask);
        if (hit.collider != null)
        {
            // Duvara kadar olan mesafeden biraz kısa git
            step = Mathf.Max(0f, hit.distance - 0.05f);
        }

        Vector2 target = start + new Vector2(dir * step, 0f);

        // İstersen knockback sırasında diğer kuvvetlerden etkilenmesin:
        RigidbodyConstraints2D prevConstraints = rb.constraints;
        if (lockMovementDuringKnockback)
        {
            // X serbest kalsın, sadece rotasyonu kilitle
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        float t = 0f;
        while (t < knockbackTime)
        {
            t += Time.fixedDeltaTime;
            float k = Mathf.Clamp01(t / knockbackTime);

            rb.MovePosition(Vector2.Lerp(start, target, k));
            yield return new WaitForFixedUpdate();
        }

        rb.linearVelocity = Vector2.zero;

        if (lockMovementDuringKnockback)
            rb.constraints = prevConstraints;

        kbRoutine = null;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * obstacleCheckDistance);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.left * obstacleCheckDistance);
    }
#endif
}