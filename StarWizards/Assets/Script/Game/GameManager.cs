using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool InFirstScene, LoadedNewScene;

    public int TotalScore;
    public int VisualScore;
    int ScoreCountHelper;

    public bool P1Active, P2Active;
    public bool InFlightControl;
    public GameObject P1Prefab, P2Prefab;
    public FlightControl FC;

    public bool TransitionActive;
    public float TransitionSpeed = 4;
    float TransitionTime, LoadTime;
    bool PlayedTransitionEffect;
    public RectTransform TransitionBarTop, TransitionBarBottom;
    public Vector3 TranstionClosedPos, TranstionOpenPos;
    public ParticleSystem TransitionEffect;
    public Transform[] LoadingGears;

    int NextScene;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        NextScene = -1;
        TransitionActive = false;
        PlayedTransitionEffect = false;

        if(InFirstScene)
        {
            StartLoadScene(1);
        }
    }

    private void Update()
    {
        if(LoadedNewScene)
        {
            print("LOADED SCENE");

            LoadTime = 0;

            TransitionActive = false;
            PlayedTransitionEffect = false;
            LoadedNewScene = false;
        }

        if(!FC)
        {
            FC = FindObjectOfType<FlightControl>();
            if (FC)
            {
                InFlightControl = true;

                SpawnPlayer(0);
            }
        }

        CheckForSpawn();
        ScoreCounter();

        TransitionAnim();
    }

    void ScoreCounter()
    {
        if(VisualScore < TotalScore)
        {
            ScoreCountHelper += 1;

            if(VisualScore + ScoreCountHelper <= TotalScore)
            {
                VisualScore += ScoreCountHelper;
            }
            else
            {
                ScoreCountHelper = 0;
            }
        }
        else
        {
            ScoreCountHelper = 0;
        }
    }

    public void AddScore(int Amount)
    {
        TotalScore += Amount;
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

    public void StartLoadScene(int SceneID)
    {
        NextScene = SceneID;
        TransitionActive = true;
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(NextScene);
        NextScene = -1;
        LoadedNewScene = true;
        InFirstScene = false;
    }

    void TransitionAnim()
    {
        if(TransitionActive && TransitionTime < 1)
        {
            TransitionTime += Time.deltaTime * TransitionSpeed;
        }
        else if(!TransitionActive && TransitionTime > 0)
        {
            TransitionTime -= Time.deltaTime * TransitionSpeed;
        }

        TransitionBarTop.localPosition = Vector3.Lerp(TranstionOpenPos, TranstionClosedPos, TransitionTime);
        TransitionBarBottom.localPosition = Vector3.Lerp(-TranstionOpenPos, -TranstionClosedPos, TransitionTime);

        if(NextScene > -1 && TransitionActive && TransitionTime >= 1)
        {
            LoadTime += Time.deltaTime;

            foreach(Transform Gear in LoadingGears)
            {
                Gear.Rotate(0, 0, 1);
            }

            if(!PlayedTransitionEffect)
            {
                TransitionEffect.Play();
                PlayedTransitionEffect = true;
            }
            if(LoadTime >= 2)
            {
                LoadScene();
            }
        }
    }
}
