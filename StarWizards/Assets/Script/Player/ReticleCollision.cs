using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReticleCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        print(collision.collider.name);
    }
}
