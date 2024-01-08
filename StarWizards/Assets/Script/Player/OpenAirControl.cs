using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class OpenAirControl : MonoBehaviour
{
    GameManager GM;
    public bool P1Active, P2Active;

    public Transform P1, P2;
    public Camera P1Cam, P2Cam;

    public Vector3 P1Spawn, P2Spawn;

    public Transform Targets;
    public int TargetsAlive;

    public float WinTimer = 1.5f;
    float P1CamTime, P2CamTime, MaxCamTime = 1.5f;
    bool ChangedCameras;

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
            Win();
        }

        UpdateCameras();
    }

    public void AssignPlayer(int PlayerID, Transform NewPlayer)
    {
        switch(PlayerID)
        {
            case 0:
                P1Active = true;
                P1 = NewPlayer;
                break;
            case 1:
                P2Active = true;
                P2 = NewPlayer;
                break;
        }
    }

    public void Win()
    {
        foreach (Camera c in GetComponentsInChildren<Camera>())
        {
            c.transform.SetParent(null);
        }

        WinTimer -= Time.deltaTime;

        if(WinTimer <= 0)
        {
            GM.AddProgress = true;
            GM.StartLoadScene(1);
        }

        
    }

    void UpdateCameras()
    {
        if(P1 && !P1Cam)
        {
            P1Cam = P1.GetComponentInChildren<Camera>();
        }
        if(P2 && !P2Cam)
        {
            P2Cam = P2.GetComponentInChildren<Camera>();
        }

        if (!P1Active && P1Cam)
        {
            P1CamTime += Time.deltaTime;

            if(P1CamTime >= MaxCamTime)
            {
                Destroy(P1Cam.gameObject);
                P1Cam = null;
                P1CamTime = 0;
            }
        }
        if (!P2Active && P2Cam)
        {
            P2CamTime += Time.deltaTime;

            if (P2CamTime >= MaxCamTime)
            {
                Destroy(P2Cam.gameObject);
                P2Cam = null;
                P2CamTime = 0;
            }
        }

        if(P1Cam && P2Cam)
        {
            P1Cam.rect = new Rect(0, 0.5f, 1, 0.5f);
            P2Cam.rect = new Rect(0, 0, 1, 0.5f);
        }
        else
        {
            if(P1Cam && !P2Cam)
            {
                P1Cam.rect = new Rect(0, 0, 1, 1);
            }
            else if(P2Cam && !P1Cam)
            {
                P2Cam.rect = new Rect(0, 0, 1, 1);
            }
        }
    }
}
