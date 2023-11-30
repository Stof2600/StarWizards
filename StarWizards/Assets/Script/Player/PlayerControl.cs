using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : StatObject
{
    public int playerID;
    public bool MoveActive;

    public float RotateSpeed = 50f;
    public float AnimRot = 20;
    public Transform Model;
    public GameObject PlayerCam;

    public bool ReticleActive;
    public float ReticleSpacing = 20;
    public Transform AimPoint;
    public Transform LongReticle, ShortReticle;
    public SpriteRenderer LRSprite, SRSprite;
    Vector3 LongPos;

    Vector2 PlayerInput;

    public GameObject ProjectilePrefab;

    void Update()
    {
        ReadInput();
        GetComponent<Rigidbody>().velocity = Vector3.zero;

        if (MoveActive)
        {
            Movement();
            ModelAnim();

            PlayerCam.SetActive(true);
        }
        else
        {
            PlayerCam.SetActive(false);
        }

        if(ReticleActive)
        {
            ShortReticle.gameObject.SetActive(true);
            LongReticle.gameObject.SetActive(true);

            ReticleCollision();
        }
        else
        {
            ShortReticle.gameObject.SetActive(false);
            LongReticle.gameObject.SetActive(false);
        }

        WallInFrontCheck();
        RunHitAnim();
    }

    void ReadInput()
    {
        if(playerID == 0)
        {
            PlayerInput.x = Input.GetAxis("P1H");
            PlayerInput.y = Input.GetAxis("P1V");

            if (Input.GetButtonDown("P1Fire"))
            {
                Fire();
            }
        }
        else
        {
            PlayerInput.x = Input.GetAxis("P2H");
            PlayerInput.y = Input.GetAxis("P2V");

            if (Input.GetButtonDown("P2Fire"))
            {
                Fire();
            }
        }
    }

    void Movement()
    {
        MoveForward();
        transform.Rotate(PlayerInput.y * RotateSpeed * Time.deltaTime, PlayerInput.x * RotateSpeed * Time.deltaTime, 0);
    }

    void Fire()
    {
        Instantiate(ProjectilePrefab, transform.position, transform.rotation);
    }

    void ModelAnim()
    {
        Model.localEulerAngles = new Vector3(PlayerInput.y * AnimRot, PlayerInput.x * AnimRot, -PlayerInput.x * AnimRot);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.GetComponentInParent<PlayerControl>() && !other.GetComponentInParent<ProjectileScript>())
        {
            GetComponent<PlayerControl>().TakeDamage(1);
        }

        if (other.GetComponentInParent<EnemyControl>())
        {
            other.GetComponentInParent<EnemyControl>().TakeDamage(1);
        }
    }

    void WallInFrontCheck()
    {
        if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 0.7f) && hit.transform.CompareTag("Level"))
        {
            TakeDamage(Health);
        }
    }

    void ReticleCollision()
    {
        if(!AimPoint)
        {
            AimPoint = transform;
        }

        Debug.DrawRay(transform.position, transform.forward * 100, Color.blue);
        if(Physics.Raycast(transform.position, transform.forward, out RaycastHit Hit, 100f) && Hit.transform.GetComponentInParent<EnemyControl>())
        {
            Vector3 ReticleDrawDir = (AimPoint.position - Hit.point);
            ReticleDrawDir *= -1;
            Debug.DrawLine(AimPoint.position, AimPoint.position + ReticleDrawDir, Color.red);

            LongReticle.position = AimPoint.position + ReticleDrawDir;
            ShortReticle.position = AimPoint.position + ReticleDrawDir / 2;

            LRSprite.color = Color.red;
            SRSprite.color = Color.red;
        }
        else 
        {
            Vector3 ReticleDrawDir = (AimPoint.position - (transform.position + new Vector3(0, 0, ReticleSpacing * 2)));
            ReticleDrawDir *= -1;
            Debug.DrawLine(AimPoint.position, AimPoint.position + ReticleDrawDir, Color.green);

            LongReticle.position = AimPoint.position + ReticleDrawDir;
            ShortReticle.position = AimPoint.position + ReticleDrawDir / 2;

            LRSprite.color = Color.green;
            SRSprite.color = Color.green;
        }
    }
}
