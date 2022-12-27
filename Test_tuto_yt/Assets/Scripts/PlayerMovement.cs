using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    [Header("Basic movements")]
    private Rigidbody2D rb;
    private Animator myAnimator;
    private float speed = 125f;
    private float horizMovement;
    private bool facingRight = true;

    [Header("State checks")]
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform checkGround;
    [SerializeField] private LayerMask whatIsWall;
    [SerializeField] private Transform checkWall;
    private bool isGrounded; 
    private bool isWalled;

    [Header("Basic jump")]
    private float jumpForce = 8f;
    private int resetJumpCounter = 1;
    private int jumpCounter;
    [SerializeField] private ParticleSystem jps;

    [Header("Wall jump")]
    private bool isWallSliding;     
    private float wallSlidingSpeed = 1f;
    private bool isWallJumping;

    [Header("Dash")]
    private float dashingVelocity = 7f;
    private float dashingTime = 0.25f;
    private float dashingXDirection;
    private bool isDashing;
    private bool canDash;
    private TrailRenderer dashTR;


    // __main__
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        dashTR = GetComponent<TrailRenderer>();
    }

    private void Update()
    {
        horizMovement = Input.GetAxisRaw("Horizontal");
        var jumpInput = Input.GetButtonDown("Jump");
        var jumpInputRelease = Input.GetButtonUp("Jump");
        var dashInput = Input.GetButtonDown("Dash");

        isGrounded = Physics2D.OverlapCircle(checkGround.position, 0.1f, whatIsGround);
        isWalled = Physics2D.OverlapCircle(checkWall.position, 0.1f, whatIsWall);

        // Grounded
        if (isGrounded)
        {
            jumpCounter = resetJumpCounter;
            myAnimator.SetBool("Falling", false);
            myAnimator.ResetTrigger("Dash");
            canDash = true;
        }
        if (isWallSliding)
        {
            jumpCounter = resetJumpCounter;
            myAnimator.ResetTrigger("Jump");
            myAnimator.ResetTrigger("Dash");
            canDash = true;
        }

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

        if (rb.velocity.y < -1)
        {
            myAnimator.SetBool("Falling", true);
            myAnimator.ResetTrigger("Jump");
        }
        else
        {
            myAnimator.SetBool("Falling", false);
        }

        // Dash
        Dash(dashInput);
    }

    private void FixedUpdate()
    {
        myAnimator.SetFloat("Speed", Mathf.Abs(horizMovement));
        myAnimator.SetBool("Sliding", isWallSliding);
        myAnimator.SetBool("Grounded", isGrounded);

        WallSlide();
        
        if (!isWallSliding)
            Flip(horizMovement);

        if (isWallJumping)
            rb.velocity = new Vector2(-horizMovement * 6f, 8f);
        else
        {
            if (!isDashing)
                rb.velocity = new Vector2(horizMovement * speed * Time.deltaTime, rb.velocity.y);
        }
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

    // Particles jump
    private void StopPS()
    {
        jps.Stop();
    }

    // Wall slide/jump
    private void WallSlide()
    {
        if (isWalled && !isGrounded && horizMovement != 0f)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
            isWallSliding = false;
    }

    private void StopWallJump()
    {
        isWallJumping = false;
    }

    // Dash
    private void Dash(bool dashInput)
    {
        if (dashInput && canDash && !isDashing)
        {
            isDashing = true;
            canDash = false;
            dashTR.emitting = true;
            dashingXDirection = horizMovement;
            myAnimator.SetTrigger("Dash");

            if (dashingXDirection == 0f)
            {
                dashingXDirection = transform.localScale.x;
            }

            StartCoroutine(StopDashing());
        }

        if (isDashing)
        {
            rb.velocity = new Vector2(dashingXDirection * dashingVelocity, 0);
        }
    }

    private IEnumerator StopDashing()
    {
        yield return new WaitForSeconds(dashingTime);
        isDashing = false;
        dashTR.emitting = false;
    }
}
