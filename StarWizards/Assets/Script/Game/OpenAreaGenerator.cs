using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenAreaGenerator : MonoBehaviour
{
    //do stuff
    public GameObject TargetPrefab;

    public int AmountToSpawn;

    public int AmountAlive;
    public bool OpenAirDone;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < AmountToSpawn; i++)
        {
            EnemyControl NewEnemy = Instantiate(TargetPrefab, transform.position, transform.rotation, transform).GetComponent<EnemyControl>();
            NewEnemy.OpenAir = true;
            NewEnemy.MoveSpeed = 20;
        }
    }

    // Update is called once per frame
    void Update()
    {
        AmountAlive = transform.childCount;

        if(AmountAlive <= 0)
        {
            OpenAirDone = true;
        }
    }
}
