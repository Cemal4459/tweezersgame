using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove2D : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 8f;
    public float acceleration = 60f;
    public float deceleration = 80f;

    [Header("Jump")]
    public float jumpForce = 14f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.12f;
    public LayerMask groundMask;

    private Rigidbody2D rb;
    private Animator anim;

    private float inputX;
    private bool jumpPressed;
    [SerializeField] private Transform visual;
    [SerializeField] private Transform hitboxTransform;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>(); // Visual'in altındaki Animator'ı bulur
        if (visual == null) visual = transform.Find("Visual");
if (hitboxTransform == null) hitboxTransform = transform.Find("HitBox"); // dikkat: HitBox olabilir
if (hitboxTransform == null) hitboxTransform = transform.Find("Hitbox"); // olmazsa bunu dener
    }

    void Update()
{
    // 1) Input oku
    inputX = Input.GetAxisRaw("Horizontal");

    // 2) Animator parametreleri (stabil)
    if (anim != null)
    {
        anim.SetFloat("Speed", Mathf.Abs(inputX));
        // Ground kontrolünü FixedUpdate'te yapıyorsan orada SetBool("IsGrounded", ...) kalsın
    }

    // 3) Jump input
    if (Input.GetButtonDown("Jump"))
        jumpPressed = true;

    // 4) (Opsiyonel) Yöne göre Visual + Hitbox çevirme
   if (inputX != 0)
{
    float dir = Mathf.Sign(inputX);

    // Visual flip
    if (visual != null)
    {
        Vector3 s = visual.localScale;
        s.x = Mathf.Abs(s.x) * dir;
        visual.localScale = s;
    }

    // Hitbox'u tarafa geçir
    if (hitboxTransform != null)
    {
        Vector3 p = hitboxTransform.localPosition;
        p.x = Mathf.Abs(p.x) * dir;
        hitboxTransform.localPosition = p;
    }
}
}

    void FixedUpdate()
    {
        // Ground check
        bool isGrounded = false;
        if (groundCheck != null)
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask);

        // İstersen anim'e grounded da gönder
        if (anim != null)
            anim.SetBool("IsGrounded", isGrounded);

        // Horizontal movement with accel/decel
        float targetSpeed = inputX * moveSpeed;
        float speedDiff = targetSpeed - rb.linearVelocity.x;
        float accelRate = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration;
        float movement = speedDiff * accelRate;

        rb.AddForce(new Vector2(movement, 0f));

        // Clamp horizontal speed
        rb.linearVelocity = new Vector2(Mathf.Clamp(rb.linearVelocity.x, -moveSpeed, moveSpeed), rb.linearVelocity.y);

        // Jump
        if (jumpPressed)
        {
            jumpPressed = false;
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}