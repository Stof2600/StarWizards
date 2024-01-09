
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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

    bool WonLevel;

    public float PlayerSpeed = 10f, PlayerRotMulti = 30;
    public float LevelSpeed = 10f;
    public float EndPosition;
    public bool LeaveUpward;

    public Vector3 P1Spawn, P2Spawn;

    public Camera cam;
    public bool AlwaysForward;

    public bool TempOpenAir;
    Camera P1Cam, P2Cam;
    float P1CamTime, P2CamTime;
    float MaxCamTime = 1.5f;
    public OpenAreaGenerator OpenArea;
    public bool WaitForPosition;
    float MoveTimer;


    // Start is called before the first frame update
    void Start()
    {
        GM = FindAnyObjectByType<GameManager>();
        if(!GM)
        {
            SceneManager.LoadScene(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(TempOpenAir)
        {
            if (P1Active)
            {
                P1.GetComponent<PlayerControl>().MoveActive = true;
            }
            if (P2Active)
            {
                P2.GetComponent<PlayerControl>().MoveActive = true;
            }

            if(OpenArea.OpenAirDone)
            {
                if (P1Active)
                {
                    P1.GetComponent<PlayerControl>().MoveActive = false;
                }
                if (P2Active)
                {
                    P2.GetComponent<PlayerControl>().MoveActive = false;
                }

                MoveTimer = 2;
                WaitForPosition = true;
                TempOpenAir = false;
            }

            UpdateCameras();
            return;
        }
        else if(WaitForPosition)
        {
            bool P1Done = false, P2Done = false, HolderDone = false;
            MoveTimer -= Time.deltaTime;

            if (P1Active)
            {
                P1.transform.rotation = transform.rotation;
                P1.transform.position = Vector3.MoveTowards(P1.transform.position, transform.position + P1Spawn, MoveTimer);

                if(P1.transform.position == transform.position + P1Spawn)
                {
                    P1Done = true;
                }
            }
            else
            {
                P1Done = true;
            }
            if (P2Active)
            {
                P2.transform.rotation = transform.rotation;
                P2.transform.position = Vector3.MoveTowards(P2.transform.position, transform.position + P2Spawn, MoveTimer);

                if (P2.transform.position == transform.position + P2Spawn)
                {
                    P2Done = true;
                }
            }
            else
            {
                P2Done = true;
            }

            transform.position = Vector3.MoveTowards(transform.position, OpenArea.transform.position + new Vector3(0, 0, OpenArea.EndTPDis), MoveTimer);
            if(transform.position == OpenArea.transform.position + new Vector3(0, 0, OpenArea.EndTPDis))
            {
                HolderDone = true;
            }


            if(P1Done && P2Done)
            {
                WaitForPosition = false;
            }

            return;
        }

        if(!WonLevel)
        {
            ReadInput();

            if (transform.position.z >= EndPosition && !AlwaysForward)
            {
                WonLevel = true;
            }
        }
        else
        {
            WinAnimation();
        }

        if (P1Active)
        {
            P1Movement();
        }
        if (P2Active)
        {
            P2Movement();
        }

        if (P1Active || P2Active || AlwaysForward)
        {
            transform.position += new Vector3(0, 0, LevelSpeed) * Time.deltaTime;
        }

        EnemyDetecting();
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

        if(!WonLevel)
        {
            if (P1.localPosition.y >= limitY)
            {
                P1.localPosition = new Vector3(P1.localPosition.x, limitY, 0);
            }
            else if (P1.localPosition.y <= -limitY)
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

            PlayerCollisionCheck(P1);

            P1Model.localEulerAngles = new Vector3(-P1Input.y * PlayerRotMulti, P1Input.x * PlayerRotMulti, -P1Input.x * PlayerRotMulti);
        }
    }

    void P2Movement()
    {
        P2.localPosition += new Vector3(P2Input.x, P2Input.y, 0) * PlayerSpeed * Time.deltaTime;

        if(!WonLevel)
        {
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

            PlayerCollisionCheck(P2);

            P2Model.localEulerAngles = new Vector3(-P2Input.y * PlayerRotMulti, P2Input.x * PlayerRotMulti, -P2Input.x * PlayerRotMulti);
        }
    }

    void PlayerCollisionCheck(Transform Player)
    {
        bool LevelCheck = false;

        Collider[] Collisions = Physics.OverlapSphere(Player.position, 1.5f);
        foreach(Collider Col in Collisions)
        {
            if(Col.CompareTag("Level"))
            {
                LevelCheck = true;
            }
        }

        if(Physics.Linecast(transform.position, Player.position, out RaycastHit hit) && hit.transform.CompareTag("Level") && LevelCheck)
        {
            Player.position = hit.point;
        }
    }

    void EnemyDetecting()
    {
        EnemyControl[] Enemies = FindObjectsOfType<EnemyControl>();

        Plane[] planes;

        planes = GeometryUtility.CalculateFrustumPlanes(cam);

        foreach (EnemyControl t in Enemies)
        {
            Collider TCol = t.GetComponentInChildren<Collider>();

            if(!GeometryUtility.TestPlanesAABB(planes, TCol.bounds) && !t.OpenAir)
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

    void WinAnimation()
    {
        cam.transform.SetParent(null);
        LevelSpeed = 5;

        if(P1Active)
        {
            if (LeaveUpward)
            {
                P1Input = new Vector2(0, 1);
                P1Model.Rotate(-50 * Time.deltaTime,0, 0);
            }
            else
            {
                if (P1.position.x > 0)
                {
                    P1Input = new Vector2(1, 0);
                    P1Model.Rotate(0, 50 * Time.deltaTime, 0);
                }
                else
                {
                    P1Input = new Vector2(-1, 0);
                    P1Model.Rotate(0, -50 * Time.deltaTime, 0);
                }
            }
        }
        if(P2Active)
        {
            if(LeaveUpward)
            {
                P2Input = new Vector2(0, 1);
                P2Model.Rotate(-50 * Time.deltaTime, 0, 0);
            }
            else
            {
                if (P2.position.x > 0)
                {
                    P2Input = new Vector2(1, 0);
                    P2Model.Rotate(0, 50 * Time.deltaTime, 0);
                }
                else
                {
                    P2Input = new Vector2(-1, 0);
                    P2Model.Rotate(0, -50 * Time.deltaTime, 0);
                }
            }
        }

        PlayerControl[] Players = FindObjectsOfType<PlayerControl>();

        if (Players.Length > 0)
        {
            Plane[] planes;

            planes = GeometryUtility.CalculateFrustumPlanes(cam);

            foreach (PlayerControl p in Players)
            {
                Collider PCol = p.GetComponentInChildren<Collider>();

                if (!GeometryUtility.TestPlanesAABB(planes, PCol.bounds))
                {
                    switch(p.playerID)
                    {
                        case 0:
                            P1Active = false;
                            break;
                        case 1:
                            P2Active = false;
                            break;
                    }
                    Destroy(p.gameObject);
                }
            }
        }
        else
        {
            GM.AddProgress = true;
            GM.StartLoadScene(1);
        }
    }

    public void StartTempOpen(OpenAreaGenerator Area)
    {
        OpenArea = Area;
        TempOpenAir = true;
    }
    void UpdateCameras()
    {
        if (P1 && !P1Cam)
        {
            P1Cam = P1.GetComponentInChildren<Camera>();
        }
        if (P2 && !P2Cam)
        {
            P2Cam = P2.GetComponentInChildren<Camera>();
        }

        if (!P1Active && P1Cam)
        {
            P1CamTime += Time.deltaTime;

            if (P1CamTime >= MaxCamTime)
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

        if (P1Cam && P2Cam)
        {
            P1Cam.rect = new Rect(0, 0.5f, 1, 0.5f);
            P2Cam.rect = new Rect(0, 0, 1, 0.5f);
        }
        else
        {
            if (P1Cam && !P2Cam)
            {
                P1Cam.rect = new Rect(0, 0, 1, 1);
            }
            else if (P2Cam && !P1Cam)
            {
                P2Cam.rect = new Rect(0, 0, 1, 1);
            }
        }
    }
}
