using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public bool movable;
    public float moveSpeed;
    public float rotateSpeed;
    public float validZoneSize;
    public GameManager manager;

    private float horizontalRotation = 0.0f;
    private float verticalRotation = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        movable = true;
    }


    // Update is called once per frame.
    private void Update()
    {
        // If the selecter is not active.
        if (!manager.selecterActive)
        {
            // Detect movement and rotation.
            Move();
            Rotate();
            FlowUpDown();
        }


    }


    // Called per frame.
    private void Move()
    {
        // Move the camera when the camera is in the valid zone.
        if (Math.Abs(transform.position.x) <= validZoneSize 
            && Math.Abs(transform.position.y) <= validZoneSize 
            && Math.Abs(transform.position.z) <= validZoneSize)
        {
            // Get the input value from two dimensions.
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            // Calculate the movement distance from given deltaTime.
            Vector3 move = new Vector3(horizontal, 0, vertical) * moveSpeed * Time.deltaTime;
            transform.Translate(move);
        }
        // When it exceed the boundery
        else
        {
            // Adjust the axis according to the exceeding one.
            if(transform.position.x > validZoneSize)
            {
                transform.position = new Vector3(validZoneSize, transform.position.y, transform.position.z);
            }else if (transform.position.x < -validZoneSize)
            {
                transform.position = new Vector3(-validZoneSize, transform.position.y, transform.position.z);
            }

            if (transform.position.y > validZoneSize)
            {
                transform.position = new Vector3(transform.position.x, validZoneSize, transform.position.z);
            }
            else if (transform.position.y < -validZoneSize)
            {
                transform.position = new Vector3(transform.position.x, -validZoneSize, transform.position.z);
            }

            if (transform.position.z > validZoneSize)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, validZoneSize);
            }
            else if (transform.position.z < -validZoneSize)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, -validZoneSize);
            }
        }
    }


    // Called per frame.
    private void Rotate()
    {
        // Only rotate when the left button of mouse clicked.
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime * 100;
            float mouseY = -Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime * 100;

            horizontalRotation += mouseX;
            verticalRotation += mouseY;

            // Rotate the camera by a quaternion.
            transform.localRotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0);

        }
    }

    private void FlowUpDown()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            // Calculate the movement distance from given deltaTime.
            Vector3 move = Vector3.up * moveSpeed * Time.deltaTime;
            transform.Translate(move);
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            // Calculate the movement distance from given deltaTime.
            Vector3 move = Vector3.down * moveSpeed * Time.deltaTime;
            transform.Translate(move);
        }
    }



}
