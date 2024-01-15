using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : StatObject
{
    public bool PlayerProjectile;
    public float DespawnTimer;
    Vector3 OldPos;

    private void Start()
    {
        OldPos = transform.position;
        StartCoroutine(DespawnTime());
    }

    // Update is called once per frame
    void Update()
    {
        MoveForward();
        CollisionDetection();
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerControl PC = other.GetComponentInParent<PlayerControl>();
        EnemyControl EC = other.GetComponentInParent<EnemyControl>();

        if(!PlayerProjectile && PC)
        {
            if(DeathEffect)
            {
                Instantiate(DeathEffect, transform.position, transform.rotation);
                Destroy(gameObject);
                return;
            }

            PC.TakeDamage(1);
            Destroy(gameObject);
        }
        else if(PlayerProjectile && EC)
        {
            if (DeathEffect)
            {
                Instantiate(DeathEffect, transform.position, transform.rotation);
                Destroy(gameObject);
                return;
            }

            EC.TakeDamage(1);
            Destroy(gameObject);
        }
    }

    void CollisionDetection()
    {
        if(Physics.Linecast(OldPos, transform.position, out RaycastHit Hit) && Hit.transform.CompareTag("Level"))
        {
            if (DeathEffect)
            {
                Instantiate(DeathEffect, transform.position, transform.rotation);
            }

            Destroy(gameObject);
        }

        OldPos = transform.position;
    }

    IEnumerator DespawnTime()
    {
        yield return new WaitForSeconds(DespawnTimer);
        Destroy(gameObject);
    }
}
