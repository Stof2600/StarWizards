using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float WaitTime = 1;
    public float DespawnTime = 5;
    public int Damage = 4;
    public float Radius;

    private void Start()
    {
        StartCoroutine(WaitTimer()); 
        StartCoroutine(DespawnTimer());
    }

    void ExplosionCheck()
    {
        Collider[] col = Physics.OverlapSphere(transform.position, Radius);
        foreach(Collider c in col)
        {
            EnemyControl EC = c.GetComponentInParent<EnemyControl>();
            PlayerControl PC = c.GetComponentInParent<PlayerControl>();
            if(EC)
            {
                EC.TakeDamage(Damage);
            }
            if(PC)
            {
                PC.TakeDamage(Damage);
            }
        }
    }

    IEnumerator WaitTimer()
    {
        yield return new WaitForSeconds(WaitTime);
        ExplosionCheck();
    }
    IEnumerator DespawnTimer()
    {
        yield return new WaitForSeconds(DespawnTime);
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, Radius);
    }
}
