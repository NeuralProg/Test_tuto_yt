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
    protected bool isGrounded;
    protected bool isWalled;

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
    #endregion


    #region subMechanic
    protected virtual void HandleMovement()
    {
        Move();
    }

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
