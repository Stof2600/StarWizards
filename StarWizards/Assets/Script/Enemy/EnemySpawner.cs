using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public bool CanActive;
    Transform PlayerHolder;

    public float LimitX, LimitY;

    public int EnemyAmount;

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
        for (int i = 0; i < EnemyAmount; i++)
        {
            Vector3 SpawnPosition = new Vector3(Random.Range(-LimitX, LimitX), Random.Range(-LimitY, LimitY), transform.position.z);
            while(!ClearArea(SpawnPosition))
            {
                SpawnPosition = new Vector3(Random.Range(-LimitX, LimitX), Random.Range(-LimitY, LimitY), transform.position.z);
            }
            
            Instantiate(EnemyPrefab, SpawnPosition, new Quaternion(transform.rotation.y + 180, transform.rotation.x, transform.rotation.z, transform.rotation.w));
        }
        
        Destroy(gameObject);
    }

    bool ClearArea(Vector3 CheckPos)
    {
        Collider[] ColList = Physics.OverlapSphere(CheckPos, 1.5f);

        if(ColList.Length > 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
