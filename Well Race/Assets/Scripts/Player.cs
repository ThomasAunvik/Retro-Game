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

    [Header("Movement")]
    public LayerMask groundLayer;
    public LayerMask iceLayer;
    public LayerMask slowLayer;

    public float groundRaycastDistance = 1;
    public float iceSlipperyReducer = 2;
    public float slowSpeed = 2;
    
    [Header("Jumping")]
    [SerializeField] float jumpForce = 1;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2;

    bool landed = false;
    bool touchingIce = false;
    
    float extraXVelocity;

    Rigidbody2D rb2d;
    Animator animator;
    SpriteRenderer spriteRenderer;
    BoxCollider2D bCollider2D;

    float defaultMovSpeed;
    float gravForce;

    public float jumpVelocity = 70;

    bool jumping = false;
    public bool IsJumping { get { return jumping; } }

    bool canDoubleJumpOnce = false;
    float flightTime = 0;

    bool stunned = false;
    public bool IsStunned() { return stunned; }

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
        bCollider2D = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (GameManager.instance)
        {
            animator.enabled = !GameManager.instance.freeze;
            rb2d.simulated = !GameManager.instance.freeze;
        }
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
            
            landed = false;
            touchingIce = false;
            jumping = true;

            if (!isGrounded) canDoubleJumpOnce = false;
        }
        
        if (!isGrounded)
        {
            if (rb2d.velocity.y < 0)
            {
                rb2d.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }
            else if (rb2d.velocity.y > 0)
            {
                rb2d.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
        }

        animator.SetBool("inAir", !isGrounded);

        if (extraXVelocity != 0 || touchingIce) extraXVelocity += moveAxis / iceSlipperyReducer;

        if(!GameManager.instance.freeze) rb2d.velocity = new Vector2(((touchingIce ? 0 : moveAxis) * movSpeed) + knockback + extraXVelocity, rb2d.velocity.y);

        float velocity = 0;
        knockback = Mathf.SmoothDamp(knockback, 0, ref velocity, 0.1f);

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
        Vector2 leftPosition = transform.position;
        leftPosition.x -= bCollider2D.size.x / 2;

        Vector2 rightPosition = transform.position;
        rightPosition.x += bCollider2D.size.x / 2;

        Vector2 direction = Vector2.down;
        float distance = groundRaycastDistance;

        RaycastHit2D leftHit = Physics2D.Raycast(leftPosition, direction, distance, groundLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(rightPosition, direction, distance, groundLayer);
        RaycastHit2D middleHit = Physics2D.Raycast(position, direction, distance, groundLayer);
        if (leftHit.collider || rightHit.collider || middleHit.collider)
        {
            RaycastHit2D hit = middleHit;
            if (middleHit.collider) hit = middleHit;
            else if (leftHit.collider) hit = leftHit;
            else if (rightHit.collider) hit = rightHit;

            bool isTouchingIce = iceLayer.value == 1 << hit.collider.gameObject.layer;
            touchingIce = isTouchingIce;
            if (isTouchingIce && !landed)
            {
                extraXVelocity = rb2d.velocity.x / iceSlipperyReducer;
                touchingIce = true;
            }
            else if (!isTouchingIce) { extraXVelocity = 0; touchingIce = false; movSpeed = defaultMovSpeed; }
            
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
        if (bCollider2D)
        {
            Vector2 position = transform.position;
            Vector2 leftPosition = transform.position;
            leftPosition.x -= bCollider2D.size.x / 2;

            Vector2 rightPosition = transform.position;
            rightPosition.x += bCollider2D.size.x / 2;

            Vector2 downPosition = new Vector2(position.x, position.y - groundRaycastDistance);
            Vector2 leftDownPosition = new Vector2(leftPosition.x, leftPosition.y - groundRaycastDistance);
            Vector2 rightDownPosition = new Vector2(rightPosition.x, rightPosition.y - groundRaycastDistance);

            Gizmos.DrawLine(position, downPosition);
            Gizmos.DrawLine(leftPosition, leftDownPosition);
            Gizmos.DrawLine(rightPosition, rightDownPosition);

            Gizmos.DrawLine(leftDownPosition, rightDownPosition);
        }
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

    public void Stun(float time = 3, bool stopVelocity = false)
    {
        stunned = true;
        CancelInvoke("UnStun");
        Invoke("UnStun", time);
        animator.SetTrigger("knockback");

        if (stopVelocity) { extraXVelocity = 0; knockback = 0; }
    }

    public void UnStun()
    {
        stunned = false;
    }

    public void AddKnockback(float value)
    {
        if (!GameManager.instance.freeze) knockback += value;
        animator.SetTrigger("knockback");

        spriteRenderer.flipX = value > 0;
    }

    public void Freeze(float time = 5, bool stopVelocity = false)
    {
        freeze = true;
        CancelInvoke("UnFreeze");
        Invoke("UnFreeze", time);

        if (stopVelocity) { extraXVelocity = 0; }
    }

    public void UnFreeze()
    {
        freeze = false;
    }
}
