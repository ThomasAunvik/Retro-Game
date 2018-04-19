using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndingTrigger : MonoBehaviour {

    [SerializeField] EndingTrigger otherTrigger;
    public bool wasFirst = false;

    public Canvas scoreCanvas;
    public Text P1Score;
    public Text P2Score;
    public Image P1Image;
    public Image P2Image;

    public Text P1WinText;
    public Text P2WinText;

    public Color winColor;
    public Color looseColor;

    private void Start()
    {
        scoreCanvas.enabled = false;
        P1WinText.text = "";
        P2WinText.text = "";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(1);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player && !otherTrigger.wasFirst && !wasFirst)
        {
            wasFirst = true;
            StartCoroutine(FinishGame(player.GetPlayerCount));
        }
    }

    IEnumerator FinishGame(Player.PlayerCount player)
    {
        if (ScoreManager.instance)
        {
            ScoreManager.instance.AddScore(player);
        }

        if (GameManager.instance) GameManager.instance.FreezeAll(100);

        yield return new WaitForSecondsRealtime(1);

        scoreCanvas.enabled = true;
        if (ScoreManager.instance)
        {
            float scoreP1 = ScoreManager.instance.GetScore(Player.PlayerCount.P1);
            float scoreP2 = ScoreManager.instance.GetScore(Player.PlayerCount.P2);
            P1Score.text = (scoreP1 - (player == Player.PlayerCount.P1 && scoreP1 > 0 ? 1 : 0)).ToString();
            P2Score.text = (scoreP2 - (player == Player.PlayerCount.P2 && scoreP2 > 0 ? 1 : 0)).ToString();

            P1Image.color = looseColor;
            P2Image.color = looseColor;
        }

        yield return new WaitForSeconds(1);

        if (ScoreManager.instance)
        {
            P1Score.text = ScoreManager.instance.GetScore(Player.PlayerCount.P1).ToString();
            P2Score.text = ScoreManager.instance.GetScore(Player.PlayerCount.P2).ToString();

            P1Image.color = (player == Player.PlayerCount.P1 ? winColor : looseColor);
            P2Image.color = (player == Player.PlayerCount.P2 ? winColor : looseColor);

            if (ScoreManager.instance.GetScore(player) == 10){
                if (player == Player.PlayerCount.P1)
                {
                    P1WinText.text = "WIN";
                }else if(player == Player.PlayerCount.P2)
                {
                    P2WinText.text = "WIN";
                }
            }else { P1WinText.text = ""; P2WinText.text = ""; }
        }

        yield return new WaitForSeconds(2);

        if (ScoreManager.instance && ScoreManager.instance.GetScore(player) < 10)
        {
            int currentScene = SceneManager.GetActiveScene().buildIndex;
            int newScene = Random.Range(2, SceneManager.sceneCountInBuildSettings);
            while(currentScene == newScene)
            {
                newScene = Random.Range(2, SceneManager.sceneCountInBuildSettings);
            }
            SceneManager.LoadScene(newScene);
        }
        else SceneManager.LoadScene(1);
    }
}
