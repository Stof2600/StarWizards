using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public float MoveSpeed = 0.5f;

    public bool PlayerProjectile;


    private void Start()
    {
        StartCoroutine(DespawnTime());
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * MoveSpeed; 
    }

    IEnumerator DespawnTime()
    {
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }
}
