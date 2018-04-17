using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Play : MonoBehaviour {

    [Header("Menu")]
    public GameObject play;
    public GameObject quit;
    public GameObject spike1;
    public GameObject spike2;
    private Image playimg;
    private Image quitimg;
    public Sprite playimg1;
    public Sprite quitimg1;
    public Sprite playimg2;
    public Sprite quitimg2;
    int stage = 1;

    [Header("Scores")]
    public Canvas scoreCanvas;
    public Text P1Score;
    public Text P2Score;
    public Image P1Image;
    public Image P2Image;

    void Start()
    {
        playimg = play.GetComponent<Image>();
        quitimg = quit.GetComponent<Image>();

        Cursor.visible = false;

        scoreCanvas.enabled = false;

        if (ScoreManager.instance)
        {
            ScoreManager.instance.ResetScores();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown("w") || (Input.GetKeyDown("up")))
        {
            if (stage != 2)
            {
                stage += 1;
            }
            else if (stage == 2)
            {
                stage = 1;
            }
        }

        if (Input.GetKeyDown("s") || (Input.GetKeyDown("down")))
        {
            if (stage == 2)
            {
                stage -= 1;
            }
            else if (stage == 1)
            {
                stage = 2;
            }
        }

        if (stage == 1)
        {
            spike2.SetActive(false);
            spike1.SetActive(true);
            playimg.sprite = playimg1;
            quitimg.sprite = quitimg2;
        }
        else if (stage == 2)
        {
            spike2.SetActive(true);
            spike1.SetActive(false);
            playimg.sprite = playimg2;
            quitimg.sprite = quitimg1;
        }

        if (Input.GetKeyDown(KeyCode.Space) || (Input.GetKeyDown(KeyCode.Return)))
        {
            if (stage == 1) StartCoroutine(StartGame());
            else if (stage == 2) Application.Quit();
        }
    }

    IEnumerator StartGame()
    {
        scoreCanvas.enabled = true;
        if (ScoreManager.instance)
        {
            P1Score.text = ScoreManager.instance.GetScore(Player.PlayerCount.P1).ToString();
            P2Score.text = ScoreManager.instance.GetScore(Player.PlayerCount.P2).ToString();
        }

        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(Random.Range(2, SceneManager.sceneCountInBuildSettings), LoadSceneMode.Single);
    }
}
