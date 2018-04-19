using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public float bulletSpeed = 1;
    public float knockback = 10;
    public float stunTime = 1;

    public AudioClip onFireSound;

    Vector3 direction;

    [HideInInspector]
    public Cannon cannon;

    [HideInInspector]
    public float range;

    float distanceReached = 0;
    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        direction = transform.InverseTransformDirection(transform.right);

        audioSource.clip = onFireSound;
        audioSource.Play();
    }

    void Update () {
        if (!GameManager.instance.freeze)
        {
            transform.Translate(direction * bulletSpeed * Time.deltaTime);
            distanceReached += bulletSpeed * Time.deltaTime;
        }

        if(distanceReached > range)
        {
            Destroy(gameObject);
        }
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject != cannon.gameObject)
        {

            Player player = collision.gameObject.GetComponent<Player>();
            if (player)
            {
                player.AddKnockback(transform.right.x * knockback);
                player.Stun(stunTime);
                player.PlayHitSound();
            }
            Destroy(gameObject);
        }
    }
}
