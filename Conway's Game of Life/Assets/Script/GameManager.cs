using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.SocialPlatforms;
using System.IO;

public class GameManager : MonoBehaviour
{
    public GameObject cubePrefab;
    public float nextGenDelay;
    public bool selecterActive;
    public GameObject grid;
    public Camera mainCamera;
    public int sizeOfGrid;
    public GameObject selecter;

    private float offsetX;
    private float offsetY;
    private float offsetZ;
    private float cubeScale = 0.9f;
    private GameObject[,,] matrix;
    private Vector3 tempCamPos;
    private Vector3 tempCamDir;
    private bool[,,] alives;
    private bool gameStarted;
    private int storedPatterns;

    private void Awake()
    {
        // Offset values to centralize the grid.
        offsetX = sizeOfGrid / 2.0f - 0.5f;
        offsetY = sizeOfGrid / 2.0f - 0.5f;
        offsetZ = sizeOfGrid / 2.0f - 0.5f;
        gameStarted = false;
        nextGenDelay = 1.0f;
        // Array used to records the alive and dead cubes.
        alives = new bool[sizeOfGrid, sizeOfGrid, sizeOfGrid];
        selecterActive = false;
        selecter.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeGrid();
    }

    // Update is called once per frame
    void Update()
    {
        // If the game is not started.
        if(!gameStarted)
        {
            ToggleSelecter();
            if(Input.GetKeyDown(KeyCode.L))
            {
                gameStarted = true;
                StartGame();
                selecterActive = false;
                selecter.SetActive(false);
            }
        }
        // If the game has started.
        else
        {
            if(Input.GetKeyDown(KeyCode.R))
            {
                gameStarted = false;
                CancelInvoke();
                if(selecterActive)
                {
                    selecter.SetActive(true);
                }
                else
                {
                    selecter.SetActive(false);
                }
            }
        }


        if (selecterActive)
        {
            MoveSelect();
            if (Input.GetKeyDown(KeyCode.K))
            {
                SelectCube(TransformToIndex());
            }
        }

    }


    void InitializeGrid()
    {
        matrix = new GameObject[sizeOfGrid, sizeOfGrid, sizeOfGrid];

        // Instantiate cubes according to the size of grid.
        for(int i = 0; i < matrix.GetLength(0); i++)
        {
            for(int j = 0; j < matrix.GetLength(1); j++)
            {
                for(int k = 0; k < matrix.GetLength(2); k++)
                {
                    GameObject cube = Instantiate(cubePrefab, new Vector3(i - offsetX, j - offsetY, k - offsetZ), Quaternion.identity);
                    cube.transform.localScale = new Vector3(cubeScale, cubeScale, cubeScale);
                    // Initially mute the renderer of the cube.
                    cube.GetComponent<Renderer>().enabled = false;
                    cube.GetComponent<Collider>().enabled = false;
                    // Rename the cube.
                    cube.gameObject.name = "Cube[" + i + ", " + j + ", " + k + "]";
                    matrix[i, j, k] = cube;
                    alives[i, j, k] = false;


                    // Add the new cube into the grid object.
                    matrix[i, j, k].transform.parent = grid.transform;
                }
            }
        }

    }


    void StartGame()
    {
        InvokeRepeating("NextGeneration", nextGenDelay, nextGenDelay);
    }


    void NextGeneration()
    {

        // Firstly calculate all states of cubes in the next generation, records the result in a new array alives.
        // Iterate every cube in the grid.
        for(int i = 0; i < matrix.GetLength(0); i++)
        {
            for(int j = 0; j < matrix.GetLength(1); j++)
            {
                for(int k = 0; k < matrix.GetLength(2); k++)
                {
                    // Calculate the neighbors of the cube.
                    int neighbors = CurrentNeighbors(i, j, k);

                    // Get the renderer of current cube.
                    Renderer renderer = matrix[i, j, k].GetComponent<Renderer>();

                    // If the current cube is alive.
                    if(renderer.enabled)
                    {

                        // Under population.
                        if (neighbors <= 1)
                        {
                            alives[i, j, k] = false;
                        }
                        // Still
                        else if (neighbors == 2)
                        {
                            alives[i, j, k] = true;
                        }
                        // Overpupulation.
                        //neighbours >= 3
                        else
                        {
                            alives[i, j, k] = false;
                        }

                    }
                    // If the current cube is dead.
                    else
                    {
                        // If there are exactlly 3 neighbours.
                        if (neighbors == 3)
                        {
                            // Born a new cube.
                            alives[i, j, k] = true;
                        }
                    }

                }
            }
        }


        // Update the cube's states according to the alives array.
        for(int i = 0; i < alives.GetLength(0); i++)
        {
            for(int j = 0; j < alives.GetLength(1); j++)
            {
                for(int k = 0; k < alives.GetLength(2); k++)
                {
                    Renderer renderer = matrix[i, j, k].GetComponent<Renderer>();
                    Collider collider = matrix[i, j, k].GetComponent<Collider>();
                    renderer.enabled = alives[i, j, k];
                    collider.enabled = alives[i, j, k];
                }
            }
        }


    }


