using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : StatObject
{
    public int playerID;
    public bool MoveActive;

    public float RotateSpeed = 50f;
    public float AnimRot = 20;
    public Transform Model;
    public GameObject PlayerCam;

    Vector2 PlayerInput;

    public GameObject ProjectilePrefab;
    public GameObject BombProjPrefab;

    public SpriteRenderer ShortReticle, LongReticle;
    public Transform AimOrigin;

    public int BombCount;
    public Color ReticleColor;

    float KillTimer;
    float SpawnTimer;

    private void Start()
    {
        Setup();

        BombCount = 1;
        SpawnTimer = 1;
    }

    void Update()
    {
        ReadInput();
        GetComponent<Rigidbody>().velocity = Vector3.zero;

        if (MoveActive)
        {
            Movement();
            ModelAnim();

            if (PlayerCam.transform.parent == transform)
            {
                PlayerCam.SetActive(true);
            }
            

            KillTimer += Time.deltaTime;

            if(KillTimer >= 5)
            {
                TakeDamage(MaxHealth);
            }
        }
        else
        {
            PlayerCam.SetActive(false);
        }

        if(SpawnTimer > 0)
        {
            SpawnTimer -= Time.deltaTime;
        }
        else
        {
            WallInFrontCheck();
        }

        ReticleMove();
        RunHitAnim();
    }

    void ReadInput()
    {
        if(playerID == 0)
        {
            PlayerInput.x = Input.GetAxis("P1H");
            PlayerInput.y = Input.GetAxis("P1V");

            if (Input.GetButtonDown("P1Fire"))
            {
                Fire(ProjectilePrefab);
            }
            if (Input.GetButtonDown("P1Bomb") && BombCount > 0)
            {
                Fire(BombProjPrefab);
                BombCount--;   
            }
        }
        else
        {
            PlayerInput.x = Input.GetAxis("P2H");
            PlayerInput.y = Input.GetAxis("P2V");

            if (Input.GetButtonDown("P2Fire"))
            {
                Fire(ProjectilePrefab);
            }
            if (Input.GetButtonDown("P2Bomb") && BombCount > 0)
            {
                Fire(BombProjPrefab);
                BombCount--;
            }
        }
    }

    void Movement()
    {
        MoveForward();
        transform.Rotate(PlayerInput.y * RotateSpeed * Time.deltaTime, PlayerInput.x * RotateSpeed * Time.deltaTime, 0);
    }

    void Fire(GameObject Projectile)
    {
        Quaternion FireRot = Quaternion.LookRotation(LongReticle.transform.forward, transform.up);
        if(MoveActive)
        {
            FireRot = Model.rotation;
        }

        Instantiate(Projectile, Model.position, FireRot);
    }

    void ModelAnim()
    {
        Model.localEulerAngles = new Vector3(PlayerInput.y * AnimRot, PlayerInput.x * AnimRot, -PlayerInput.x * AnimRot);
    }

    void ReticleMove()
    {
        if(Physics.Raycast(transform.position, transform.forward, out RaycastHit Hit) && Hit.transform.GetComponentInParent<EnemyControl>())
        {
            ShortReticle.color = Color.red;
            LongReticle.color = Color.red;
        }
        else
        {
            ShortReticle.color = ReticleColor;
            LongReticle.color = ReticleColor;
        }

        float Offset = 1f;
        if(MoveActive)
        {
            AimOrigin = PlayerCam.transform;
            Offset = 1.2f;
        }
        else
        {
            AimOrigin = GetComponentInParent<FlightControl>().cam.transform;
            Offset = 0.7f;
        }

        Vector3 ToPlayerDir = (transform.position + transform.forward * 20) - AimOrigin.position;
        LongReticle.transform.position = AimOrigin.position + ToPlayerDir * Offset;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BombPickup"))
        {
            BombCount++;
            Destroy(other.gameObject);
            return;
        }
        if(other.CompareTag("HealthPickup"))
        {
            if(Health < MaxHealth)
            {
                Health += 1;
            }

            Destroy(other.gameObject);
            return;
        }

        if (!MoveActive && other.CompareTag("Border"))
        {
            FlightControl FC = FindObjectOfType<FlightControl>();
            FC.StartTempOpen(other.GetComponentInChildren<OpenAreaGenerator>());
        }

        if(!other.GetComponentInParent<PlayerControl>() && !other.GetComponentInParent<ProjectileScript>() && !other.CompareTag("Border"))
        {
            GetComponent<PlayerControl>().TakeDamage(1);
        }

        if (other.GetComponentInParent<EnemyControl>())
        {
            other.GetComponentInParent<EnemyControl>().TakeDamage(1);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Border"))
        {
            KillTimer = 0;
        }
    }

    void WallInFrontCheck()
    {
        if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 0.7f) && hit.transform.CompareTag("Level"))
        {
            TakeDamage(Health);
        }
    }
}
