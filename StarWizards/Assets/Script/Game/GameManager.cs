using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int TotalScore;
    public int VisualScore;

    public bool P1Active, P2Active;
    public bool InFlightControl;
    public GameObject P1Prefab, P2Prefab;
    public FlightControl FC;

    private void Start()
    {
        FC = FindAnyObjectByType<FlightControl>();
        if (FC)
        {
            InFlightControl = true;
        }
    }

    private void Update()
    {
        CheckForSpawn();
        ScoreCounter();
    }

    void ScoreCounter()
    {
        if(VisualScore < TotalScore)
        {
            VisualScore += 1;
        }
    }

    void CheckForSpawn()
    {
        if(!P1Active && Input.GetButtonDown("P1Fire"))
        {
            SpawnPlayer(0);
        }
        if (!P2Active && Input.GetButtonDown("P2Fire"))
        {
            SpawnPlayer(1);
        }
    }

    void SpawnPlayer(int PlayerID)
    {
        if(InFlightControl)
        {
            PlayerControl PC = null;

            switch (PlayerID)
            {
                case 0:
                    PC = Instantiate(P1Prefab, FC.transform.position + FC.P1Spawn, FC.transform.rotation, FC.transform).GetComponent<PlayerControl>();
                    FC.AssignPlayer(PC, PlayerID);
                    P1Active = true;
                    break; 
                case 1:
                    PC = Instantiate(P2Prefab, FC.transform.position + FC.P2Spawn, FC.transform.rotation, FC.transform).GetComponent<PlayerControl>();
                    FC.AssignPlayer(PC, PlayerID);
                    P2Active = true;
                    break;
                
            }
        }
    }

    public void ResetPlayer(int PlayerID)
    {
        switch (PlayerID)
        {
            case 0:
                P1Active = false;

                if (InFlightControl)
                {
                    FC.P1 = null;
                    FC.P1Model = null;
                    FC.P1Active = false;
                }
                break;
            case 2:
                P2Active = false;

                if (InFlightControl)
                {
                    FC.P2 = null;
                    FC.P2Model = null;
                    FC.P2Active = false;
                }
                break;
        }
    }
}
