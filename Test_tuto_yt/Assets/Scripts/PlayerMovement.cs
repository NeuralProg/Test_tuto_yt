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

    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform checkGround;
    private bool isGrounded;
    private float checkRadius = 0.15f;

    public float jumpForce;
    private int resetJumpCounter = 1;
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
        HandleLayers();
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

    private void HandleLayers()
    {
        if (!isGrounded)
        {
            myAnimator.SetLayerWeight(1, 1);
        }
        else
        {
            myAnimator.SetLayerWeight(1, 0);
        }
    } // Probleme : running anim in air 

    private void StopPS()
    {
        jps.Stop();
    }
}
