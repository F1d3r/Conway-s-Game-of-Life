using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudManager : MonoBehaviour
{
    public GameObject[] cloudPrefabs;
    public GameObject parent;

    private int cloudAmount = 28;
    private float cloudUpperBound = 600;
    private float cloudButtomBound = 400;



    void Awake()
    {
        // Setup clouds in scene.
        for(int i = 0; i < cloudAmount; i++)
        {
            int index = Random.Range(0, cloudPrefabs.Length);
            Vector3 cloudDirection = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
            float distance = Random.Range(cloudButtomBound, cloudUpperBound);
            Vector3 cloudPos = cloudDirection * distance;
            GameObject cloud = Instantiate(cloudPrefabs[index], cloudPos, Quaternion.identity);
            float cloudScale = Random.Range(2.0f, 5.0f);
            cloud.transform.localScale = new Vector3(cloudScale, cloudScale, cloudScale);

            // Assign parent object to the genereated clouds.
            cloud.transform.parent = parent.transform;
        }
    }



    void Update()
    {
        
    }
}
