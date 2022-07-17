using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int score = 0;
    public float speed = 1;
    public int lives = 3;
    [SerializeField] GameObject dicePrefab;
    [SerializeField] Knife knife;

    public void incrementScore()
    {
        ++score;
        speed = 1 + .1f * score;
        startWave();
    }

    public void startWave()
    {
        for (int i = 0; i < 1 + (score/5); ++i)
        {
            Dice dice = Instantiate(dicePrefab).GetComponent<Dice>();
            dice.knife = knife;
            dice.gm = this;
        }
    }

    private void Start()
    {
        startWave();
        Physics.autoSimulation = false;
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
    }
}
