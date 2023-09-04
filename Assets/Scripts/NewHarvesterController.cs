using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NewHarvesterController : MonoBehaviour
{
    public LayerMask cornLayer;
    
    private float movementSpeed = 2.0f;
    private float actualSpeed; 

    private float fuel = 1000; 
    private float fuelConsumption = 20;
    

    
    public int currentRow; 
    public int currentCol;
    
    public Vector2[] path =
        
    {
        new Vector2(0,0),
        new Vector2(0,1), 
        new Vector2(0,2),
        new Vector2(0,3),
        new Vector2(1, 3),
        new Vector2(1,2), 
        new Vector2(1,1),
        new Vector2(1,0),
    };

    public bool isMoving = false; 
    public bool finishedPath = false;


    private WS_Client wsClient; 
    // private WS_Client wsClient2; 

    private Vector3 posCorrectionRight = new Vector3(-5.5f, 0f, 2.6f); 
    private Vector3 posCorrectionUp  = new Vector3(-3.2f, 0f, -5.4f);
    private Vector3 posCorrectionLeft = new Vector3(5.2f, 0f, 2.8f);
    private Vector3 posCorrectionDown = new Vector3(-3.2f, 0f, 5.1f); 
    
    public ParticleSystem harvestParticlesPrefab;
    private ParticleSystem harvestParticles;


    void Start()
    {
        currentRow = 0;
        currentCol = -1;
        GoToUnit(0,0);

        Instantiate(harvestParticlesPrefab, transform); 
        harvestParticles = GetComponentInChildren<ParticleSystem>();

        wsClient = FindObjectOfType<WS_Client>(); // Find the WebSocket client script
    }


    void Update()
    {
        Debug.Log(GlobalData.selfID); 
        if(fuel <= 0) // If the harvester has no fuel
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
        if(path.Length == 0)
        {
            finishedPath = true;
            return; 
        }
        
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
        
        path = path[1..]; // Remove the first element of the array
    }
    
    

    void GoToUnit(int row, int col)
    {
        // Get the position of the unit
        Vector3 unitLeftPosition = new Vector3(col * GlobalData.unit_xSize, 0, row * GlobalData.unit_zSize);
        Vector3 unitRightPosition = new Vector3(col * GlobalData.unit_xSize + GlobalData.unit_xSize, 0, row * GlobalData.unit_zSize);
        
        
        // Get the distance to the unit
        float distanceToLeft = Vector3.Distance(transform.position, unitLeftPosition); 
        float distanceToRight;
        if(transform.rotation.y == 0)
        {
            distanceToRight = Vector3.Distance(transform.position, unitRightPosition + new Vector3(0, 0, 6));
        }
        else
        {
            distanceToRight = Vector3.Distance(transform.position, unitRightPosition);
        }
        
        // Move to the closest distance to the unit
        if (distanceToLeft < distanceToRight)
        {
            // MoveToCoordinates(unitLeftPosition);
            transform.position = unitLeftPosition + posCorrectionRight;
            transform.rotation = Quaternion.Euler(0, 90, 0);
        }
        else
        {
            transform.position = unitRightPosition + new Vector3(0, 0, 6); 
            transform.rotation = Quaternion.Euler(0, 90, 0);
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
        wsClient.SendGasCapacity(fuel);
        GlobalData.fieldMatrix[currentRow, currentCol] = 0; 
        Common.printMatrix(GlobalData.fieldMatrix);
        isMoving = false; 
        harvestParticles.Stop();
        
        
        wsClient.SendCampo(GlobalData.fieldMatrix);
    }
    
}














