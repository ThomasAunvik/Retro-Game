using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public float bulletSpeed = 1;
    public float knockback = 10;
    Vector3 direction;

    private void Start()
    {
        direction = transform.InverseTransformDirection(transform.right);
    }

    void Update () {
        transform.Translate(direction * bulletSpeed * Time.deltaTime);
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player)
        {
            player.AddKnockback(direction.x * knockback * bulletSpeed);
            Debug.Log("Adding Force: " + (direction.x * knockback));
        }
        Destroy(gameObject);
    }
}
