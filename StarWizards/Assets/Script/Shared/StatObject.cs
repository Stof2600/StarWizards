using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatObject : MonoBehaviour
{
    public int Health, MaxHealth;
    public int ScoreOnDeath;

    public float MoveSpeed;

    public Material HitMat;
    public Material DefaultMat;
    public GameObject DeathEffect;

    float HitAnimTime;

    public void Setup()
    {
        Health = MaxHealth;
        DefaultMat = GetComponentInChildren<MeshRenderer>().material;
    }

    public void MoveForward()
    {
        transform.position += transform.forward * MoveSpeed * Time.deltaTime;
    }

    public void TakeDamage(int Amount)
    {
        if(HitAnimTime <= 0)
        {
            Health -= Amount;

            HitAnimTime = 0.6f;
        }

        if (Health <= 0)
        {
            if (GetComponent<PlayerControl>())
            {
                FindObjectOfType<GameManager>().ResetPlayer(GetComponent<PlayerControl>().playerID);
                if (GetComponentInChildren<Camera>())
                {
                    GetComponentInChildren<Camera>().transform.SetParent(null);
                }
            }
            if(ScoreOnDeath > 0)
            {
                FindObjectOfType<GameManager>().AddScore(ScoreOnDeath);
            }

            if(DeathEffect)
            {
                Transform NewEffect = Instantiate(DeathEffect, transform.position, transform.rotation).transform;
                NewEffect.localScale = transform.localScale;

                TextMesh DeathText = NewEffect.GetComponentInChildren<TextMesh>();
                if(DeathText && ScoreOnDeath > 0)
                {
                    DeathText.text = "+" + ScoreOnDeath.ToString("00");
                    Vector3 LookDir = (FindFirstObjectByType<PlayerControl>().transform.position - transform.position).normalized;
                    DeathText.transform.LookAt(LookDir);
                }
            }

            Destroy(gameObject);
        }
    }

    public void RunHitAnim()
    {
        if (HitAnimTime > 0)
        {
            HitAnimTime -= Time.deltaTime;

            if((HitAnimTime * 10) % 2 < 1)
            {
                foreach (MeshRenderer MR in GetComponentsInChildren<MeshRenderer>())
                {
                    MR.material = HitMat;
                }
            }
            else if((HitAnimTime * 10) % 2 >= 1)
            {
                foreach (MeshRenderer MR in GetComponentsInChildren<MeshRenderer>())
                {
                    MR.material = DefaultMat;
                }
            }
        }
        else 
        {
            if(!DefaultMat)
            {
                DefaultMat = GetComponentInChildren<MeshRenderer>().material;
            }

            foreach (MeshRenderer MR in GetComponentsInChildren<MeshRenderer>())
            {
                MR.material = DefaultMat;
            }
        }
    }
}
