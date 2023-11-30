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

    public float PatrolRadius = 10, ChaseRadius = 30;
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
    Vector3 MovePoint;

    Transform Cam, PlayerTarget;

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
        PatrolPoint = transform.position;
        PlayerTarget = null;
    }

    // Update is called once per frame
    void Update()
    {
        MoveForward();
        RunHitAnim();

        if(OpenAir)
        {
            OpenAirMovement();
        }

        if((OpenAir && !ChasePlayer) || !OpenAir)
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
        if(MoveSpeed == 0)
        {
            MoveSpeed = 10;
        }

        Debug.DrawLine(transform.position, MovePoint);

        if(MovePoint != Vector3.zero)
        {
            Quaternion TargetRot = Quaternion.LookRotation(MovePoint - transform.position);

            transform.rotation = Quaternion.Slerp(transform.rotation, TargetRot, Time.deltaTime * RotateSpeed);
        }

        if(!ChasePlayer)
        {
            if(PlayerTarget)
            {
                ChasePlayer = true;
                return;
            }

            if(MovePoint == Vector3.zero)
            {
                MovePoint = PatrolPoint + new Vector3(Random.Range(-PatrolRadius, PatrolRadius), Random.Range(-PatrolRadius, PatrolRadius), Random.Range(-PatrolRadius, PatrolRadius));
            }
            else if(Vector3.Distance(transform.position, MovePoint) < 3f)
            {
                MovePoint = Vector3.zero;
            }

            Collider[] NearbyCheck = Physics.OverlapSphere(transform.position, PatrolRadius);
            foreach (Collider Col in NearbyCheck)
            {
                if (Col.GetComponentInParent<PlayerControl>())
                {
                    PlayerTarget = Col.transform;
                    return;
                }
            }
        }
        else
        {
            MovePoint = PlayerTarget.position;

            if(Vector3.Distance(transform.position, PlayerTarget.position) > ChaseRadius)
            {
                PatrolPoint = transform.position;
                PlayerTarget = null;
                ChasePlayer = false;
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, PatrolRadius);
        Gizmos.DrawWireCube(PatrolPoint, new Vector3(PatrolRadius, PatrolRadius, PatrolRadius));
    }
}
