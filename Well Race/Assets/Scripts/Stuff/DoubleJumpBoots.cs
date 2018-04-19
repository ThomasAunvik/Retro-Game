using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJumpBoots : MonoBehaviour {
    [SerializeField] DoubleJumpBoots otherBoot;

    public AudioClip onCollectedAudio;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player)
        {
            player.SetCanDoubleJump();

            SoundManager.PlayAudio(onCollectedAudio);

            if (otherBoot) Destroy(otherBoot.gameObject);
            Destroy(gameObject);
        }
    }
}
