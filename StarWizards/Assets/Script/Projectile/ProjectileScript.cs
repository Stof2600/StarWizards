using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public float MoveSpeed = 50f;

    public bool PlayerProjectile;


    private void Start()
    {
        StartCoroutine(DespawnTime());
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * MoveSpeed * Time.deltaTime; 
    }

    private void OnTriggerEnter(Collider other)
    {
        HealthScript HS = other.GetComponentInParent<HealthScript>();
        PlayerControl PC = other.GetComponentInParent<PlayerControl>();

        if (HS != null && (PC == null || (PC != null && !PlayerProjectile)))
        {
            HS.TakeDamage(1);
        }
    }

    IEnumerator DespawnTime()
    {
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }
}
