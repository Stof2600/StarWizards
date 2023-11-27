
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

    bool WonLevel;

    public float PlayerSpeed = 10f, PlayerRotMulti = 30;
    public float LevelSpeed = 10f;
    public float EndPosition;
    public bool LeaveUpward;

    public Vector3 P1Spawn, P2Spawn;

    public Camera cam;
   

    // Start is called before the first frame update
    void Start()
    {
        GM = FindAnyObjectByType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!WonLevel)
        {
            ReadInput();

            if (transform.position.z >= EndPosition)
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

        if (P1Active || P2Active)
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

            PlayerCollisionCheck(P1);

            P2Model.localEulerAngles = new Vector3(-P2Input.y * PlayerRotMulti, P2Input.x * PlayerRotMulti, -P2Input.x * PlayerRotMulti);
        }
    }

    void PlayerCollisionCheck(Transform Player)
    {
        if(Physics.Linecast(transform.position, Player.position, out RaycastHit hit))
        {
            if(!hit.transform.GetComponent<PlayerControl>())
            {
                Player.position = hit.point;
            }
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
                NewPlayer.AimPoint = cam.transform;
                P1Active = true;
                break;
            case 1:
                P2 = NewPlayer.transform;
                P2Model = NewPlayer.Model;
                NewPlayer.MoveActive = false;
                NewPlayer.playerID = 1;
                NewPlayer.AimPoint = cam.transform;
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
            P1.GetComponent<PlayerControl>().ReticleActive = false;

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
            P2.GetComponent<PlayerControl>().ReticleActive = false;

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
}
