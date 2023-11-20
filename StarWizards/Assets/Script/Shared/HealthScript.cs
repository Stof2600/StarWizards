using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthScript : MonoBehaviour
{
    public int Health, MaxHealth;
    

    // Start is called before the first frame update
    void Start()
    {
        Health = MaxHealth;
    }

    public void TakeDamage(int Amount)
    {
        Health -= Amount;

        if (Health <= 0)
        {
            if(GetComponent<PlayerControl>())
            {
                FindObjectOfType<GameManager>().ResetPlayer(GetComponent<PlayerControl>().playerID);
            }

            Destroy(gameObject);
        }
    }
}
