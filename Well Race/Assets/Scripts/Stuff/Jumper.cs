using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumper : MonoBehaviour {

    public float jumpForce = 100;
    Animator animator;

    bool canFire = true;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player && canFire)
        {
            player.Boost(jumpForce, transform.up);
            animator.SetTrigger("Fire");
            canFire = false;
        }
    }

    public void SetCanFire()
    {
        canFire = true;
    }
}
