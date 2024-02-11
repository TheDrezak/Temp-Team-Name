using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{
    [SerializeField] int sensitivity;
    [SerializeField] int lockVertMin;
    [SerializeField] int lockVertMax;
    [SerializeField] bool invertY;

    float rotX;

    void Start()
    {
        // Lock & hide cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
         // Get input
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity;
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity;

        // Invert look
        if (invertY) 
        { rotX += mouseY; }    
        else 
        { rotX -= mouseY; }

        // Clamp the rot on the x-axis
        rotX = Mathf.Clamp(rotX, lockVertMin, lockVertMax);

        // Rotate the cam on the x-axis
        transform.localRotation = Quaternion.Euler(rotX, 0, 0);

        // Rotate the player on the y-axis
        transform.parent.Rotate(Vector3.up * mouseX);

    }
}
