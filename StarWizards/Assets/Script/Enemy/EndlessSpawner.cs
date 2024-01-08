using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessSpawner : MonoBehaviour
{
    public FlightControl PlayerHolder;

    public float LimitX, LimitY;

    public int SpawnCount;
    public int Difficulty;
    public GameObject[] EnemyPrefabs;

    public float SpawnTimer;

    // Start is called before the first frame update
    void Start()
    {
        Difficulty = 0;
        SpawnCount = 2;
        PlayerHolder = FindObjectOfType<FlightControl>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = PlayerHolder.transform.position + Vector3.forward * 50;

        if(SpawnTimer > 0 && (PlayerHolder.P1Active || PlayerHolder.P2Active) && !PlayerHolder.TempOpenAir)
        {
            SpawnTimer -= Time.deltaTime;
        }
        else if(SpawnTimer <= 0)
        {
            SpawnEnemy();
            SpawnTimer = Random.Range(5.0f, 7.0f);
        }
    }

    void SpawnEnemy()
    {
        SpawnCount++;

        if(SpawnCount >= 10)
        {
            SpawnCount = 3;
            Difficulty += 1;

            if(Difficulty >= EnemyPrefabs.Length)
            {
                Difficulty = EnemyPrefabs.Length - 1;
            }
        }

        for (int i = 0; i < SpawnCount % 10; i++)
        {
            int EnemySelect = Random.Range(0, Difficulty + 1);

            Vector3 SpawnPosition = new Vector3(Random.Range(-LimitX, LimitX), Random.Range(-LimitY, LimitY), transform.position.z);
            if(ClearArea(SpawnPosition))
            {
                Instantiate(EnemyPrefabs[EnemySelect], SpawnPosition, new Quaternion(transform.rotation.x, transform.rotation.y + 180, transform.rotation.z, transform.rotation.w));
            }
        }
    }

    bool ClearArea(Vector3 CheckPos)
    {
        Collider[] ColList = Physics.OverlapSphere(CheckPos, 1.5f);

        if (ColList.Length > 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
