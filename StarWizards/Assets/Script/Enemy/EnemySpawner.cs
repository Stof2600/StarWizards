using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public bool CanActive;
    Transform PlayerHolder;

    public GameObject EnemyPrefab;

    // Start is called before the first frame update
    void Start()
    {
        PlayerHolder = FindAnyObjectByType<FlightControl>().transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float Distance = Vector3.Distance(PlayerHolder.position, transform.position);

        if (Distance <= 100)
        {
            CanActive = true;
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        Instantiate(EnemyPrefab, transform.position, new Quaternion(transform.rotation.y + 180, transform.rotation.x, transform.rotation.z, transform.rotation.w));
        Destroy(gameObject);
    }
}
