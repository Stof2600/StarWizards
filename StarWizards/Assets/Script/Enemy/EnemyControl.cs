using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : StatObject
{
    public float FireTimerMin = 0.7f, FireTimerMax = 1.5f;
    public float ShootCountdown;

    Transform Target;

    public bool OpenAir;
    public bool StaticEnemy;

    public Transform FirePoint;
    public Transform AimBase, AimRot;

    public float RotateSpeed = 20;

    public float TimeCount;
    public float FireRate = 0;

    bool ChasePlayer;

    float FireTime;
    bool DoFire;
    int CurrentProjectile;

    public GameObject EnemyProjectilePrefab;

    public List<Vector3> ProjectileDirections = new List<Vector3>();

    Vector3 PatrolPoint;
    Vector3 PatrolLimit;
    Vector3 MovePoint;

    float ChaseTimer, ChaseStart;

    Transform Cam, PlayerTarget;

    public Transform TargetArrow;

    // Start is called before the first frame update
    void Start()
    {
        Setup();

        FlightControl FC = FindObjectOfType<FlightControl>();
        if(FC)
        {
            Cam = FC.transform;
        }

        ShootCountdown = Random.Range(FireTimerMin, FireTimerMax);

        if(OpenAir)
        {
            ChaseStart = Random.Range(10, 100);
            GetOpenArea();
        }
        PlayerTarget = null;
    }

    // Update is called once per frame
    void Update()
    {
        MoveForward();
        RunHitAnim();

        if(TargetArrow && Target)
        {
            TargetArrowControl();
        }

        if(OpenAir)
        {
            OpenAirMovement();
        }

        if((OpenAir && ChasePlayer) || !OpenAir)
        {
            ShootCountdown -= Time.deltaTime;
        }

        if (ShootCountdown <= 0)
        {
            FireTime = FireRate;
            CurrentProjectile = 0;
            DoFire = true;

            ShootCountdown = Random.Range(FireTimerMin, FireTimerMax);
        }

        if(DoFire)
        {
            FireProjectiles();
        }

        if(AimBase || AimRot)
        {
            SmartAim();
        }

        if (StaticEnemy)
        {
            MoveSpeed = 0;
            return;
        }

        if(!Cam)
        {
            return;
        }

        float Distance = Vector3.Distance(Cam.position, transform.position);

        float XRotFixed = transform.eulerAngles.x;

        if (Distance <= 15 && (XRotFixed > 360 - 90 || XRotFixed == 0))
        {
            MoveSpeed = 10;

            transform.Rotate(-0.5f, 0, 0);
        }
    }

    void FireProjectiles()
    {
        FireTime += Time.deltaTime;

        if(FireTime >= FireRate)
        {
            if(CurrentProjectile == ProjectileDirections.Count)
            {
                DoFire = false;
                return;
            }

            Quaternion FireRot = transform.rotation;

            if(AimRot)
            {
                FireRot = AimRot.rotation;
            }

            Vector3 Dir = ProjectileDirections[CurrentProjectile];
            GameObject Proj = Instantiate(EnemyProjectilePrefab, FirePoint.position, FireRot);
            Proj.transform.Rotate(-Dir.y, Dir.x, 0);

            CurrentProjectile += 1;
            FireTime = 0;
        }
    }

    void SmartAim()
    {
        if (!Target)
        {
            Target = FindTarget();
            return;
        }

        if (AimRot && AimBase)
        {
            AimBase.LookAt(new Vector3(Target.position.x, AimBase.position.y, Target.position.z));
            AimRot.LookAt(Target.position);
        }
        else if(AimRot && !AimBase)
        {
            AimRot.LookAt(Target.position);
        }
        else if(!AimRot && AimBase)
        {
            AimBase.LookAt(new Vector3(Target.position.x, AimBase.position.y, Target.position.z));
        }
    }

    void OpenAirMovement()
    {
        if(MoveSpeed < 15)
        {
            MoveSpeed = 15;
        }

        Debug.DrawLine(transform.position, MovePoint);

        if(MovePoint != Vector3.zero)
        {
            Quaternion TargetRot = Quaternion.LookRotation(MovePoint - transform.position);

            transform.rotation = Quaternion.Slerp(transform.rotation, TargetRot, Time.deltaTime * RotateSpeed);
        }

        if(!ChasePlayer)
        {
            if (MovePoint == Vector3.zero || Vector3.Distance(transform.position, MovePoint) < 10)
            {
                MovePoint = PatrolPoint + new Vector3(Random.Range(-PatrolLimit.x, PatrolLimit.x), Random.Range(-PatrolLimit.y, PatrolLimit.y), Random.Range(-PatrolLimit.z, PatrolLimit.z));
                if(Physics.Linecast(transform.position, MovePoint))
                {
                    MovePoint = Vector3.zero;
                }
            }

            if(ChaseTimer < ChaseStart)
            {
                ChaseTimer += Time.deltaTime;
            }
            else
            {
                PlayerTarget = FindTarget();

                if(PlayerTarget)
                {
                    ChasePlayer = true;
                }
            }
        }
        else
        {
            if(ChaseTimer > 20)
            {
                ChaseTimer = 20;
            }

            if(PlayerTarget)
            {
                MovePoint = PlayerTarget.position;
            }
            else
            {
                ChaseTimer = 0;
            }

            ChaseTimer -= Time.deltaTime;

            if(ChaseTimer <= 0)
            {
                PlayerTarget = null;
                ChasePlayer = false;
                ChaseStart = Random.Range(10, 200);
            }
        }
    }

    Transform FindTarget()
    {
        Transform ReturnValue = null;

        PlayerControl[] Targets = FindObjectsOfType<PlayerControl>();
        float LastDis = Mathf.Infinity;

        foreach(PlayerControl PC in Targets)
        {
            float Dis = Vector3.Distance(transform.position, PC.transform.position);

            if(Dis < LastDis)
            {
                LastDis = Dis;
                ReturnValue = PC.transform;
            }
        }

        return ReturnValue;
    }

    void TargetArrowControl()
    {
        TargetArrow.LookAt(Target.position);
        float Scale = Vector3.Distance(transform.position, Target.position) / 50;

        if(Scale < 1)
        {
            Scale = 0;
        }

        TargetArrow.localScale = new Vector3(Scale, Scale, Scale);
    }

    void GetOpenArea()
    {
        BoxCollider Area = GameObject.FindGameObjectWithTag("Border").GetComponent<BoxCollider>();

        PatrolPoint = Area.transform.position + Area.center;
        PatrolLimit = Area.size * 0.5f;
    }
}
