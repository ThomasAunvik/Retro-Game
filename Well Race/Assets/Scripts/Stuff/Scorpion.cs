using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scorpion : MonoBehaviour {
    public float stunTime = 2;
    public float stunDelay = 10;
    float stunTimer;

    public float minX;
    public float maxX;

    public float movSpeed;

    Vector3 initPosition;

    public Vector2 imageSize;

    public bool otherSide = false;

    bool reverse;

    SpriteRenderer spriteRenderer;
    Animator animator;
    
    private void Awake()
    {
        initPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        otherSide = GetComponentInParent<PlayArea>().transform.rotation.y == 1;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        animator.enabled = !GameManager.instance.freeze;
        if (GameManager.instance.freeze) return;

        if (transform.position.y != initPosition.y) initPosition.y = transform.position.y;
        if (!otherSide)
        {
            if (initPosition.x - minX > transform.position.x)
            {
                reverse = false;
            }
            else if (initPosition.x + maxX < transform.position.x)
            {
                reverse = true;
            }
        }
        else
        {
            if (initPosition.x + minX < transform.position.x)
            {
                reverse = false;
            }
            else if (initPosition.x - maxX > transform.position.x)
            {
                reverse = true;
            }
        }

        spriteRenderer.flipX = reverse;
        transform.Translate((reverse ? Vector3.left : Vector3.right) * movSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player)
        {
            if(Time.timeSinceLevelLoad > stunTimer && !GameManager.instance.freeze)
            {
                stunTimer = Time.timeSinceLevelLoad + stunDelay;
                player.Stun(stunTime);
                player.PlayHitSound();
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (initPosition == Vector3.zero) initPosition = transform.position;

        Vector3 startPos = initPosition != Vector3.zero ? initPosition : transform.position;
        if (!otherSide)
        {
            startPos.y -= imageSize.y / 2;
            startPos.x -= minX + imageSize.x / 2;
        }
        else
        {
            startPos.y -= imageSize.y / 2;
            startPos.x += minX + imageSize.x / 2;
        }

        Vector3 endPos = initPosition != Vector3.zero ? initPosition : transform.position;
        if (!otherSide)
        {
            endPos.y += imageSize.y / 2;
            endPos.x += maxX + imageSize.x / 2;
        }
        else
        {
            endPos.y += imageSize.y / 2;
            endPos.x -= maxX + imageSize.x / 2;
        }

        Vector3 point1 = new Vector3(startPos.x, startPos.y, startPos.z);
        Vector3 point2 = new Vector3(startPos.x, endPos.y, startPos.z);
        Vector3 point3 = new Vector3(endPos.x, endPos.y, endPos.z);
        Vector3 point4 = new Vector3(endPos.x, startPos.y, endPos.z);

        Gizmos.DrawLine(point1, point2);
        Gizmos.DrawLine(point2, point3);
        Gizmos.DrawLine(point3, point4);
        Gizmos.DrawLine(point4, point1);
    }
}
