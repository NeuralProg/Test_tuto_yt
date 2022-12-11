using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator myAnimator;

    public float speed = 2.0f;
    private float horizMovement;
    private bool facingRight = true;

    public LayerMask whatIsGround;
    private bool isGrounded;
    public Transform checkGround;
    public float checkRadius;

    public float jumpForce;
    public int resetJumpCounter;
    private int jumpCounter;

    public ParticleSystem jps;

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
        isGrounded = Physics2D.OverlapCircle(checkGround.position, checkRadius, whatIsGround);

        if (jumpInput && jumpCounter > 0)
        {
            rb.velocity = Vector2.up * jumpForce;
            jumpCounter--;
            if (!isGrounded)
            {
                jps.transform.position = gameObject.transform.position;
                jps.Play();
                Invoke(nameof(StopPS), jps.main.duration);
            }
        }
        if (jumpInputRelease && rb.velocity.y > 0)
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 3);
        if (isGrounded)
            jumpCounter = resetJumpCounter;
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizMovement * speed, rb.velocity.y);
        Flip(horizMovement);
        myAnimator.SetFloat("Speed", Mathf.Abs(horizMovement));
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

    private void StopPS()
    {
        jps.Stop();
    }
}
