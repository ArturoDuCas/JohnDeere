using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewHarvesterController : MonoBehaviour
{
    public LayerMask cornLayer;
    
    private float movementSpeed = 2.0f;

    public float fuel = 100; 
    public float fuelConsumption = 20;
    

    
    public int currentRow; 
    public int currentCol;


    private WS_Client wsClient; 
    private WS_Client wsClient2; 

    private Vector3 posCorrectionRight = new Vector3(-5.5f, 0f, 2.6f); 
    private Vector3 posCorrectionUp  = new Vector3(-3.2f, 0f, -5.4f);
    private Vector3 posCorrectionLeft = new Vector3(0.2f, 0f, 0f);


    void Start()
    {
        currentRow = 0;
        currentCol = -1;
        GoToUnit(0,0);

        wsClient = FindObjectOfType<WS_Client>(); // Find the WebSocket client script
        wsClient2 = FindObjectOfType<WS_Client>();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            HarvestUp();
        } else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            HarvestRight(); 
        } else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            HarvestLeft();
        }
        
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

    IEnumerator GoUpCoroutine(int finishZPosition)
    {
        Vector3 finishPosition = new Vector3(transform.position.x, transform.position.y, finishZPosition);
        float distance = Vector3.Distance(transform.position, finishPosition);
        
        while (distance >= 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, finishPosition, movementSpeed * Time.deltaTime);
            distance = Vector3.Distance(transform.position, finishPosition);
            yield return null;
        }
        
        fuel -= fuelConsumption / 3;
    }

    void HarvestRight()
    {
        // Rotate the harvester before moving
        float previousYRotation = transform.eulerAngles.y;
        if(previousYRotation == 0f) // if was looking up
        {
            transform.position += new Vector3(-1.7f, 0, 2.1f);
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
        }
        
        transform.rotation = Quaternion.Euler(0, 270, 0); 
        
        // Get the finish position
        Vector3 finishPosition = transform.position;
        finishPosition += new Vector3(-GlobalData.unit_xSize, 0, 0) + posCorrectionLeft;
        currentCol -= 1;

        StartCoroutine(HarvestCoroutine(finishPosition)); 
    }
    
    // precondition: harvester must be on the right or left side of the unit
    void HarvestUnit()
    {
        Vector3 finishPosition = transform.position;
        if(transform.rotation.y == 0) // Looking to the right
        {
            finishPosition += new Vector3(GlobalData.unit_xSize, 0, 0);
            currentCol += 1;
        }
        else // Looking to the left
        {
            finishPosition += new Vector3(-GlobalData.unit_xSize, 0, 0);
            currentCol -= 1;
        }
        
        StartCoroutine(HarvestCoroutine(finishPosition));
    }
    
    IEnumerator HarvestCoroutine(Vector3 finishPosition)
    {
        GlobalData.fieldMatrix[currentRow, currentCol] = 2;
        
        float distance = Vector3.Distance(transform.position, finishPosition);

        while (distance >= 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, finishPosition, movementSpeed * Time.deltaTime);
            const HarvesterSpeed = movementSpeed * Time.deltaTime; 
            distance = Vector3.Distance(transform.position, finishPosition);
            yield return null;

            wsClient2.SendSpeed(HarvesterSpeed );
    
        }
        
        fuel -= fuelConsumption;
        GlobalData.fieldMatrix[currentRow, currentCol] = 0; 
    }
    
}














