using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour {

    public static ScoreManager instance;

    public List<int> scores = new List<int>();
    private void Start()
    {
        if (instance != null) Destroy(this);
        else instance = this;

        scores = new List<int>();
        foreach(string player in Enum.GetNames(typeof(Player.PlayerCount)))
        {
            scores.Add(0);
        }
    }

    public void AddScore(Player.PlayerCount player)
    {
        if ((int)player < scores.Count)
        {
            scores[(int)player]++;
        }
    }
    public int GetScore(Player.PlayerCount player)
    {
        if ((int)player < scores.Count)
        {
            return scores[(int)player];
        }
        else return -1;
    }

    public void ResetScores()
    {
        scores = new List<int>();
        foreach (string player in Enum.GetNames(typeof(Player.PlayerCount)))
        {
            scores.Add(0);
        }
    }

}
