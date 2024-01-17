using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool InFirstScene, LoadedNewScene;

    public int GameProgress;
    public bool AddProgress;

    public int TotalScore;
    public int VisualScore;
    int ScoreCountHelper;

    public bool P1Active, P2Active;
    public bool P1Dead, P2Dead;
    public bool InFlightControl, InOpenControl;
    public GameObject P1Prefab, P2Prefab;
    public FlightControl FC;
    public OpenAirControl OAC;

    public Slider P1HPBar, P2HPBar;
    public Text ScoreText;
    public Text LivesText;
    public Text P1BombText, P2BombText;
    public GameObject FCHud;

    public GameObject MenuScreen;
    public GameObject GameOverScreen;
    public Text EndScore;

    public GameObject MapSelector;
    public MultiButtonSelect[] MissionButtons;

    public bool TransitionActive;
    public float TransitionSpeed = 4;
    float TransitionTime, LoadTime;
    bool PlayedTransitionEffect;
    public RectTransform TransitionBarTop, TransitionBarBottom;
    public Vector3 TranstionClosedPos, TranstionOpenPos;
    public ParticleSystem TransitionEffect;
    public Transform[] LoadingGears;

    public float[] UIYPosFlight, UIYPosOpen;
    public RectTransform[] UIObjects;

    int NextScene;

    public int MissionLives;

    float ResetTimer;
    bool DoReset;

    LevelGenerator LG;
    int MultipleCheck;
    bool DidScoreCall;

    [Header("HIGHSCORE STUFF")]
    public Text HighscoreDisplay;
    int[] DisplayScores;
    string[] DisplayNames;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        GameOverScreen.SetActive(false);

        DisplayScores = new int[5];
        LoadScores();

        GameProgress = 0;

        NextScene = -1;
        TransitionActive = false;
        PlayedTransitionEffect = false;

        MissionLives = 5;
        MultipleCheck = 1;

        if (InFirstScene)
        {
            StartLoadScene(1);
        }   
    }

    private void Update()
    {
        if(DoReset)
        {
            ResetAnim();
            ResetTimer -= Time.deltaTime;

            if (ResetTimer <= 0 || Input.GetButtonDown("P1Fire") || Input.GetButtonDown("P2Fire"))
            {
                SaveScores();
                TotalScore = 0;
                VisualScore = 0;
                MissionLives = 5;
                MultipleCheck = 1;
                DoReset = false;
                GameProgress = 0;
                MenuScreen.SetActive(true);
                StartLoadScene(1);
            }
            return;
        }

        if(LoadedNewScene)
        {
            print("LOADED SCENE");
            GameOverScreen.SetActive(false);

            LoadScores();
            LoadTime = 0;

            P1Dead = false;
            P2Dead = false;

            if(AddProgress)
            {
                GameProgress += 1;
                AddProgress = false;
            }

            InFlightControl = false;
            InOpenControl = false;

            DidScoreCall = false;

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
        if (!OAC)
        {
            OAC = FindObjectOfType<OpenAirControl>();
            if (OAC)
            {
                InOpenControl = true;

                SpawnPlayer(0);
            }
        }
        if(!LG)
        {
            LG = FindObjectOfType<LevelGenerator>();
        }

        CheckForSpawn();
        CheckLives();

        ScoreCounter();

        UIControl();

        TransitionAnim();

        if(Input.GetKeyDown(KeyCode.Z))
        {
            GameProgress += 1;
        }
        if(Input.GetKeyDown(KeyCode.X))
        {
            ResetScores();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            TotalScore += 50;
        }

        if (LG && !DidScoreCall && TotalScore > (100 * MultipleCheck))
        {
            if(FC && !FC.TempOpenAir)
            {
                print("RequestOpen");
                LG.RequestOpenArea(MultipleCheck * 2);
            }
            MultipleCheck += 1;
            DidScoreCall = true;
        }
        else if(TotalScore % 100 != 0 && DidScoreCall)
        {
            DidScoreCall = false;
        }
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

        if(TotalScore > 999999)
        {
            TotalScore = 999999;
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
        if(MissionLives <= 0)
        {
            return;
        }

        if(InFlightControl || FC)
        {
            PlayerControl PC = null;

            switch (PlayerID)
            {
                case 0:
                    PC = Instantiate(P1Prefab, FC.transform.position + FC.P1Spawn, FC.transform.rotation, FC.transform).GetComponent<PlayerControl>();
                    FC.AssignPlayer(PC, PlayerID);
                    P1Active = true;
                    if (P1Dead)
                    {
                        ChangeLives(-1, true);
                        P1Dead = false;
                    }
                    break; 
                case 1:
                    PC = Instantiate(P2Prefab, FC.transform.position + FC.P2Spawn, FC.transform.rotation, FC.transform).GetComponent<PlayerControl>();
                    FC.AssignPlayer(PC, PlayerID);
                    P2Active = true;
                    if (P2Dead)
                    {
                        ChangeLives(-1, true);
                        P2Dead = false;
                    }
                    break;
                
            }
        }
        if(InOpenControl && OAC)
        {
            Transform NewP;

            switch (PlayerID)
            {
                case 0:
                    NewP = Instantiate(P1Prefab, OAC.transform.position + OAC.P1Spawn, OAC.transform.rotation, OAC.transform).transform;
                    OAC.AssignPlayer(PlayerID, NewP);
                    P1Active = true;
                    if (P1Dead)
                    {
                        ChangeLives(-1, true);
                        P1Dead = false;
                    }
                    break;
                case 1:
                    NewP = Instantiate(P2Prefab, OAC.transform.position + OAC.P2Spawn, OAC.transform.rotation, OAC.transform).transform;
                    OAC.AssignPlayer(PlayerID, NewP);
                    P2Active = true;
                    if (P2Dead)
                    {
                        ChangeLives(-1, true);
                        P2Dead = false;
                    }
                    break;

            }
        }
    }

    void UIControl()
    {

        string DeadText = MissionLives > 0 ? "FIRE BUTTON TO START" : "OUT OF LIVES";
        if(!P1Active)
        {
            P1HPBar.value = 0;
            P1HPBar.gameObject.SetActive(false);
            P1BombText.text = DeadText;
        }
        if(!P2Active)
        {
            P2HPBar.value = 0;
            P2HPBar.gameObject.SetActive(false);
            P2BombText.text = DeadText;
        }

        if(OAC || (FC && FC.TempOpenAir))
        {
            FCHud.SetActive(true);
            MapSelector.SetActive(false);

            if(FC && FC.TempOpenAir)
            {
                InOpenControl = true;
                InFlightControl = false;
            }

            ScoreText.text = "SCORE\n" + VisualScore.ToString("000000");

            LivesText.text = "LIVES: " + MissionLives.ToString("00");

            if (P1Active)
            {
                PlayerControl P1C = null;
                if(FC && FC.TempOpenAir)
                {
                    P1C = FC.P1.GetComponent<PlayerControl>();
                }
                else
                {
                    P1C = OAC.P1.GetComponent<PlayerControl>();
                }

                P1HPBar.gameObject.SetActive(true);
                P1HPBar.maxValue = P1C.MaxHealth;
                P1HPBar.value = P1C.Health;

                P1BombText.text = "BOMBS: " + P1C.BombCount.ToString();
            }
            if (P2Active)
            {
                PlayerControl P2C = null;
                if (FC && FC.TempOpenAir)
                {
                    P2C = FC.P2.GetComponent<PlayerControl>();
                }
                else
                {
                    P2C = OAC.P2.GetComponent<PlayerControl>();
                }

                P2HPBar.gameObject.SetActive(true);
                P2HPBar.maxValue = P2C.MaxHealth;
                P2HPBar.value = P2C.Health;

                P2BombText.text = "BOMBS: " + P2C.BombCount.ToString();
            }
        }
        else if(FC)
        {
            FCHud.SetActive(true);
            MapSelector.SetActive(false);

            InOpenControl = false;
            InFlightControl = true;

            ScoreText.text = "SCORE\n" + VisualScore.ToString("000000");

            LivesText.text = "LIVES: " + MissionLives.ToString("00");

            if (FC.transform.position.z >= FC.EndPosition)
            {
                return;
            }

            if (P1Active)
            {
                PlayerControl P1C = FC.P1.GetComponent<PlayerControl>();
                P1HPBar.gameObject.SetActive(true);
                P1HPBar.maxValue = P1C.MaxHealth;
                P1HPBar.value = P1C.Health;
                P1BombText.text = "BOMBS: " + P1C.BombCount.ToString();
            }
            if (P2Active)
            {
                PlayerControl P2C = FC.P2.GetComponent<PlayerControl>();
                P2HPBar.gameObject.SetActive(true);
                P2HPBar.maxValue = P2C.MaxHealth;
                P2HPBar.value = P2C.Health;
                P2BombText.text = "BOMBS: " + P2C.BombCount.ToString();
            }
        }
        else
        {
            FCHud.SetActive(false);

            if(!InFirstScene)
            {
                MapSelector.SetActive(true);

                for (int i = 0; i < MissionButtons.Length; i++)
                {
                    if (i <= GameProgress)
                    {
                        MissionButtons[i].Enable();
                    }
                    else
                    {
                        MissionButtons[i].Disable();
                    }
                }
            }
            else
            {
                MapSelector.SetActive(false);
            }
        }

        for(int i = 0; i < UIObjects.Length; i++)
        {
            RectTransform t = UIObjects[i];

            if(InFlightControl || (InOpenControl && ((P1Active && !P2Active) || (!P1Active && P2Active))))
            {
                t.localPosition = new Vector3(t.localPosition.x, UIYPosFlight[i], t.localPosition.z);
                P2BombText.alignment = TextAnchor.UpperCenter;
            }
            else if (InOpenControl && P2Active && P1Active)
            {
                t.localPosition = new Vector3(t.localPosition.x, UIYPosOpen[i], t.localPosition.z);
                P2BombText.alignment = TextAnchor.LowerCenter;
            }
        }
    }

    public void ResetPlayer(int PlayerID)
    {
        switch (PlayerID)
        {
            case 0:
                P1Active = false;
                P1Dead = true;

                if (InFlightControl || FC)
                {
                    FC.P1 = null;
                    FC.P1Model = null;
                    FC.P1Active = false;
                }
                if(InOpenControl && OAC)
                {
                    OAC.P1Active = false;
                    OAC.P1 = null;
                }
                break;
            case 1:
                P2Active = false;
                P2Dead = true;

                if (InFlightControl || FC)
                {
                    FC.P2 = null;
                    FC.P2Model = null;
                    FC.P2Active = false;
                }
                if (InOpenControl && OAC)
                {
                    OAC.P2Active = false;
                    OAC.P2 = null;
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

    public void ToMissionSelect()
    {
        TransitionActive = true;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ChangeLives(int Amount, bool MissionMode)
    {
        switch (MissionMode)
        {
            case true:
                MissionLives += Amount;
                break;
        }
    }

    void CheckLives()
    {
        if(MissionLives <= 0 && !P1Active && !P2Active)
        {
            ResetTimer = 5;
            DoReset = true;
        }
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

        if(TransitionActive && TransitionTime >= 1)
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
            if(LoadTime >= 2 && NextScene > -1)
            {
                LoadScene();
            }
            else if(LoadTime >= 2 && NextScene == -1)
            {
                LoadedNewScene = true;
                MenuScreen.SetActive(false);
            }
        }
    }

    void ResetAnim()
    {
        MenuScreen.SetActive(false);
        FCHud.SetActive(false);

        GameOverScreen.SetActive(true);
        EndScore.text = "SCORE : " + TotalScore.ToString("000000");
    }

    void SaveScores()
    {
        for (int i = 0; i < DisplayScores.Length; i++)
        {
            string Key = "score" + i;
            PlayerPrefs.SetInt(Key, DisplayScores[i]);
        }

        PlayerPrefs.SetInt("score5", TotalScore);
        PlayerPrefs.Save();

        LoadScores();
    }
    void LoadScores()
    {
        int[] LoadedScores = new int[6];

        for(int i = 0; i < 6; i++)
        {
            string ScoreKey = "score" + i;
            LoadedScores[i] = PlayerPrefs.GetInt(ScoreKey);
        }

        for (int j = 0; j < 10; j++)
        {
            for (int i = 0; i < LoadedScores.Length; i++)
            {
                if (i - 1 >= 0)
                {
                    int S1 = LoadedScores[i - 1];
                    int S2 = LoadedScores[i];

                    if (S1 < S2)
                    {
                        LoadedScores[i - 1] = S2;
                        LoadedScores[i] = S1;
                    }
                }
            }
        }

        string HSDText = "";
        for (int i = 0; i < DisplayScores.Length; i++)
        {
            DisplayScores[i] = LoadedScores[i];

            HSDText += (i + 1).ToString("00") + ": " + DisplayScores[i].ToString("000000") + "\n";
        }

        HighscoreDisplay.text = HSDText;
    }
    void ResetScores()
    {
        for (int i = 0; i < DisplayScores.Length; i++)
        {
            DisplayScores[i] = 0;
            string Key = "score" + i;
            PlayerPrefs.SetInt(Key, 0);
        }

        PlayerPrefs.SetInt("score5", 0);

        PlayerPrefs.Save();
        LoadScores();
    }
}

[System.Serializable]
public class MultiButtonSelect
{
    public GameObject Button1, Button2;

    public void Enable()
    {
        Button1.SetActive(true);
        if(!Button2)
        {
            return;
        }
        Button2.SetActive(true);
    }
    public void Disable()
    {
        Button1.SetActive(false);
        if (!Button2)
        {
            return;
        }
        Button2.SetActive(false);
    }
}
