using UnityEngine;

public class MobileInputController : MonoBehaviour
{
    public FixedJoystick moveJoystick;     // Left joystick for movement + jump
    public FixedJoystick shootJoystick;    // Right joystick for aiming + shooting

    public float speed = 5f;
    public float jumpForce = 8f;
    public float jumpThreshold = 0.7f;      // how far up user must drag to jump
    public float shootThreshold = 0.5f;     // how far user must drag to trigger shoot

    public Transform feetPos;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    public GameObject Sword;
    public Transform pos_sword;
    public float shootForce = 8f;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded = false;
    private bool facingRight = true;
    private bool canShoot = true;
    private float shootCooldown = 0.5f;
    private float nextShootTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(feetPos.position, groundCheckRadius, groundLayer);

        float moveInput = moveJoystick.Horizontal;
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        // Flip direction
        if (moveInput > 0 && !facingRight)
            Flip();
        else if (moveInput < 0 && facingRight)
            Flip();

        // Run animation
        anim.SetBool("isRun", Mathf.Abs(moveInput) > 0.1f && isGrounded);

        // Jump if pushing joystick upward
        if (isGrounded && moveJoystick.Vertical > jumpThreshold)
        {
            anim.SetTrigger("isJump");
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // Shoot if dragging right joystick
        if (Time.time > nextShootTime && shootJoystick.Horizontal != 0)
        {
            Vector2 shootDirection = new Vector2(shootJoystick.Horizontal, shootJoystick.Vertical);
            if (shootDirection.magnitude > shootThreshold)
            {
                OnFire(shootDirection.normalized);
                nextShootTime = Time.time + shootCooldown;
            }
        }
    }

    void OnFire(Vector2 direction)
    {
        // Optional: flip player toward shoot direction
        if (direction.x > 0 && !facingRight)
            Flip();
        else if (direction.x < 0 && facingRight)
            Flip();

        // Apply force and shoot
        rb.velocity = new Vector2(direction.x * shootForce, rb.velocity.y);
        Instantiate(Sword, pos_sword.position, Quaternion.identity);
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
