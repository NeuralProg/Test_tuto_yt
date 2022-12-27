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
    }

    protected override void HandleMovement()
    {
        base.HandleMovement();
        myAnimator.SetFloat("Speed", direction);
        myAnimator.SetBool("Sliding", isWallSliding);
        myAnimator.SetBool("Grounded", isGrounded);
        Flip(direction);
    }
}
