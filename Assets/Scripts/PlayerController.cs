using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public GameObject Sword, pos_sword;
    public float Scale_karak;
    public bool canMoveInAir = true;

    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private Transform feetPos;
    [SerializeField] private float circleRadius;
    [SerializeField] private LayerMask whatIsGround;

    private Rigidbody2D rigidBody;
    private Animator anim;
    private bool facingRight = true;
    private bool isGrounded;

    public FixedJoystick moveJoystick;   // left joystick for move + jump
    public FixedJoystick shootJoystick;  // right joystick for shooting

    public float shootForce = 8f;
    public float jumpThreshold = 0.7f;
    public float shootThreshold = 0.5f;
    private float nextShootTime = 0f;
    public float shootCooldown = 0.5f;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        facingRight = true;
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(feetPos.position, circleRadius, whatIsGround);

        HandleMovement();
        HandleJump();
        HandleShooting();

        // Out-of-screen death check
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        if (screenPosition.y > Screen.height || screenPosition.y < 0)
            died();
    }

    void HandleMovement()
    {
        float moveInput = moveJoystick.Horizontal != 0 ? moveJoystick.Horizontal : Input.GetAxis("Horizontal");

        rigidBody.velocity = new Vector2(moveInput * speed, rigidBody.velocity.y);

        if (moveInput > 0 && !facingRight)
            Flip();
        else if (moveInput < 0 && facingRight)
            Flip();

        anim.SetBool("isRun", Mathf.Abs(moveInput) > 0.1f && isGrounded);
    }

    void HandleJump()
    {
        if (isGrounded && moveJoystick.Vertical > jumpThreshold)
        {
            anim.SetTrigger("isJump");
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpForce);
        }
    }

    void HandleShooting()
    {
        Vector2 shootDir = new Vector2(shootJoystick.Horizontal, shootJoystick.Vertical);

        if (Time.time > nextShootTime && shootDir.magnitude > shootThreshold)
        {
            Shooting(shootDir.normalized);
            nextShootTime = Time.time + shootCooldown;
        }
    }

    void Shooting(Vector2 direction)
    {
        // 🔫 Just spawn the projectile — no movement applied to player
        Instantiate(Sword, pos_sword.transform.position, Quaternion.identity);

        // 🔁 Flip player based on shoot direction
        if (direction.x > 0 && !facingRight)
            Flip();
        else if (direction.x < 0 && facingRight)
            Flip();
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.CompareTag("Batas_Mati"))
            died();
    }

    void died()
    {
        SceneManager.LoadScene("GameOver");
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }
}
