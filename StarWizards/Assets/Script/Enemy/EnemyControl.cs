using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    public float ShootCountdown = 1;
    public float MoveSpeed = 5;

    public GameObject EnemyProjectilePrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * MoveSpeed * Time.deltaTime;

        ShootCountdown -= Time.deltaTime;

        if (ShootCountdown <= 0)
        {
            Instantiate(EnemyProjectilePrefab, transform.position, transform.rotation);

            ShootCountdown = 1;
        }
    }


}
