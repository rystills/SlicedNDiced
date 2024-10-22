using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int score = 0;
    public float speed = 1;
    [SerializeField] GameObject dicePrefab;
    [SerializeField] Knife knife;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI scoreOutline;
    [SerializeField] Animator scoreAnimator;
    [SerializeField] GameObject titleCard;
    [SerializeField] GameObject startButton;
    [SerializeField] GameObject retryButton;
    [SerializeField] GameObject diecIcon;
    List<Dice> liveDice = new List<Dice>();
    int allowedNumFrames = 300;
    int maxAllowedFrames = 300;
    bool active = false;
    static int scoreSave = -1;


    private void SetScoreText()
    {
        scoreText.text = score.ToString();
        scoreOutline.text = score.ToString();
    }
    public void incrementScore()
    {
        ++score;
        SetScoreText();
        scoreAnimator.SetBool("score_up", true);
        scoreAnimator.playbackTime = 0;
        scoreAnimator.Play("score_increase");
        scoreAnimator.SetBool("score_up", false);
        speed = 1 + .1f * score;
        startWave();
    }

    public void startWave()
    {
        liveDice.Clear();
        for (int i = 0; i < 1 + (score/5); ++i)
        {
            Dice dice = Instantiate(dicePrefab).GetComponent<Dice>();
            liveDice.Add(dice);
            dice.transform.Translate(new Vector3(0,1,0) * (5f*i));
            dice.knife = knife;
            dice.gm = this;
        }
        allowedNumFrames = (int)(200 * (1 / (speed / 4f)));
        maxAllowedFrames = allowedNumFrames;
    }
    void Start()
    {
        diecIcon.transform.localScale = Vector3.one;
        if (scoreSave != -1)
        {
            score = scoreSave;
            retryButton.SetActive(true);
            startButton.SetActive(false);
            SetScoreText();
        }
    }

    private void StartGame()
    {
        titleCard.SetActive(false);
        startButton.SetActive(false);
        retryButton.SetActive(false);
        active = true;
        score = 0;
        SetScoreText();
        speed = 1;
        startWave();
        Physics.autoSimulation = false;
    }

    private void Lose()
    {
        diecIcon.transform.localScale = Vector3.one;
        liveDice.ForEach(d => Destroy(d));
        liveDice = new List<Dice>();
        active = false;
        titleCard.SetActive(true);
        retryButton.SetActive(true);
        scoreSave = score;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Update()
    {
        if (active)
            return;
        if (Input.GetMouseButtonDown(0))
        {
            StartGame();
        }
    }

    public void FixedUpdate()
    {
        if (!active)
        {
            return;
        }
        float timeRem = speed;
        while (timeRem >= 1)
        {
            timeRem -= 1;
            Physics.Simulate(Time.fixedDeltaTime);
        }
        if (timeRem > 0)
        {
            Physics.Simulate(Time.fixedDeltaTime * (timeRem / 1));
        }
        if (liveDice.Count == 0)
        {
            return;
        }

        if (liveDice.Any(d => d.readyToCut) && !knife.animating)
        {
            //Debug.Log(allowedNumFrames);
            if (--allowedNumFrames == 0)
            {
                Lose();
                return;
            }
            diecIcon.transform.localScale = new Vector3(allowedNumFrames / (float)maxAllowedFrames, allowedNumFrames / (float)maxAllowedFrames, allowedNumFrames / (float)maxAllowedFrames);
        }
        if (!knife.animating && liveDice.Any(d => d.readyToCut && d.cutRenderers.Count > d.targetCutCount))
        {
            Lose();
        }

        if (!knife.animating)
        {
            foreach (Dice d in liveDice)
            {
                if (!d.readyToCut || d.cutRenderers.Count != d.targetCutCount)
                {
                    return;
                }
            }
            knife.animate(liveDice.SelectMany(d => d.Slice()).ToList());
        }
    }
}
