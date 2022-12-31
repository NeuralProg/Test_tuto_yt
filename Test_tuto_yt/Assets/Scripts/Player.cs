using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Player : Character
{
    private float movementSpeed = 125f;

    public override void Start()
    {
        base.Start();
        speed = movementSpeed;
        dashTR = GetComponent<TrailRenderer>();
    }

    public override void Update()
    {
        base.Update();
        direction = Input.GetAxisRaw("Horizontal");

        var jumpInput = Input.GetButtonDown("Jump");
        var jumpInputRelease = Input.GetButtonUp("Jump");
        var dashInput = Input.GetButtonDown("Dash");

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
            HandleJumping();
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
    }

    protected override void HandleMovement()
    {
        base.HandleMovement();
        myAnimator.SetFloat("Speed", direction);
        myAnimator.SetBool("Sliding", isWallSliding);
        myAnimator.SetBool("Grounded", isGrounded);
        Flip(direction);
    }

    protected override void HandleJumping()
    {
        if (!isGrounded && !isWallSliding)
        {
            Jump();
            jumpCounter--;
            myAnimator.SetTrigger("Jump");

            jps.transform.position = gameObject.transform.position;
            jps.Play();
            Invoke(nameof(StopPS), jps.main.duration);
        }

        if (isGrounded)
        {
            Jump();
            jumpCounter--;
            myAnimator.SetTrigger("Jump");
        }

        if (isWallSliding)
        {
            isWallJumping = true;
            Invoke("StopWallJump", 0.05f);
        }
    }

    protected override void HandleDashing()
    {

    }

    private void StopPS()
    {
        jps.Stop();
    }
}
