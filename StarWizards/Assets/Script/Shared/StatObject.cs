using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatObject : MonoBehaviour
{
    public int Health, MaxHealth;
    public int ScoreOnDeath;

    public float MoveSpeed;

    private void Start()
    {
        Health = MaxHealth;
    }

    public void MoveForward()
    {
        transform.position += transform.forward * MoveSpeed * Time.deltaTime;
    }

    public void TakeDamage(int Amount)
    {
        Health -= Amount;

        if (Health <= 0)
        {
            if (GetComponent<PlayerControl>())
            {
                FindObjectOfType<GameManager>().ResetPlayer(GetComponent<PlayerControl>().playerID);
            }
            if(ScoreOnDeath > 0)
            {
                FindObjectOfType<GameManager>().AddScore(ScoreOnDeath);
            }

            Destroy(gameObject);
        }
    }
}
