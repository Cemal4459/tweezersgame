using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyDummy : MonoBehaviour
{
    [Header("HP")]
    public int maxHP = 100;

    [Header("Animator (optional)")]
    public Animator anim;                 // Enemy root'taki Animator
    public string hitTrigger = "TakeHit"; // controller'da yoksa boş bırak
    public string deathTrigger = "Death"; // controller'da yoksa boş bırak
    public float hitStun = 0.20f;         // vurulunca sersemleme
    public float destroyDelay = 1.2f;     // death anim süresi kadar

    [Header("Refs (optional)")]
    public EnemyAI ai;                    // otomatik bulur
    public Collider2D bodyCollider;       // root collider (player ile çarpışma)

    [Header("Knockback (fixed step)")]
    public float knockbackStep = 0.6f;      // 1 adım mesafe
    public float knockbackTime = 0.08f;     // ne kadar sürede geri gitsin
    public LayerMask obstacleMask;          // sadece Ground/Wall ver
    public float obstacleCheckDistance = 0.65f;

    [Header("Options")]
    public bool lockMovementDuringKnockback = true;

    int hp;
    Rigidbody2D rb;
    Coroutine kbRoutine;
    bool dead;
    public System.Action OnDead;

    void Awake()
    {
        hp = maxHP;
        rb = GetComponent<Rigidbody2D>();

        if (ai == null) ai = GetComponent<EnemyAI>();
        if (anim == null) anim = GetComponent<Animator>();
        if (bodyCollider == null) bodyCollider = GetComponent<Collider2D>();
    }

    public void TakeHit(int damage, Vector2 knockback, Vector2 hitPoint)
    {
        if (dead) return;

        Debug.Log("EnemyDummy TakeHit called!");
        hp -= damage;

        // Shake (ikisi birden varsa ikisini de kullanabilir)
        if (CameraShakeTrigger.Instance != null)
            CameraShakeTrigger.Instance.Shake(0.8f);

        if (CameraTargetShaker.Instance != null)
            CameraTargetShaker.Instance.Shake(0.10f, 0.12f);

        // Vurulma anim + stun
        if (anim != null && !string.IsNullOrEmpty(hitTrigger))
            anim.SetTrigger(hitTrigger);

        float stunTime = Mathf.Max(knockbackTime, hitStun);
        if (ai != null) ai.Stun(stunTime);

        // Kayma birikmesin (Unity 6 uyumu)
        rb.linearVelocity = Vector2.zero;
        rb.linearVelocity = Vector2.zero;

        // HER ZAMAN saldırandan UZAKLAŞ (hitPoint = saldırı kaynağı)
        float dir = Mathf.Sign(transform.position.x - hitPoint.x);
        if (dir == 0f) dir = 1f;

        if (kbRoutine != null) StopCoroutine(kbRoutine);
        kbRoutine = StartCoroutine(KnockbackStep(dir));

        if (hp <= 0)
            Die();
    }

    void Die()
    {
        OnDead?.Invoke();
        if (dead) return;
        dead = true;

        // AI kapat
        if (ai != null) ai.enabled = false;

        // Collider kapat (üst üste binmesin)
        if (bodyCollider != null) bodyCollider.enabled = false;

        // Hitbox child varsa kapatmak istersen (EnemyHitbox gibi)
        // foreach (var c in GetComponentsInChildren<Collider2D>()) c.enabled = false;

        rb.linearVelocity = Vector2.zero;
        rb.linearVelocity = Vector2.zero;

        // Death anim
        if (anim != null && !string.IsNullOrEmpty(deathTrigger))
            anim.SetTrigger(deathTrigger);

        // Ölüyken knockback coroutine kalmasın
        if (kbRoutine != null) StopCoroutine(kbRoutine);
        kbRoutine = null;

        // Bir süre sonra yok et
        Destroy(gameObject, destroyDelay);
    }

    System.Collections.IEnumerator KnockbackStep(float dir)
    {
        Vector2 start = rb.position;

        float step = knockbackStep;
        Vector2 checkDir = new Vector2(dir, 0f);

        // obstacleMask boş değilse duvar kontrolü yap
        if (obstacleMask.value != 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(start, checkDir, obstacleCheckDistance, obstacleMask);
            if (hit.collider != null)
                step = Mathf.Max(0f, hit.distance - 0.05f);
        }

        Vector2 target = start + new Vector2(dir * step, 0f);

        RigidbodyConstraints2D prevConstraints = rb.constraints;
        if (lockMovementDuringKnockback)
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        float t = 0f;
        while (t < knockbackTime)
        {
            if (dead) yield break;

            t += Time.fixedDeltaTime;
            float k = Mathf.Clamp01(t / knockbackTime);

            rb.MovePosition(Vector2.Lerp(start, target, k));
            yield return new WaitForFixedUpdate();
        }

        rb.linearVelocity = Vector2.zero;
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