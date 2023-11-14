using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FlightControl : MonoBehaviour
{
    public Transform P1, P2;
    public Transform P1Model, P2Model;

    public float limitX, limitY;

    Vector2 P1Input, P2Input;
    
    public bool P1Active, P2Active;

    public float PlayerSpeed = 0.1f, PlayerRotMulti = 30;
    public float LevelSpeed = 0.1f;
   

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ReadInput();
        Movement();
        ModelAnimation();
    }

    void ReadInput()
    {
        P1Input.x = Input.GetAxis("P1H");
        P1Input.y = Input.GetAxis("P1V");

        P2Input.x = Input.GetAxis("P2H");
        P2Input.y = Input.GetAxis("P2V");
    }
    
    void Movement()
    {
        P1.localPosition += new Vector3(P1Input.x, P1Input.y, 0) * PlayerSpeed;
        P2.localPosition += new Vector3(P2Input.x, P2Input.y, 0) * PlayerSpeed;

        transform.position += new Vector3(0, 0, LevelSpeed);


        if(P1.localPosition.y >= limitY)
        {
            P1.localPosition = new Vector3(P1.localPosition.x, limitY, 0);
        }
        else if(P1.localPosition.y <= -limitY)
        {
            P1.localPosition = new Vector3(P1.localPosition.x, -limitY, 0);
        }
        if (P1.localPosition.x >= limitX)
        {
            P1.localPosition = new Vector3(limitX, P1.localPosition.y, 0);
        }
        else if (P1.localPosition.x <= -limitX)
        {
            P1.localPosition = new Vector3(-limitX, P1.localPosition.y, 0);
        }

        if (P2.localPosition.y >= limitY)
        {
            P2.localPosition = new Vector3(P2.localPosition.x, limitY, 0);
        }
        else if (P2.localPosition.y <= -limitY)
        {
            P2.localPosition = new Vector3(P2.localPosition.x, -limitY, 0);
        }
        if (P2.localPosition.x >= limitX)
        {
            P2.localPosition = new Vector3(limitX, P2.localPosition.y, 0);
        }
        else if (P2.localPosition.x <= -limitX)
        {
            P2.localPosition = new Vector3(-limitX, P2.localPosition.y, 0);
        }
    }

    void ModelAnimation()
    {
        P1Model.localEulerAngles = new Vector3(-P1Input.y * PlayerRotMulti, P1Input.x * PlayerRotMulti, -P1Input.x * PlayerRotMulti);
        P2Model.localEulerAngles = new Vector3(-P2Input.y * PlayerRotMulti, P2Input.x * PlayerRotMulti, -P2Input.x * PlayerRotMulti);
    }

}
