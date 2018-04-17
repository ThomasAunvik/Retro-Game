using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jetpack : MonoBehaviour {
    [SerializeField] Jetpack otherJetpack;
    public float jetpackTime = 2;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player)
        {
            player.SetFlight(jetpackTime);
            if (otherJetpack) Destroy(otherJetpack.gameObject);
            Destroy(gameObject);
        }
    }
}
