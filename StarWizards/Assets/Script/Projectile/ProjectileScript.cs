using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : StatObject
{
    public bool PlayerProjectile;
    public float DespawnTimer;

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
            Destroy(gameObject);
        }
        else if(PlayerProjectile && EC)
        {
            EC.TakeDamage(1);
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }

    IEnumerator DespawnTime()
    {
        yield return new WaitForSeconds(DespawnTimer);
        Destroy(gameObject);
    }
}
