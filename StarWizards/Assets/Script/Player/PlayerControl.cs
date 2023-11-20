using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public int playerID;
    public bool MoveActive;

    public float MoveSpeed = 10f, RotateSpeed = 50f;
    public float AnimRot = 20;
    public Transform Model;
    public GameObject PlayerCam;

    Vector2 PlayerInput;

    public GameObject ProjectilePrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ReadInput();
        if(MoveActive)
        {
            Movement();
            ModelAnim();

            PlayerCam.SetActive(true);
        }
        else
        {
            PlayerCam.SetActive(false);
        }

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
        transform.position += transform.forward * MoveSpeed * Time.deltaTime;

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
            GetComponent<HealthScript>().TakeDamage(1);
        }

        if (other.GetComponentInParent<HealthScript>())
        {
            other.GetComponentInParent<HealthScript>().TakeDamage(1);
        }
    }
}
