using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuModel : MonoBehaviour
{
    public GameObject[] Models;
    public float RotateSpeed;

    public Transform Cam;

    // Start is called before the first frame update
    void Start()
    {
        int R = Random.Range(0, Models.Length);
        for (int i = 0; i < Models.Length; i++)
        {
            if(i == R)
            {
                Models[i].SetActive(true);
            }
            else
            {
                Models[i].SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, RotateSpeed * Time.deltaTime, 0);
        Cam.Rotate(-RotateSpeed * Time.deltaTime / 6, 0, 0);
    }
}
