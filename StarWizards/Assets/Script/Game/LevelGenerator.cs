using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public float StepDis = 200;
    public GameObject StartRoomPrefab;

    public GameObject[] SegmentPrefabs;
    public int MaxSpawns = 0;

    public List<Transform> RoomList = new List<Transform>();
    public Transform PlayerHolder;

    // Start is called before the first frame update
    void Start()
    {
        Transform New = Instantiate(StartRoomPrefab, transform.position, transform.rotation).transform;
        RoomList.Add(New);

        PlayerHolder = FindObjectOfType<FlightControl>().transform;
    }

    void FixedUpdate()
    {
        if(RoomList.Count < MaxSpawns)
        {
            transform.position += transform.forward * StepDis;
            Transform New = Instantiate(SegmentPrefabs[0], transform.position, transform.rotation).transform;
            RoomList.Add(New);
        }

        for (int i = 0; i < RoomList.Count; i++)
        {
            Transform t = RoomList[i];

            if(t.position.z < PlayerHolder.position.z && Vector3.Distance(PlayerHolder.position, t.position) > StepDis)
            {
                RoomList.Remove(t);
                Destroy(t.gameObject);
            }
        } 
    }
}
