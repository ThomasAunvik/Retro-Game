using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freeze : MonoBehaviour {

    public Freeze otherFreezer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player)
        {
            GameManager.instance.Freeze(player);
            Destroy(otherFreezer.gameObject);
            Destroy(gameObject);
        }
    }
}