    int CurrentNeighbors(int x, int y, int z)
    {
        int neighbors = 0;




        // Version 1. Iterate all cubes and its corresponding neighbors. Time Complexity: O(14*n^3) = O(n^3).
        /*for (int i = 0; i < 3; i++)
        {
            switch (i)
            {
                case 0:
                    // X
                    // Only works when the cube is not at the boundry.
                    if (x == 0)
                    {
                        if (matrix[x + 1, y, z].GetComponent<Renderer>().enabled)
                        {
                            neighbors++;
                        }
                    }
                    else if (x == sizeOfGrid - 1)
                    {
                        if (matrix[x - 1, y, z].GetComponent<Renderer>().enabled)
                        {
                            neighbors++;
                        }
                    }
                    else
                    {
                        if (matrix[x + 1, y, z].GetComponent<Renderer>().enabled)
                        {
                            neighbors++;
                        }
                        if (matrix[x - 1, y, z].GetComponent<Renderer>().enabled)
                        {
                            neighbors++;
                        }
                    }
                    break;
                case 1:
                    // Y
                    // Only works when the cube is not at the boundry.
                    if (y == 0)
                    {
                        if (matrix[x, y + 1, z].GetComponent<Renderer>().enabled)
                        {
                            neighbors++;
                        }
                    }
                    else if (y == sizeOfGrid - 1)
                    {
                        if (matrix[x, y - 1, z].GetComponent<Renderer>().enabled)
                        {
                            neighbors++;
                        }
                    }
                    else
                    {
                        if (matrix[x, y + 1, z].GetComponent<Renderer>().enabled)
                        {
                            neighbors++;
                        }
                        if (matrix[x, y - 1, z].GetComponent<Renderer>().enabled)
                        {
                            neighbors++;
                        }
                    }
                    break;
                case 2:
                    // Z
                    // Only works when the cube is not at the boundry.
                    if (z == 0)
                    {
                        if (matrix[x, y, z + 1].GetComponent<Renderer>().enabled)
                        {
                            neighbors++;
                        }
                    }
                    else if (z == sizeOfGrid - 1)
                    {
                        if (matrix[x, y, z - 1].GetComponent<Renderer>().enabled)
                        {
                            neighbors++;
                        }
                    }
                    else
                    {
                        if (matrix[x, y, z + 1].GetComponent<Renderer>().enabled)
                        {
                            neighbors++;
                        }
                        if (matrix[x, y, z - 1].GetComponent<Renderer>().enabled)
                        {
                            neighbors++;
                        }
                    }
                    break;
                default:
                    break;
            }
        }*/



        // Version2: For every cube, generates 14 rays from it, detected the hits amount. Time Complixt:
        // O(9*n^3) = O(n^3).
        Ray ray1 = new Ray(matrix[x, y, z].transform.position, matrix[x, y, z].transform.forward);
        Ray ray2 = new Ray(matrix[x, y, z].transform.position, -matrix[x, y, z].transform.forward);
        Ray ray3 = new Ray(matrix[x, y, z].transform.position, matrix[x, y, z].transform.up);
        Ray ray4 = new Ray(matrix[x, y, z].transform.position, -matrix[x, y, z].transform.up);
        Ray ray5 = new Ray(matrix[x, y, z].transform.position, matrix[x, y, z].transform.right);
        Ray ray6 = new Ray(matrix[x, y, z].transform.position, -matrix[x, y, z].transform.right);

        RaycastHit hit;
        if (Physics.Raycast(ray1, out hit))
        {
            neighbors++;
            Debug.Log(hit.collider.gameObject.transform.position);
        }
        if (Physics.Raycast(ray2, out hit))
        {
            neighbors++;
            Debug.Log(hit.collider.gameObject.transform.position);
        }
        if (Physics.Raycast(ray3, out hit))
        {
            neighbors++;
            Debug.Log(hit.collider.gameObject.transform.position);
        }
        if (Physics.Raycast(ray4, out hit))
        {
            neighbors++;
            Debug.Log(hit.collider.gameObject.transform.position);
        }
        if (Physics.Raycast(ray5, out hit))
        {
            neighbors++;
            Debug.Log(hit.collider.gameObject.transform.position);
        }
        if (Physics.Raycast(ray6, out hit))
        {
            neighbors++;
            Debug.Log(hit.collider.gameObject.transform.position);
        }

        Debug.Log("Cube at: " + x + ", " + y + ", " + z + "has Neighbors: " + neighbors);


        return neighbors;
    }


