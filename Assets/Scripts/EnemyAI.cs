using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    [Header("Refs")]
    public Transform target;                 
    public Animator anim;                    

    [Header("Move")]
    public float moveSpeed = 3.5f;
    public float stopDistance = 1.6f;
    public float chaseDistance = 8f;

    [Header("Attack")]
    public float attackCooldown = 1.2f;
    public float attackRange = 1.5f;
    public int contactDamage = 10;
    public float attackHitboxTime = 0.12f;

    [Header("Hitbox (optional)")]
    public Collider2D attackHitbox;

    [Header("Animator Params (optional)")]
    public string speedParam = "";
    public string attackTrigger = "";

    [Header("Facing (only visual)")]
    public bool invertFacing = true; // sende büyük ihtimal TRUE olacak

    private Rigidbody2D rb;
    private float lastAttackTime;
    private float stunUntil = 0f;

    private int moveDir = 1;          // hareket yönü (ASLA invert ile değişmez)
    private float baseScaleX = 1f;

    public void Stun(float seconds)
    {
        stunUntil = Mathf.Max(stunUntil, Time.time + seconds);
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (anim == null) anim = GetComponent<Animator>();

        baseScaleX = Mathf.Abs(transform.localScale.x);

        if (attackHitbox != null)
            attackHitbox.enabled = false;
    }

    void Start()
    {
        if (target == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) target = p.transform;
        }
    }

    void FixedUpdate()
    {
        if (Time.time < stunUntil)
        {
            StopMove();
            return;
        }

        if (target == null) return;

        float dx = target.position.x - transform.position.x;
        float dist = Mathf.Abs(dx);

        if (dist > chaseDistance)
        {
            StopMove();
            return;
        }

        // Hareket yönü: player sağdaysa +1, soldaysa -1
        if (dx != 0) moveDir = (int)Mathf.Sign(dx);

        // Görseli her frame hedefe döndür
        SetVisualFacing(moveDir);

        // Attack
        if (dist <= attackRange)
        {
            StopMove();
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                lastAttackTime = Time.time;
                DoAttack();
            }
            return;
        }

        // Çok yakınsa dur
        if (dist <= stopDistance)
        {
            StopMove();
            return;
        }

        // Kovala
        Vector2 next = rb.position + new Vector2(moveDir * moveSpeed * Time.fixedDeltaTime, 0f);
        rb.MovePosition(next);
        SetAnimSpeed(1f);
    }

    void DoAttack()
    {
        // Attack başında da hedefe bak
        if (target != null)
        {
            float dx = target.position.x - transform.position.x;
            if (dx != 0) moveDir = (int)Mathf.Sign(dx);
            SetVisualFacing(moveDir);
        }

        if (anim != null && !string.IsNullOrEmpty(attackTrigger))
            anim.SetTrigger(attackTrigger);

        if (attackHitbox != null)
        {
            attackHitbox.enabled = true;
            CancelInvoke(nameof(DisableEnemyHitbox));
            Invoke(nameof(DisableEnemyHitbox), attackHitboxTime);
        }
        else
        {
            TryDamagePlayer();
        }
    }

    void DisableEnemyHitbox()
    {
        if (attackHitbox != null)
            attackHitbox.enabled = false;
    }

    void TryDamagePlayer()
    {
        if (target == null) return;
        var hp = target.GetComponentInParent<PlayerHealth>();
        if (hp != null) hp.TakeDamage(contactDamage);
    }

    void StopMove()
    {
        rb.linearVelocity = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
        SetAnimSpeed(0f);
    }

    void SetVisualFacing(int dirTowardTarget)
    {
        // invertFacing sadece görseli ters çevirir, hareketi etkilemez
        int visualDir = invertFacing ? -dirTowardTarget : dirTowardTarget;

        Vector3 s = transform.localScale;
        s.x = baseScaleX * visualDir;
        transform.localScale = s;
    }

    void SetAnimSpeed(float v)
    {
        if (anim == null) return;
        if (string.IsNullOrEmpty(speedParam)) return;

        anim.SetFloat(speedParam, v);
    }
}