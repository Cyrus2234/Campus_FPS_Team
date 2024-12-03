using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraContols : MonoBehaviour
{
    [SerializeField] int sensitivity;
    [SerializeField] int lockVertexMinimum;
    [SerializeField] int lockVertexMaximum;
    [SerializeField] bool invertYAxis;

    float rotationX;
    float rotationY;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //get input
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        //Tie the mouse to input of rotation X
        if (!invertYAxis)
        {
            rotationX -= mouseY;
        }
        else
        {
            rotationX += mouseY;
        }

        //clamp camera x rotation to prevent over/underflow
        rotationX = Mathf.Clamp(rotationX, lockVertexMinimum, lockVertexMaximum);


        //rotate camera on X axis
        transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

        //rotate player on Y axis

        transform.parent.Rotate(Vector3.up * mouseX);

    }

    void Movement()
    { 
        
    }

}
