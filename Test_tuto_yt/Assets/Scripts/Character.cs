using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    // References
    protected Rigidbody2D rb;
    protected Animator myAnimator;
    protected TrailRenderer dashTR;

    // Basic movements
    protected float speed = 80f;
    protected float direction;
    protected bool facingRight = true;

    [Header("State checks")]
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] protected Transform checkGround;
    [SerializeField] protected LayerMask whatIsWall;
    [SerializeField] protected Transform checkWall;
    [SerializeField] protected bool isGrounded;
    [SerializeField] protected bool isWalled;

    [Header("Basic jump")]
    [SerializeField] protected ParticleSystem jps;
    protected float jumpForce = 8f;
    protected int resetJumpCounter = 1;
    protected int jumpCounter;

    // Wall jump
    protected bool isWallSliding;
    protected float wallSlidingSpeed = 1f;
    protected bool isWallJumping;

    // Dash
    protected float dashingVelocity = 7f;
    protected float dashingTime = 0.25f;
    protected float dashingXDirection;
    protected bool isDashing;
    protected bool canDash;


    #region monos
    public virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
    }

    public virtual void Update() 
    {
        // Handle Input
        isGrounded = Physics2D.OverlapCircle(checkGround.position, 0.1f, whatIsGround);

        if (rb.velocity.y < -1)
        {
            myAnimator.SetBool("Falling", true);
            myAnimator.ResetTrigger("Jump");
        }
        else
        {
            myAnimator.SetBool("Falling", false);
        }
    }

    public virtual void FixedUpdate()
    {
        // Handle mechanics / physics
        HandleMovement();
    }
    #endregion


    #region mechanics
    protected void Move()
    {
        rb.velocity = new Vector2(direction * speed * Time.deltaTime, rb.velocity.y);
    }

    protected void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    protected void Dash()
    {

    }
    #endregion


    #region subMechanic
    protected virtual void HandleMovement()
    {
        Move();
    }

    protected abstract void HandleJumping();

    protected abstract void HandleDashing();

    protected void Flip(float horizontal)
    {
        if (horizontal < 0 && facingRight || horizontal > 0 && !facingRight)
        {
            facingRight = !facingRight;
            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        }
    }
    #endregion

}
