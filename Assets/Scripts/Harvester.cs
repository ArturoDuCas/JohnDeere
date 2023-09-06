using System;
using System.Collections;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;


public class Harvester : MonoBehaviour
{
    public LayerMask cornLayer;

    public int id;
    
    private float movementSpeed = 4.0f;
    private float actualSpeed; 

    public float fuel = 1000; //Se asigna en WS_Client 
    private float fuelConsumption = 20;
    

    
    public int currentRow; 
    public int currentCol;

    public List<Vector2> path;
    public Vector2 lastPos; 

    public bool isMoving = false; 
    public bool finishedPath = false;

    private int grainCapacity = 20; 
    public int grainLoad = 0;
    


    private WS_Client wsClient; 

    private Vector3 posCorrectionRight = new Vector3(-5.5f, 0f, 2.6f); 
    private Vector3 posCorrectionUp  = new Vector3(-3.2f, 0f, -5.4f);
    private Vector3 posCorrectionLeft = new Vector3(5.2f, 0f, 2.8f);
    private Vector3 posCorrectionDown = new Vector3(-3.2f, 0f, 5.1f); 
    
    public ParticleSystem unloadParticlesPrefab;
    private ParticleSystem unloadParticles;
    public ParticleSystem harvestParticlesPrefab;
    private ParticleSystem harvestParticles;
    


    void Start()
    {
        harvestParticles = Instantiate(harvestParticlesPrefab, transform); 
        
        

        wsClient = FindObjectOfType<WS_Client>(); // Find the WebSocket client script
    }


    void Update()
    {
        if (path.Count == 0)
        {
            return;
        }
        
        
        if(fuel <= 0 || grainCapacity <= grainLoad) // If the harvester has no fuel or is full
        {
            return; 
        }
        
        
        if (!finishedPath) // If the path is not finished
        {
            if (!isMoving) // If the harvester is not moving
            {
                GetMovement(); 
            }
        }
        
    }
    

    void GetMovement()
    { 
        if(path.Count == 0)
        {
            finishedPath = true;
            return; 
        }
        
        lastPos = new Vector2(currentRow, currentCol); 
        
        isMoving = true;
        if(GlobalData.fieldMatrix[(int) path[0].x, (int) path[0].y] == 1) // if the unit has corn
        {
            harvestParticles.Play();
        }
        
        if (currentCol < path[0].y) // Se mueve a la derecha
        {
            HarvestRight(); 
        } else if (currentCol > path[0].y) // Se mueve a la izquierda
        {
            HarvestLeft(); 
        } else if (currentRow < path[0].x) // Se mueve hacia arriba
        {
            HarvestUp(); 
        } else if (currentRow > path[0].x) // Se mueve hacia abajo
        {
            HarvestDown(); 
        }
        
        
        // Remove the first element of the array
        path.RemoveAt(0);
    }
    
    

    public void PutOnPosition(int row, int col)
    {
        if (col == -1) // starting on the left side
        {
            transform.position = new Vector3(0f, 0, row * GlobalData.unit_zSize) + new Vector3(-5.5f, 0, 2.7f); // -3.6 + 5.5
            transform.rotation = Quaternion.Euler(0, 90, 0);
        } else if(col == GlobalData.fieldCols) // starting on the right side
        {
            transform.position = new Vector3(col * GlobalData.unit_xSize + GlobalData.unit_xSize, 0, row * GlobalData.unit_zSize) + new Vector3(-0.5f, 0, 2.7f); 
            transform.rotation = Quaternion.Euler(0, 270, 0);
        } else if (row == -1) // starting on the bottom
        {
            transform.position = new Vector3(col * GlobalData.unit_xSize, 0, 0) + new Vector3(2.8f, 0, -5f);
            transform.rotation = Quaternion.Euler(0, 0, 0); 
        } else if(row == GlobalData.fieldRows) // starting on the top
        {
            transform.position = new Vector3(col * GlobalData.unit_xSize, 0, row * GlobalData.unit_zSize + GlobalData.unit_zSize) + new Vector3(2.8f, 0, -1f);
            transform.rotation = Quaternion.Euler(0, 180, 0); 
        }
    }

