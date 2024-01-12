using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenAreaGenerator : MonoBehaviour
{
    //do stuff
    public GameObject TargetPrefab;

    public Transform[] SpawnPoints;
    public int SpawnCountOveride;


    public int AmountAlive;
    public bool OpenAirDone;

    public float EndTPDis = 100; //how much should the FlightController jump forward at the end (+ position of self)
    bool DidEndCheck;

    // Start is called before the first frame update
    void Start()
    {
        DidEndCheck = false;

        int SpawnCount = SpawnPoints.Length;
        if(SpawnCountOveride > 0 && SpawnCountOveride <= SpawnPoints.Length)
        {
            SpawnCount = SpawnCountOveride;
        }

        for (int i = 0; i < SpawnCount; i++)
        {
            GameObject NewEnemy = Instantiate(TargetPrefab, SpawnPoints[i].position, Quaternion.Euler(0, 0, 0), transform);
            EnemyControl EC = NewEnemy.GetComponent<EnemyControl>();
            EC.OpenAir = true;
            EC.IsTarget = true;
            EC.MoveSpeed = 20;
        }
    }

    // Update is called once per frame
    void Update()
    {
        AmountAlive = transform.childCount;

        if(AmountAlive <= 0)
        {
            OpenAirDone = true;

            if(!DidEndCheck)
            {
                foreach (Collider c in transform.parent.GetComponentsInChildren<Collider>())
                {
                    c.enabled = false;
                }
                foreach (EnemyControl EC in FindObjectsOfType<EnemyControl>())
                {
                    if (EC.OpenAir)
                    {
                        Destroy(EC.gameObject);
                    }
                }
                DidEndCheck = true;
            }

        }
    }

    public void ChangeEnemies()
    {
        foreach(EnemyControl EC in FindObjectsOfType<EnemyControl>())
        {
            if (!EC.OpenAir)
            {
                EC.OpenAir = true;
                EC.MoveSpeed = 20;
            }
        }
    }
}
