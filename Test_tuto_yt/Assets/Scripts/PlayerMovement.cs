using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator myAnimator;

    // Basic movements 
    public float speed = 2.0f;
    private float horizMovement;
    private bool facingRight = true;

    // Checks ground & wall
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform checkGround;
    [SerializeField] private LayerMask whatIsWall;
    [SerializeField] private Transform checkWall;
    private bool isGrounded;
    private bool isWalled;

    // Jump
    public float jumpForce;
    private int resetJumpCounter = 1;
    private int jumpCounter;
    public ParticleSystem jps;

    // Wall slide
    private bool isWallSliding;
    private float wallSlidingSpeed = 1f;

    // Wall jump
    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;
    private Vector2 wallJumpingPower = new Vector2(8f, 16f);


    // __main__
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        horizMovement = Input.GetAxisRaw("Horizontal");
        var jumpInput = Input.GetButtonDown("Jump");
        var jumpInputRelease = Input.GetButtonUp("Jump");
        isGrounded = Physics2D.OverlapCircle(checkGround.position, 0.1f, whatIsGround);
        isWalled = Physics2D.OverlapCircle(checkWall.position, 0.2f, whatIsWall);

        // Jump
        if (jumpInput && jumpCounter > 0)
        {
            rb.velocity = Vector2.up * jumpForce;
            jumpCounter--;
            myAnimator.SetTrigger("Jump");
            if (!isGrounded)
            {
                jps.transform.position = gameObject.transform.position;
                jps.Play();
                Invoke(nameof(StopPS), jps.main.duration);
            }
        }

        if (jumpInputRelease && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            myAnimator.SetBool("Falling", true);
        }

        if (rb.velocity.y < 0)
        {
            myAnimator.SetBool("Falling", true);
            myAnimator.ResetTrigger("Jump");
        }
        else
        {
            myAnimator.SetBool("Falling", false);
        }

        if (isGrounded)
        {
            jumpCounter = resetJumpCounter;
            myAnimator.SetBool("Falling", false);
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizMovement * speed, rb.velocity.y);
        Flip(horizMovement);
        myAnimator.SetFloat("Speed", Mathf.Abs(horizMovement));
        WallSlide();
    }

    private void Flip(float horizontal)
    {
        if(horizontal < 0 && facingRight ||  horizontal > 0 && !facingRight)
        {
            facingRight = !facingRight;

            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }

    private void WallSlide()
    {
        if (isWalled && !isGrounded)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void WallJump()
    {

    }

    private void StopPS()
    {
        jps.Stop();
    }
}