    void HarvestUp()
    {
        // Rotate the harvester before moving
        float previousYRotation = transform.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        if (previousYRotation == 90f) // if was looking right
        {
            transform.position += new Vector3(2.2f, 0f, -1f); 
        } else if(previousYRotation == 270f || previousYRotation == -90f) // if was looking left
        {
            transform.position += new Vector3(-2.4f, 0f, -2f); 
        }
        
        // Get the position of the unit
        currentRow += 1;
        Vector3 finishPosition = new Vector3(currentCol * GlobalData.unit_xSize + GlobalData.unit_xSize, 0, currentRow * GlobalData.unit_zSize + GlobalData.unit_zSize) + posCorrectionUp;

        StartCoroutine(HarvestCoroutine(finishPosition));
    }
    
    
    void HarvestRight()
    {
        // Rotate the harvester before moving
        float previousYRotation = transform.eulerAngles.y;
        if(previousYRotation == 0f) // if was looking up
        {
            transform.position += new Vector3(-1.7f, 0, 2.1f);
        } else if (Math.Abs(previousYRotation) == 180f) // is was looking down
        {
            transform.position += new Vector3(-2.5f, 0f, -2.5f); 
        } else if (previousYRotation == -90f || previousYRotation == 270f) // if was looking left
        {
            transform.position += new Vector3(-3.2f, 0f, 0f); 
        }
        
        transform.rotation = Quaternion.Euler(0, 90, 0); 
        
        // Get the finish position
        currentCol += 1;
        Vector3 finishPosition = new Vector3(currentCol * GlobalData.unit_xSize + GlobalData.unit_xSize, 0, currentRow * GlobalData.unit_zSize) + posCorrectionRight;
        
        StartCoroutine(HarvestCoroutine(finishPosition));
    }

    void HarvestLeft()
    {
        // Rotate the harvester before moving
        float previousYRotation = transform.eulerAngles.y;
        if(previousYRotation == 0f) // if was looking up
        {
            transform.position += new Vector3(2.2f, 0, 2.1f);
        } else if (previousYRotation == 90f) // if was looking right
        {
            transform.position += new Vector3(3.5f, 0f, 0f); 
        } else if(Math.Abs(previousYRotation) == 180f) // is was looking down
        {
            transform.position += new Vector3(2f, 0f, -2.4f); 
        }
        
        transform.rotation = Quaternion.Euler(0, 270, 0); 
        
        // Get the finish position
        currentCol -= 1; 
        Vector3 finishPosition = new Vector3(currentCol * GlobalData.unit_xSize, 0, currentRow * GlobalData.unit_zSize) + posCorrectionLeft;

        StartCoroutine(HarvestCoroutine(finishPosition)); 
    }


    void HarvestDown()
    {
        // Rotate the harvester before moving
        float previousYRotation = transform.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0, 180, 0);
        if (previousYRotation == 90f) // if was looking right
        {
            transform.position += new Vector3(2.3f, 0f, 2f); 
        } else if(previousYRotation == -90 || previousYRotation == 270f) // if was looking left
        {
            transform.position += new Vector3(-2.4f, 0f, 1.8f); 
        }
        
        // Get the finish position
        currentRow -= 1;
        Vector3 finishPosition = new Vector3(currentCol * GlobalData.unit_xSize + GlobalData.unit_xSize, 0, currentRow * GlobalData.unit_zSize) + posCorrectionDown;
        
        StartCoroutine(HarvestCoroutine(finishPosition));

    }
    
    // Function to convert a 2D integer array to a JSON string
    public static string ConvertMatrixToJson(int[,] matrix)
    {
        // Create a class to hold your matrix
        var matrixData = new
        {
            Data = matrix
        };

        // Convert the matrix data to a JSON string
        string json = JsonConvert.SerializeObject(matrixData);

        return json;
    }

    public void UnloadGrain(int id)
    {
        StartCoroutine(UnloadProcessCoroutine(id)); 
    }

    IEnumerator UnloadProcessCoroutine(int id)
    {
        yield return new WaitForSeconds(.5f); 
        
        unloadParticles = Instantiate(unloadParticlesPrefab, transform); 

        // Espera 5 segundos
        yield return new WaitForSeconds(5f);
        
        Destroy(unloadParticles);
        GlobalData.trucks[id].grainLoad += grainLoad; 
        GlobalData.trucks[id].isAviable = true;
        
        grainLoad = 0;
        
    }
    
    
    
    
    IEnumerator HarvestCoroutine(Vector3 finishPosition)
    {
        GlobalData.fieldMatrix[currentRow, currentCol] = 2;
        
        float distance = Vector3.Distance(transform.position, finishPosition);

        while (distance >= 0.01f)
        {
            actualSpeed = movementSpeed * Time.deltaTime; 
            transform.position = Vector3.MoveTowards(transform.position, finishPosition, actualSpeed);
            distance = Vector3.Distance(transform.position, finishPosition);
            yield return null;

            // wsClient.SendSpeed(actualSpeed);
    
        }
        
        fuel -= fuelConsumption;
        grainLoad += GlobalData.grainsPerUnit;
        wsClient.SendCapacidad(grainLoad);
        
        isMoving = false; 
        harvestParticles.Stop();
        
        
        
        if (grainLoad >= grainCapacity) // if the harvester is full, call the truck
        {
            wsClient.SendHarvesterUnloadRequest((int)lastPos.x, (int)lastPos.y, id);
        }
        
        GlobalData.fieldMatrix[currentRow, currentCol] = 0; 
        
        
        
        wsClient.SendGasCapacity(fuel);
        
            
    }
    
}














