
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine;
using JetBrains.Annotations;

public class FlightControl : MonoBehaviour
{
    public GameManager GM;

    public Transform P1, P2;
    public Transform P1Model, P2Model;

    public float limitX, limitY;

    Vector2 P1Input, P2Input;
    
    public bool P1Active, P2Active;

    public float PlayerSpeed = 10f, PlayerRotMulti = 30;
    public float LevelSpeed = 10f;

    public Vector3 P1Spawn, P2Spawn;

    public Slider P1HealthBar, P2HealthBar;
    public Text ScoreText;

    public Camera cam;
   

    // Start is called before the first frame update
    void Start()
    {
        GM = FindAnyObjectByType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        ReadInput();

        if (P1Active)
        {
            P1Movement();
        }
        if (P2Active)
        {
            P2Movement();
        }

        transform.position += new Vector3(0, 0, LevelSpeed) * Time.deltaTime;

        EnemyDetecting();

        UIControl();
    }

    void ReadInput()
    {
        P1Input.x = Input.GetAxis("P1H");
        P1Input.y = Input.GetAxis("P1V");

        P2Input.x = Input.GetAxis("P2H");
        P2Input.y = Input.GetAxis("P2V");
    }
    
    void P1Movement()
    {
        P1.localPosition += new Vector3(P1Input.x, P1Input.y, 0) * PlayerSpeed * Time.deltaTime;

        if(P1.localPosition.y >= limitY)
        {
            P1.localPosition = new Vector3(P1.localPosition.x, limitY, 0);
        }
        else if(P1.localPosition.y <= -limitY)
        {
            P1.localPosition = new Vector3(P1.localPosition.x, -limitY, 0);
        }
        if (P1.localPosition.x >= limitX)
        {
            P1.localPosition = new Vector3(limitX, P1.localPosition.y, 0);
        }
        else if (P1.localPosition.x <= -limitX)
        {
            P1.localPosition = new Vector3(-limitX, P1.localPosition.y, 0);
        }

        P1Model.localEulerAngles = new Vector3(-P1Input.y * PlayerRotMulti, P1Input.x * PlayerRotMulti, -P1Input.x * PlayerRotMulti);
    }

    void P2Movement()
    {
        P2.localPosition += new Vector3(P2Input.x, P2Input.y, 0) * PlayerSpeed * Time.deltaTime;

        if (P2.localPosition.y >= limitY)
        {
            P2.localPosition = new Vector3(P2.localPosition.x, limitY, 0);
        }
        else if (P2.localPosition.y <= -limitY)
        {
            P2.localPosition = new Vector3(P2.localPosition.x, -limitY, 0);
        }
        if (P2.localPosition.x >= limitX)
        {
            P2.localPosition = new Vector3(limitX, P2.localPosition.y, 0);
        }
        else if (P2.localPosition.x <= -limitX)
        {
            P2.localPosition = new Vector3(-limitX, P2.localPosition.y, 0);
        }

        P2Model.localEulerAngles = new Vector3(-P2Input.y * PlayerRotMulti, P2Input.x * PlayerRotMulti, -P2Input.x * PlayerRotMulti);
    }

    void EnemyDetecting()
    {
        EnemyControl[] Enemies = FindObjectsOfType<EnemyControl>();

        Plane[] planes;

        planes = GeometryUtility.CalculateFrustumPlanes(cam);

        foreach (EnemyControl t in Enemies)
        {
            Collider TCol = t.GetComponentInChildren<Collider>();

            if(!GeometryUtility.TestPlanesAABB(planes, TCol.bounds))
            {
                Destroy(t.gameObject);
            }
        }
    }

    public void AssignPlayer(PlayerControl NewPlayer, int PlayerID)
    {
        switch (PlayerID)
        {
            case 0:
                P1 = NewPlayer.transform;
                P1Model = NewPlayer.Model;
                NewPlayer.MoveActive = false;
                NewPlayer.playerID = 0;
                P1Active = true;
                break;
            case 1:
                P2 = NewPlayer.transform;
                P2Model = NewPlayer.Model;
                NewPlayer.MoveActive = false;
                NewPlayer.playerID = 1;
                P2Active = true;
                break;
        }
    }

    void UIControl()
    {
        if(P1Active)
        {
            HealthScript P1H = P1.GetComponent<HealthScript>();
            P1HealthBar.maxValue = P1H.MaxHealth;
            P1HealthBar.value = P1H.Health;
        }
        else
        {
            P1HealthBar.value = 0;
        }
        if(P2Active)
        {
            HealthScript P2H = P2.GetComponent<HealthScript>();
            P2HealthBar.maxValue = P2H.MaxHealth;
            P2HealthBar.value = P2H.Health;
        }
        else
        {
            P2HealthBar.value = 0;
        }

        ScoreText.text = "SCORE\n" + GM.VisualScore.ToString("000000000");
    }
}
