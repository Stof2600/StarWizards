using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : StatObject
{
    public float FireTimerMin = 0.7f, FireTimerMax = 1.5f;
    public float ShootCountdown;

    public bool StaticEnemy;

    public float TimeCount;

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
            FireProjectiles();

            ShootCountdown = Random.Range(FireTimerMin, FireTimerMax);
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
        for (int i = 0; i < ProjectileDirections.Count; i++)
        {
            Vector3 Dir = ProjectileDirections[i];
            GameObject Proj = Instantiate(EnemyProjectilePrefab, transform.position, transform.rotation);
            Proj.transform.Rotate(-Dir.y, Dir.x, 0);
        }
    }

}
