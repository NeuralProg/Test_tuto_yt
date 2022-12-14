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

    // Wall jump
    private bool isWallSliding;     
    private float wallSlidingSpeed = 1f;
    private bool isWallJumping;

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
        isGrounded = Physics2D.OverlapCircle(checkGround.position, 0.05f, whatIsGround);
        isWalled = Physics2D.OverlapCircle(checkWall.position, 0.1f, whatIsWall);

        // Jump
        if (jumpInput && jumpCounter > 0)
        {
            Jump();
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
        myAnimator.SetFloat("Speed", Mathf.Abs(horizMovement));
        myAnimator.SetBool("Sliding", isWallSliding);

        WallSlide();
        
        if (!isWallSliding)
            Flip(horizMovement);

        if (isWallJumping)
            rb.velocity = new Vector2(-horizMovement * 6f, 8f);
        else
            rb.velocity = new Vector2(horizMovement * speed, rb.velocity.y);
    }

    // Custom functions
    private void Flip(float horizontal)
    {
        if (horizontal < 0 && facingRight || horizontal > 0 && !facingRight)
        {
            facingRight = !facingRight;
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }

    private void Jump()
    {
        if (!isGrounded && !isWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCounter--;
            myAnimator.SetTrigger("Jump");

            jps.transform.position = gameObject.transform.position;
            jps.Play();
            Invoke(nameof(StopPS), jps.main.duration);
        }

        if (isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCounter--;
            myAnimator.SetTrigger("Jump");
        }

        if (isWallSliding)
        {
            isWallJumping = true;
            Invoke("StopWallJump", 0.05f);
        }
    }

    // Wall slide
    private void WallSlide()
    {
        if (isWalled && !isGrounded && horizMovement != 0f)
        {
            isWallSliding = true;
            jumpCounter = resetJumpCounter;
            myAnimator.ResetTrigger("Jump");

            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
            isWallSliding = false;
    }

    private void StopWallJump()
    {
        isWallJumping = false;
    }

    // Particles
    private void StopPS()
    {
        jps.Stop();
    }
}
