using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPlayer : StatObject
{
    public int PlayerID;

    Vector2 PlayerInput;
    Rigidbody RB;

    public bool MoveActive;

    public GameObject ProjectilePrefab;

    public Camera PlayerCam;
    Vector3 DefaultCamLocal;

    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody>();
        DefaultCamLocal = PlayerCam.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        ReadInput();

        Movement();
    }

    public void ReadInput()
    {
        if(PlayerID == 0)
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
        RB.velocity = transform.forward * PlayerInput.y * MoveSpeed;
        transform.Rotate(0, PlayerInput.x * Time.deltaTime * MoveSpeed * 25, 0);

        CameraCollision();
    }

    void Fire()
    {
        Instantiate(ProjectilePrefab, transform.position + transform.forward * 0.5f, transform.rotation);
    }

   void CameraCollision()
    {
        bool WallCheck = Physics.Linecast(transform.position, PlayerCam.transform.position, out RaycastHit CollisionInfo);

        if (WallCheck && CollisionInfo.transform.CompareTag("Level"))
        {
            Debug.DrawLine(transform.position, CollisionInfo.point, Color.green);

            PlayerCam.transform.position = CollisionInfo.point;
        }
        else
        {
            PlayerCam.transform.localPosition = Vector3.MoveTowards(PlayerCam.transform.localPosition, DefaultCamLocal, Time.deltaTime);
        }
    }
}
