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

    float KillTimer;
    float SpawnTimer;

    private void Start()
    {
        Setup();

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
                Fire();
            }
        }
        else
        {
            PlayerInput.x = Input.GetAxis("P2H");
            PlayerInput.y = Input.GetAxis("P2V");

            if (Input.GetButtonDown("P2Fire"))
            {
                Fire();
            }
        }
    }

    void Movement()
    {
        MoveForward();
        transform.Rotate(PlayerInput.y * RotateSpeed * Time.deltaTime, PlayerInput.x * RotateSpeed * Time.deltaTime, 0);
    }

    void Fire()
    {
        Quaternion FireRot = transform.rotation;
        if(MoveActive)
        {
            FireRot = Model.rotation;
        }

        Instantiate(ProjectilePrefab, Model.position, FireRot);
    }

    void ModelAnim()
    {
        Model.localEulerAngles = new Vector3(PlayerInput.y * AnimRot, PlayerInput.x * AnimRot, -PlayerInput.x * AnimRot);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!MoveActive && other.CompareTag("Border"))
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
