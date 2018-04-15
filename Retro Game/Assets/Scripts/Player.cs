using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    enum PlayerCount
    {
        P1, P2
    }

    [SerializeField] PlayerCount playerCount = PlayerCount.P1;
    [SerializeField] Camera playerCamera;
    [SerializeField] float cameraYOffset;

    [SerializeField] float movSpeed = 0.5f;
    [SerializeField] float jumpForce = 1;
    
    Rigidbody2D rb2d;
    Animator animator;

    void Start () {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        var height = 2 * Camera.main.orthographicSize;
        var width = height * Camera.main.aspect;

        Debug.Log("Width: " + width + ", Height: " + height);
    }

    float extraXVelocity;
	void Update ()
    {
        Vector2 newPosition = rb2d.position;
        bool jump = false;
        float moveAxis = 0;

        if (playerCount == PlayerCount.P1)
        {
            moveAxis = Input.GetAxis("P1_Horizontal");
            jump = Input.GetButton("P1_Jump");
        }
        else if(playerCount == PlayerCount.P2)
        {
            moveAxis = Input.GetAxis("P2_Horizontal");
            jump = Input.GetButton("P2_Jump");
        }

        bool isGrounded = IsGrounded();
        if (jump && isGrounded)
        {
            Vector2 Movement = new Vector2(rb2d.velocity.x, jumpForce);
            rb2d.velocity = Movement;
            extraXVelocity = 0;
            landed = false;
        }

        animator.SetBool("inAir", !isGrounded);

        if (extraXVelocity != 0) extraXVelocity += moveAxis / iceSlipperyReducer;
        rb2d.velocity = new Vector2((moveAxis * movSpeed) + extraXVelocity, rb2d.velocity.y);
        
        Vector3 cameraPos = playerCamera.transform.position;
        float velocity = 0;
        cameraPos.y = Mathf.SmoothDamp(playerCamera.transform.position.y, rb2d.position.y + cameraYOffset, ref velocity, 0.05f);
        playerCamera.transform.position = cameraPos;

        if (moveAxis > 0)
        {
            transform.rotation = new Quaternion(0, 0, 0, 0);
            animator.SetBool("isWalking", true);
        }
        else if (moveAxis < 0)
        {
            transform.rotation = new Quaternion(0, 1, 0, 0);
            animator.SetBool("isWalking", true);
        }
        else animator.SetBool("isWalking", false);
    }


    public LayerMask groundLayer;
    public LayerMask iceLayer;
    public float groundRaycastDistance = 1;
    public float iceSlipperyReducer = 2;

    bool landed = false;
    bool IsGrounded()
    {
        Vector2 position = transform.position;
        Vector2 direction = Vector2.down;
        float distance = groundRaycastDistance;

        RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, groundLayer);
        if (hit.collider)
        {
            Debug.Log(iceLayer.value == 1 << hit.collider.gameObject.layer);
            bool isTouchingIce = iceLayer.value == 1 << hit.collider.gameObject.layer;
            if (isTouchingIce && !landed)
            {
                extraXVelocity = rb2d.velocity.x / iceSlipperyReducer;
            }
            else if (!isTouchingIce) extraXVelocity = 0;

            if (!landed && rb2d.velocity.y == 0)
            {
                landed = true;
            }

            return true;
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - groundRaycastDistance, transform.position.z));
    }
}
