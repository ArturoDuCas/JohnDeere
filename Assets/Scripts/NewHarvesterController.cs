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
    


    public Vector2 currentUnit; 
    

    private WS_Client wsClient; 
    

    void Start()
    {
<<<<<<< Updated upstream
        GoToUnit(0,0);
        HarvestUnit(); 
        InvokeRepeating("DecreaseGas", 0.0f, 5.0f);
        wsClient = FindObjectOfType<WS_Client>(); // Find the WebSocket client script

=======
        // currentUnit = new Vector2(0,0);
        // GoToUnit(0,0);
        // HarvestUnit();
>>>>>>> Stashed changes
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
<<<<<<< Updated upstream
           gas -= 1;
            Debug.Log("Gas decreased. Current gas: " + gas );
            wsClient.SendGasCapacity(gas);
        }
        else
        {
            Debug.Log("Out of gas!");
=======
            GoUp(); 
>>>>>>> Stashed changes
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
            transform.position = unitLeftPosition;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            transform.position = unitRightPosition + new Vector3(0, 0, 6); 
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    void GoUp()
    {
        transform.rotation = Quaternion.Euler(0, -90, 0);
        transform.position += new Vector3(0, 0, 6);
    }

    
    // precondition: harvester must be on the right or left side of the unit
    void HarvestUnit()
    {
        Vector3 finishPosition = transform.position;
        if(transform.rotation.y == 0) // Looking to the right
        {
            finishPosition += new Vector3(GlobalData.unit_xSize, 0, 0);
        }
        else // Looking to the left
        {
            finishPosition += new Vector3(-GlobalData.unit_xSize, 0, 0);
        }
        
        StartCoroutine(HarvestCoroutine(finishPosition));
    }
    
    IEnumerator HarvestCoroutine(Vector3 finishPosition)
    {
        GlobalData.fieldMatrix[(int)currentUnit.x, (int)currentUnit.y] = 2;
        
        float distance = Vector3.Distance(transform.position, finishPosition);

        while (distance >= 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, finishPosition, movementSpeed * Time.deltaTime);
            distance = Vector3.Distance(transform.position, finishPosition);
            yield return null;
        }
        
        fuel -= fuelConsumption;
        GlobalData.fieldMatrix[(int)currentUnit.x, (int)currentUnit.y] = 0; // TODO: No cambia a 0 
    }
    
}