    void ToggleSelecter()
    {
        // Mute the selecter.
        if (Input.GetKeyDown(KeyCode.I) && selecterActive)
        {
            selecterActive = false;
            selecter.SetActive(false);
            mainCamera.transform.position = tempCamPos;
            mainCamera.transform.rotation = Quaternion.Euler(tempCamDir);
        }
        // Activate the selecter.
        else if (Input.GetKeyDown(KeyCode.I) && !selecterActive)
        {
            tempCamPos = mainCamera.transform.position;
            tempCamDir= mainCamera.transform.forward;

            selecterActive = true;
            selecter.SetActive(true);

            mainCamera.transform.position = new Vector3(-sizeOfGrid, sizeOfGrid, -sizeOfGrid);
            mainCamera.transform.rotation = Quaternion.Euler(35, 45, 0);
        }
    }


    public float GetOffestX()
    {
        return offsetX;
    }

    public float GetOffestY()
    {
        return offsetY;
    }

    public float GetOffestZ()
    {
        return offsetZ;
    }


    public void SelectCube(int[] index)
    {
        Renderer renderer = matrix[index[0], index[1], index[2]].GetComponent<Renderer>();
        Collider collider = matrix[index[0], index[1], index[2]].GetComponent<Collider>();
        if (renderer.enabled == true)
        {
            renderer.enabled = false;
            collider.enabled = false;
            alives[index[0], index[1], index[2]] = false;
        }
        else
        {
            renderer.enabled = true;
            collider.enabled = true;
            alives[index[0], index[1], index[2]] = true;
        }
    }


    void MoveSelect()
    {
        Vector3 previousLocation = selecter.transform.position;

        if (Input.GetKeyDown(KeyCode.W))
        {
            selecter.transform.Translate(Vector3.forward);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            selecter.transform.Translate(Vector3.back);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            selecter.transform.Translate(Vector3.left);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            selecter.transform.Translate(Vector3.right);
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            selecter.transform.Translate(Vector3.down);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            selecter.transform.Translate(Vector3.up);
        }


        if (selecter.transform.position.x > offsetX || selecter.transform.position.x < -offsetX)
        {
            selecter.transform.position = previousLocation;
        }

        if (selecter.transform.position.y > offsetY || selecter.transform.position.y < -offsetY)
        {
            selecter.transform.position = previousLocation;
        }

        if (selecter.transform.position.z > offsetZ|| selecter.transform.position.z < -offsetZ)
        {
            selecter.transform.position = previousLocation;
        }


    }


    int[] TransformToIndex()
    {
        int[] index = new int[3];

        index[0] = (int)(offsetX + selecter.transform.position.x);
        index[1] = (int)(offsetY + selecter.transform.position.y);
        index[2] = (int)(offsetZ + selecter.transform.position.z);

        return index;
    }


    void LoadPattern()
    {

    }


    public void SavePattern()
    {
        storedPatterns += 1;

        SaveData data = new SaveData(sizeOfGrid);
        data.SetCubes(alives);

        /*        SaveData data = new SaveData();
                data.sizeOfGrid = 19;*/
        Debug.Log(data.sizeOfGrid);
        for(int i = 0; i < data.alives.Length; i++)
        {
            Debug.Log("[" + data.alives[i].x + " " + data.alives[i].y + " " + data.alives[i].z + "] : " + data.alives[i].value);
        }

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "pattern(" + storedPatterns + ").json", json);
        Debug.Log(Application.persistentDataPath);
    }

}

class Alive
{
    public int x;
    public int y;
    public int z;
    public bool value;
}
 
class SaveData
{
    public int sizeOfGrid;

    public Alive[] alives;
    

    public SaveData(int sizeOfGrid)
    {
        this.sizeOfGrid = sizeOfGrid;
        this.alives = new Alive[(int)Math.Pow(sizeOfGrid, 3)];
    }

    public void SetCubes(bool[,,] alives)
    {
        for (int i = 0; i < alives.GetLength(0); i++)
        {
            for (int j = 0; j < alives.GetLength(1); j++)
            {
                for (int k = 0; k < alives.GetLength(2); k++)
                {
                    this.alives[(int)Math.Pow(sizeOfGrid, 2) * k + sizeOfGrid * j + i].x = i;
                    this.alives[(int)Math.Pow(sizeOfGrid, 2) * k + sizeOfGrid * j + i].y = j;
                    this.alives[(int)Math.Pow(sizeOfGrid, 2) * k + sizeOfGrid * j + i].z = k;
                    this.alives[(int)Math.Pow(sizeOfGrid, 2) * k + sizeOfGrid * j + i].value = alives[i, j, k];
                }
            }
        }
    }


}


/* 
//Json Example
{
    "sizeOfGrid" : 5,
    "Alives": [{
        "x": 0,
		"y": 0,
		"z": 0,
		"value": false

    }, {
        "x": 0,
		"y": 0,
		"z": 1,
		"value": false

    }, {
        "x": 0,
		"y": 0,
		"z": 2,
		"value": false

    }]
}
*/