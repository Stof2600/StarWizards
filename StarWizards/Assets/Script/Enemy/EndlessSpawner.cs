using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessSpawner : MonoBehaviour
{
    public FlightControl PlayerHolder;
    public GameManager GM;

    public float LimitX, LimitY;

    public float SpawnCount;
    public int Difficulty;
    public DifficultyLevel[] DifficultyLevels;

    public float SpawnTimer;

    // Start is called before the first frame update
    void Start()
    {
        Difficulty = 0;
        PlayerHolder = FindObjectOfType<FlightControl>();
        GM = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = PlayerHolder.transform.position + Vector3.forward * 100;

        if(SpawnTimer > 0 && (PlayerHolder.P1Active || PlayerHolder.P2Active) && !PlayerHolder.TempOpenAir && !PlayerHolder.WaitForPosition)
        {
            SpawnTimer -= Time.deltaTime;
        }
        else if(SpawnTimer <= 0)
        {
            SpawnEnemy();
            SpawnTimer = Random.Range(5.0f, 7.0f);
        }

        UpdateDifficulty();
    }

    void SpawnEnemy()
    {
        for (int i = 0; i < (int)SpawnCount; i++)
        {
            int DifficultySelect = Random.Range(0, Difficulty + 1);
            int EnemySelect = Random.Range(0, DifficultyLevels[DifficultySelect].EnemyPrefabs.Length);

            Vector3 SpawnPosition = new Vector3(Random.Range(-LimitX, LimitX), Random.Range(-LimitY, LimitY), transform.position.z);
            if(ClearArea(SpawnPosition))
            {
                Instantiate(DifficultyLevels[DifficultySelect].EnemyPrefabs[EnemySelect], SpawnPosition, new Quaternion(transform.rotation.x, transform.rotation.y + 180, transform.rotation.z, transform.rotation.w));
            }
        }
    }
    void UpdateDifficulty()
    {
        float SpawnCountTicks = (float)GM.TotalScore / 50;
        SpawnCountTicks -= Difficulty;
        print((float)GM.TotalScore / 50);
        SpawnCount = Mathf.Lerp(3, 10, SpawnCountTicks);
        if(SpawnCount >= 10 && Difficulty < DifficultyLevels.Length)
        {
            Difficulty += 1;
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

    [System.Serializable]
    public class DifficultyLevel
    {
        public GameObject[] EnemyPrefabs;
    }
}
