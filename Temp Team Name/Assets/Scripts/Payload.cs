using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class Payload : MonoBehaviour, IDamage
{
    [SerializeField] int HP;
    [SerializeField] private Transform spawn;
    [SerializeField] float speed;
    [SerializeField] float waypointStartDuration;
    [SerializeField] float waypointStopDuration;
    [SerializeField] int rotationSpeed;
    [SerializeField] private Image hpBar;
    [SerializeField] private Color color;
    [SerializeField] Renderer model;



    public List<GameObject> waypoints;
    int index = 0;
    private int HPOrig;
    bool isMoving = true;
    

    void Start()
    {
        color = model.material.color;
        spawnCart();
        MoveToNextWaypoint();
        HPOrig = HP;
    }

    void Update()
    {
        if (isMoving)
        {
            Vector3 destination = waypoints[index].transform.position;
            Vector3 direction = (destination - transform.position).normalized;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }

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

    public void spawnCart()
    {
        gameObject.transform.position = spawn.position;
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;

        // Flash red
        StartCoroutine(flashMat());
        if (HP <= 0)
        {
            
            Destroy(gameObject);
        }
        // Lower HP on HP bar
        updateUI();
    }

    void updateUI()
    {
        hpBar.fillAmount = (float)HP / HPOrig;
    }

    IEnumerator flashMat()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = color;
    }
}




