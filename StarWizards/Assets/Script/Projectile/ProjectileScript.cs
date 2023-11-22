using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : StatObject
{
    public bool PlayerProjectile;


    private void Start()
    {
        StartCoroutine(DespawnTime());
    }

    // Update is called once per frame
    void Update()
    {
        MoveForward();
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerControl PC = other.GetComponentInParent<PlayerControl>();
        EnemyControl EC = other.GetComponentInParent<EnemyControl>();

        if(!PlayerProjectile && PC)
        {
            PC.TakeDamage(1);
        }
        else if(PlayerProjectile && EC)
        {
            EC.TakeDamage(1);
        }
    }

    IEnumerator DespawnTime()
    {
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }
}
