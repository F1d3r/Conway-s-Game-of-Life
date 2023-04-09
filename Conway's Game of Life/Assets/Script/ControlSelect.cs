using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ControlSelect : MonoBehaviour
{
    public GameManager manager;

    private float xRange;
    private float yRange;
    private float zRange;

    // Start is called before the first frame update
    void Start()
    {
        // Adjust the initial position of the selecter according to the size of grid.
        transform.position = new Vector3(-(manager.sizeOfGrid/2.0f - 0.5f), -(manager.sizeOfGrid / 2.0f - 0.5f), -(manager.sizeOfGrid / 2.0f - 0.5f));

        xRange = manager.GetOffestX();
        yRange = manager.GetOffestY();
        zRange = manager.GetOffestZ();

    }

    // Update is called once per frame
    void Update()
    {
        //gameObject.GetComponentInChildren<Renderer>().enabled = manager.selecterActive;
        gameObject.SetActive(manager.selecterActive);

        if (manager.selecterActive)
        {
            Move();
            Select();
        }
    }


    void Move()
    {
        Vector3 previousLocation = transform.position;

        if (Input.GetKeyDown(KeyCode.W))
        {
            transform.Translate(Vector3.forward);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            transform.Translate(Vector3.back);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.Translate(Vector3.left);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            transform.Translate(Vector3.right);
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            transform.Translate(Vector3.down);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            transform.Translate(Vector3.up);
        }


        if(transform.position.x > xRange || transform.position.x < -xRange)
        {
            transform.position = previousLocation;
        }

        if (transform.position.y > yRange || transform.position.y < -yRange)
        {
            transform.position = previousLocation;
        }

        if (transform.position.z > zRange || transform.position.z < -zRange)
        {
            transform.position = previousLocation;
        }


    }


    void Select()
    {
        if (Input.GetKeyDown(KeyCode.K)){
            manager.SelectCube(TransformToIndex());
        }
    }


    int[] TransformToIndex()
    {
        int[] index = new int[3];

        index[0] = (int)(manager.GetOffestX() + transform.position.x);
        index[1] = (int)(manager.GetOffestX() + transform.position.y);
        index[2] = (int)(manager.GetOffestX() + transform.position.z);

        return index;
    }


}
