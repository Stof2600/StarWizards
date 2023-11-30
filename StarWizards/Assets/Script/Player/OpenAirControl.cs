using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class OpenAirControl : MonoBehaviour
{
    GameManager GM;
    public bool P1Active, P2Active;

    public Vector3 P1Spawn, P2Spawn;

    public Transform Targets;
    public int TargetsAlive;

    bool WonLevel;

    private void Start()
    {
        GM = FindObjectOfType<GameManager>();

        if(!GM)
        {
            SceneManager.LoadScene(0);
        }
    }

    private void FixedUpdate()
    {
        TargetsAlive = Targets.childCount;

        if(TargetsAlive <= 0)
        {
            WonLevel = true;
        }
    }

    public void AssignPlayer(int PlayerID)
    {
        switch(PlayerID)
        {
            case 0:
                P1Active = true;
                break;
            case 1:
                P2Active = true;
                break;
        }
    }
}
