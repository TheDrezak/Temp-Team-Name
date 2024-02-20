using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOnWaypoints : MonoBehaviour
{
    public List<GameObject> waypoints;
    public float speed = 2;
    public float waypointStopDuration = 4f;
    int index = 0;
    bool isMoving = true;

    void Start()
    {
        MoveToNextWaypoint();
    }

    void Update()
    {
        if (isMoving)
        {
            Vector3 destination = waypoints[index].transform.position;
            Vector3 newPos = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
            transform.position = newPos;

            float distance = Vector3.Distance(transform.position, destination);
            if (distance <= 0.05)
            {
                StartCoroutine(StopAtWaypoint());
            }
        }
    }

    IEnumerator StopAtWaypoint()
    {
        isMoving = false;
        yield return new WaitForSeconds(waypointStopDuration);
        isMoving = true;
        MoveToNextWaypoint();
    }

    void MoveToNextWaypoint()
    {
        index++;
        if (index >= waypoints.Count)
        {
            index = 0;
        }
    }
}

    


   