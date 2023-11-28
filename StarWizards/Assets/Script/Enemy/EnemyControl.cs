using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : StatObject
{
    public float FireTimerMin = 0.7f, FireTimerMax = 1.5f;
    public float ShootCountdown;

    Transform Target;

    public bool StaticEnemy;

    public Transform FirePoint;
    public Transform AimBase, AimRot;

    public float TimeCount;
    public float FireRate = 0;

    float FireTime;
    bool DoFire;
    int CurrentProjectile;

    public GameObject EnemyProjectilePrefab;

    public List<Vector3> ProjectileDirections = new List<Vector3>();

    Transform Cam;

    // Start is called before the first frame update
    void Start()
    {
        Cam = FindAnyObjectByType<FlightControl>().transform;
        ShootCountdown = Random.Range(FireTimerMin, FireTimerMax);
    }

    // Update is called once per frame
    void Update()
    {
        MoveForward();

        ShootCountdown -= Time.deltaTime;

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
}
