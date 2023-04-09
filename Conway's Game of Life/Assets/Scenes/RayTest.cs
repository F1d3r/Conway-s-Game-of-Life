using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Ray ray1 = new Ray(transform.position, -transform.forward);
        Ray ray2 = new Ray(transform.position, transform.forward);
        Ray ray3 = new Ray(transform.position, -transform.up);
        Ray ray4 = new Ray(transform.position, transform.up);
        Ray ray5 = new Ray(transform.position, -transform.right);
        Ray ray6 = new Ray(transform.position, transform.right);


        RaycastHit hit;

        if(Physics.Raycast(ray1, out hit))
        {
            Debug.Log(hit.collider.gameObject.name);
        }
        if (Physics.Raycast(ray2, out hit))
        {
            Debug.Log(hit.collider.gameObject.name);
        }
        if (Physics.Raycast(ray3, out hit))
        {
            Debug.Log(hit.collider.gameObject.name);
        }
        if (Physics.Raycast(ray4, out hit))
        {
            Debug.Log(hit.collider.gameObject.name);
        }
        if (Physics.Raycast(ray5, out hit))
        {
            Debug.Log(hit.collider.gameObject.name);
        }
        if (Physics.Raycast(ray6, out hit))
        {
            Debug.Log(hit.collider.gameObject.name);
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
