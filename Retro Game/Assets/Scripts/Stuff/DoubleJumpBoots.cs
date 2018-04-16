using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJumpBoots : MonoBehaviour {
    [SerializeField] DoubleJumpBoots otherBoot;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player)
        {
            player.SetCanDoubleJump();
            if(otherBoot) Destroy(otherBoot.gameObject);
            Destroy(gameObject);
        }
    }
}
