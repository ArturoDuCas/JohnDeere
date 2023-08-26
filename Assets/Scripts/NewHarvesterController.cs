using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewHarvesterController : MonoBehaviour
{
    private float movementSpeed = 2.0f;
    public LayerMask cornLayer;


    void Start()
    {
        GoToUnit(1, 1);
        HarvestUnit(); 
    }


    void GoToUnit(int row, int col)
    {
        // Get the position of the unit
        Vector3 unitLeftPosition = new Vector3(col * 50, 0, row * 6);
        Vector3 unitRightPosition = new Vector3(col * 50 + 50, 0, row * 6);
        
        
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

    void MoveToCoordinates(Vector3 finalPos)
    {
        // float distance = Vector3.Distance(transform.position, finalPos);
        //
        // while (distance > 2f)
        // {
        //     transform.position = Vector3.MoveTowards(transform.position, finalPos, movementSpeed * Time.deltaTime);
        //     distance = Vector3.Distance(transform.position, finalPos);
        // }
    }

    void HarvestUnit()
    {
        Vector3 finishPosition = transform.position; 
        if (transform.rotation.y == 0) // Looking to the right
        {
            finishPosition += new Vector3(50f, 0, 0);
        }
        else // Looking to the left
        {
            finishPosition += new Vector3(-50f, 0, 0);
        }
        
        StartCoroutine(HarvestCoroutine(finishPosition));
    }
    
    IEnumerator HarvestCoroutine(Vector3 finishPosition)
    {
        float distance = Vector3.Distance(transform.position, finishPosition);
        while (distance >= 0f)
        {
            transform.position = Vector3.MoveTowards(transform.position, finishPosition, movementSpeed * Time.deltaTime);
            distance = Vector3.Distance(transform.position, finishPosition);
            yield return null;
        }
    }
    
}
