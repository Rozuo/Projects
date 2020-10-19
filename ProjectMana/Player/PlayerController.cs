using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private Rigidbody2D rgbd2d;

    float moveVertical;
    float moveHorizontal;

    private bool facingRight = true;
    [SerializeField]
    private bool grounded = false;
    [SerializeField]
    private bool canJump = false;
    [SerializeField]
    private bool fallEarly = false;
    [SerializeField]
    private bool onWall = false;
    [SerializeField]
    private bool canWallJump;
    public bool leftWallJump = false;
    public bool rightWallJump = false;
    [SerializeField]
    private bool canDash = false;

    public LayerMask whatIsGround;
    public Transform groundCheck;
    public LayerMask whatIsWall;
    //private GameObject player;

    public float maxSpeed = 25f;
    private float jumpForce = 35f;
    private float wallJumpForce = 25f;
    private float fallForce = 17.5f;
    private float normalGravity = 9f;
    private float wallSlide = 3.25f;
    private float wallJumpX = 50f;
    private float friction = -3f;
    private float dashSpeed = 50f;


    private void Start()
    {
        // Initialize the rigidbody2d component to reduce typing
        rgbd2d = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Check if the player is on the ground
        grounded = rgbd2d.IsTouchingLayers(whatIsGround);
        // Check if the player is on a wall
        onWall = rgbd2d.IsTouchingLayers(whatIsWall);

        if (grounded && Input.GetButtonDown("Jump"))
        {
            //rgbd2d.velocity = new Vector2(0, jumpSpeed); apply a jump velocity
            //Debug.Log("JUMP!");
            canJump = true;
        }
        else if (Input.GetButtonUp("Jump") && !grounded)
        {
            if (rgbd2d.velocity.y > 0)
            {
                fallEarly = true;
            }
            else if (rgbd2d.velocity.y < 0)
            {
                fallEarly = false;
            }
        }
        if (onWall && Input.GetButtonDown("Jump"))
        {
            canWallJump = true;
        }
        else if (!onWall)
            canWallJump = false;
        else if (!onWall && Input.GetButtonUp("Jump"))
        {
            fallEarly = true;
        }
        if (transform.localScale.x > 0)
        {
            leftWallJump = true;
            rightWallJump = false;
        }else if (transform.localScale.x < 0)
        {
            leftWallJump = false;
            rightWallJump = true;
        }
        if (Input.GetButtonDown("Dash"))
        {
            canDash = true;
        }else
        {
            canDash = false;
        }

    }
        // Fixedupdate for physics
    private void FixedUpdate()
    {
        // moveHorizontal gets the axis on horizontal and multiply it by the max speed
        moveHorizontal = Input.GetAxis("Horizontal");
        rgbd2d.velocity = new Vector2(moveHorizontal * maxSpeed, rgbd2d.velocity.y);

        if (canJump)
        {
            rgbd2d.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse); //apply a jump velocity
            canJump = false;
        }
        else if (!canJump && fallEarly)
        {
            rgbd2d.gravityScale = fallForce;
            fallEarly = false;
        }
        else if (!grounded && onWall)
        {
            rgbd2d.velocity = new Vector2(rgbd2d.velocity.x, friction);
            if (canWallJump)
            {
                
                if (leftWallJump)
                {
                    rgbd2d.AddForce(new Vector2(wallJumpX * -1, wallJumpForce), ForceMode2D.Impulse);
                }
                else if (rightWallJump)
                {
                    rgbd2d.AddForce(new Vector2(wallJumpX, wallJumpForce), ForceMode2D.Impulse);
                }
            }
        }
        if (rgbd2d.velocity.y < 0)
        {
            rgbd2d.gravityScale = fallForce;
        }
        else if (rgbd2d.velocity.y >= 0)
        {
            rgbd2d.gravityScale = normalGravity;
        }

        if (canDash)
        {
            rgbd2d.velocity = new Vector2(dashSpeed*(transform.localScale.x), 0);

        }

        // This code is to flip the sprites facing direction
        if (moveHorizontal > 0 && !facingRight)
        {
            Flip();
        }
        else if (moveHorizontal < 0 && facingRight)
        {
            Flip();
        }
    }

    // A function to flip the sprites facing direction
    void Flip()
    {
        facingRight = !facingRight; // facing right is now equal to the opposite bool from the original facingRight
        Vector3 theScale = transform.localScale; // get the current localscale of the player
        theScale.x *= -1; // change the numerical representation of the sprites facing direction
        transform.localScale = theScale; // apply the change to the sprites
    }
}
