using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int score = 0;
    public float speed = 1;
    [SerializeField] GameObject dicePrefab;
    [SerializeField] Knife knife;
    List<Dice> liveDice = new List<Dice>();
    int allowedNumFrames = 300;

    public void incrementScore()
    {
        ++score;
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
    }

    private void Start()
    {
        startWave();
        Physics.autoSimulation = false;
    }

    private void Lose()
    {
        Debug.Log("lmao");
    }

    public void FixedUpdate()
    {
        float timeRem = speed;
        while (timeRem >= 1)
        {
            timeRem -= 1;
            Physics.Simulate(Time.fixedDeltaTime);
        }
        if (timeRem > 0)
        {
            Physics.Simulate(Time.fixedDeltaTime * (timeRem/1));
        }
        if (liveDice.Count == 0)
        {
            return;
        }

        if (liveDice.Any(d => d.readyToCut) && !knife.animating)
        {
            Debug.Log(allowedNumFrames);
            if (--allowedNumFrames == 0)
            {
                Lose();
                return;
            }
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
