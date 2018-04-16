using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance;

    public Player theFreezer;
    public bool freeze;

    private void Awake()
    {
        instance = this;
    }

    public void Freeze(Player theFreezer, float time = 3)
    {
        this.theFreezer = theFreezer;
        freeze = true;

        Invoke("UnFreeze", time);
    }

    void UnFreeze()
    {
        freeze = false;
        theFreezer = null;
    }
}
