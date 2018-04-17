using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {

    public float minX;
    public float maxX;

    public float movSpeed;

    public float platforms;

    Vector3 initPosition;

    public bool otherSide = false;

    bool reverse;
    private void Awake()
    {
        initPosition = transform.position;
    }

	void Update () {
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

        transform.Translate((reverse ? Vector3.left : Vector3.right) * movSpeed * Time.deltaTime);
	}

    private void OnDrawGizmos()
    {
        if (initPosition == Vector3.zero) initPosition = transform.position;

        Vector3 startPos = initPosition != Vector3.zero ? initPosition : transform.position;
        if (!otherSide)
        {
            startPos.y -= 12.5f;
            startPos.x -= minX;
        }
        else
        {
            startPos.y -= 12.5f;
            startPos.x += minX;
        }

        Vector3 endPos = initPosition != Vector3.zero ? initPosition : transform.position;
        if (!otherSide)
        {
            endPos.y += 12.5f;
            endPos.x += maxX + (platforms * 32);
        }
        else
        {
            endPos.y += 12.5f;
            endPos.x -= maxX + (platforms * 32);
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
