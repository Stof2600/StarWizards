using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int TotalScore;
    public int VisualScore;

    private void Update()
    {
        ScoreCounter();
    }

    void ScoreCounter()
    {
        if(VisualScore < TotalScore)
        {
            VisualScore += 1;
        }
    }
}
