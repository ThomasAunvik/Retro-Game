using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public enum PlayerCount
    {
        P1, P2
    }
    [Header("Information")]
    [SerializeField] PlayerCount playerCount = PlayerCount.P1;
    public PlayerCount GetPlayerCount { get { return playerCount; } }

    [SerializeField] float movSpeed = 0.5f;

    [SerializeField] float jumpForce = 1;

    [Header("Movement")]
    public LayerMask groundLayer;
    public LayerMask iceLayer;
    public LayerMask slowLayer;

    public float groundRaycastDistance = 1;
    public float iceSlipperyReducer = 2;
    public float slowSpeed = 2;

    bool landed = false;
    bool touchingIce = false;
    
    float extraXVelocity;

    Rigidbody2D rb2d;
    Animator animator;
    SpriteRenderer spriteRenderer;

    float defaultMovSpeed;
    float gravForce;

    public float jumpVelocity = 70;

    bool jumping = false;
    public bool IsJumping { get { return jumping; } }

    bool canDoubleJumpOnce = false;
    float flightTime = 0;

    bool stunned = false;
    float knockback;

    bool freeze = false;

    private void Awake()
    {
        defaultMovSpeed = movSpeed;
    }

    void Start () {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
	void FixedUpdate ()
    {
        if (freeze)
        {
            rb2d.simulated = false;
            return;
        }
        else rb2d.simulated = true;

        // Inputs and variables
        Vector2 newPosition = rb2d.position;
        bool jump = false;
        bool jumpButtonPressed = false;
        float moveAxis = 0;

        if ((!GameManager.instance.freeze || GameManager.instance.theFreezer == this) && !stunned)
        {
            if (playerCount == PlayerCount.P1)
            {
                moveAxis = Input.GetAxis("P1_Horizontal");
                jump = Input.GetButton("P1_Jump");
                jumpButtonPressed = Input.GetButtonDown("P1_Jump");
            }
            else if (playerCount == PlayerCount.P2)
            {
                moveAxis = Input.GetAxis("P2_Horizontal");
                jump = Input.GetButton("P2_Jump");
                jumpButtonPressed = Input.GetButtonDown("P2_Jump");
            }
        }
        else
        {
            moveAxis = 0;
            jump = false;
            jumpButtonPressed = false;
        }
        // Jumping
        bool isGrounded = IsGrounded();
        if (jump && ((isGrounded && rb2d.velocity.y == 0) || (canDoubleJumpOnce && jumpButtonPressed) || Time.timeSinceLevelLoad < flightTime))
        {
            //Vector2 Movement = new Vector2(rb2d.velocity.x, jumpForce);
            rb2d.velocity = Vector2.up * jumpForce;
            
            extraXVelocity = 0;
            landed = false;
            touchingIce = false;
            jumping = true;

            if (!isGrounded) canDoubleJumpOnce = false;
        }
        animator.SetBool("inAir", !isGrounded);

        // Walking
        if (extraXVelocity != 0 || touchingIce) extraXVelocity += moveAxis / iceSlipperyReducer;
        if (!touchingIce) extraXVelocity = 0;
        
        rb2d.velocity = new Vector2((moveAxis * movSpeed) + knockback + extraXVelocity, rb2d.velocity.y);

        if (knockback > 0) knockback -= Time.deltaTime * 1000;
        else if (knockback < 0) knockback += Time.deltaTime * 1000;


        // Rotation of player
        if (moveAxis > 0)
        {
            spriteRenderer.flipX = false;
            animator.SetBool("isWalking", true);
        }
        else if (moveAxis < 0)
        {
            spriteRenderer.flipX = true;
            animator.SetBool("isWalking", true);
        }
        else animator.SetBool("isWalking", false);
    }

    bool IsGrounded()
    {
        Vector2 position = transform.position;
        Vector2 direction = Vector2.down;
        float distance = groundRaycastDistance;

        RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, groundLayer);
        if (hit.collider)
        {
            bool isTouchingIce = iceLayer.value == 1 << hit.collider.gameObject.layer;
            touchingIce = isTouchingIce;
            if (isTouchingIce && !landed)
            {
                extraXVelocity = rb2d.velocity.x / iceSlipperyReducer;
                touchingIce = true;
            }
            else if (!isTouchingIce) { extraXVelocity = 0; touchingIce = false; }
            
            bool isTouchingSlowness = slowLayer.value == 1 << hit.collider.gameObject.layer;
            if (isTouchingSlowness)
            {
                movSpeed = defaultMovSpeed / slowSpeed;
            }
            else movSpeed = defaultMovSpeed;

            if (!landed && rb2d.velocity.y == 0)
            {
                landed = true;
                jumping = false;
            }
            
            return true;
        }
        else
        {
            movSpeed = defaultMovSpeed;
            touchingIce = false;
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - groundRaycastDistance, transform.position.z));
    }

    public void Boost(float value, Vector3 direction)
    {
        rb2d.velocity = direction * value;
        jumping = false;
    }
    
    public void SetCanDoubleJump()
    {
        canDoubleJumpOnce = true;
    }

    public void SetFlight(float time)
    {
        flightTime = time + Time.timeSinceLevelLoad;
    }

    public bool GetJumpButton()
    {
        if (playerCount == PlayerCount.P1)
        {
            return Input.GetButton("P1_Jump");
        }
        else if (playerCount == PlayerCount.P2)
        {
            return Input.GetButton("P2_Jump");
        }
        else return false;
    }

    public void Stun(float time = 3)
    {
        stunned = true;
        Invoke("UnStun", time);
        animator.SetTrigger("knockback");
    }

    public void UnStun()
    {
        stunned = false;
    }

    public void AddKnockback(float value)
    {
        knockback += value;
        animator.SetTrigger("knockback");
    }

    public void Freeze(float time = 5)
    {
        freeze = true;
        Invoke("UnFreeze", time);
    }

    public void UnFreeze()
    {
        freeze = false;
    }
}
