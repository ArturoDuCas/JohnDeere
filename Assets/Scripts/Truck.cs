using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Truck : MonoBehaviour
{
    public int currentRow; 
    public int currentCol; 
    
    public float movementSpeed = 3.0f;
    private float actualSpeed;

    private float fuel = 1200; 
    float fuelConsumption = 20;
    
    void Start()
    {
        currentRow = 0;
        currentCol = -1;
        PutOnPosition(0, 0); // index is 0-based
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveRight();
        } else if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveUp();
        } else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveLeft(); 
        } else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveDown(); 
        }
    }

    void PutOnPosition(int row, int col)
    {
        if (col == 0) // starting on the left side
        {
            transform.position = new Vector3(0, 0, row * GlobalData.unit_zSize) + new Vector3(-4f, 0, 2f); 
            transform.rotation = Quaternion.Euler(0, 270, 0);
        } else if(col == GlobalData.fieldCols - 1) // starting on the right side
        {
            transform.rotation = Quaternion.Euler(0, 90, 0);
            transform.position = new Vector3(col * GlobalData.unit_xSize + GlobalData.unit_xSize, 0, row * GlobalData.unit_zSize) + new Vector3(4f, 0, 2f);
        }
    }

    void MoveRight()
    {
        // Rotate the truck before moving 
        float previousYRotation = transform.eulerAngles.y;
        if (previousYRotation == 180) // if was looking up
        {
            transform.position += new Vector3(0f, 0f, 0f);
        } else if(previousYRotation == 90) // if was looking left
        {
            transform.position += new Vector3(0f, 0f, 0f);
        } 
        transform.rotation = Quaternion.Euler(0, 270, 0); 
        
        // Get the finish Position
        currentCol += 1;
        Vector3 finishPosition = new Vector3(currentCol * GlobalData.unit_xSize + GlobalData.unit_xSize, 0, currentRow * GlobalData.unit_zSize) + new Vector3(-3.8f, 0, 2f);
        
        StartCoroutine(MoveTruckCoroutine(finishPosition));
    }

    void MoveUp()
    {
        // Rotate the truck before moving
        float previousYRotation = transform.eulerAngles.y;
        if(previousYRotation == -90 || previousYRotation == 270f) // if was looking left
        {
            transform.position += new Vector3(0.8f, 0f, 0.2f); 
        }
        transform.rotation = Quaternion.Euler(0, 180, 0); 
        
        // Get the finish position 
        currentRow += 1;
        Vector3 finishPosition = new Vector3(currentCol * GlobalData.unit_xSize + GlobalData.unit_xSize, 0, currentRow * GlobalData.unit_zSize) + new Vector3(-3f, 0, 2.2f);
        
        StartCoroutine(MoveTruckCoroutine(finishPosition));
        
    }

    void MoveLeft()
    {
        // Rotate the truck before moving
        float previousYRotation = transform.eulerAngles.y;
        if(previousYRotation == 270 || previousYRotation == -90) // if was looking right
        {
            transform.position += new Vector3(0f, 0f, 0f); 
        } else if(previousYRotation == 180) // if was looking up
        {
            transform.position += new Vector3(0f, 0f, 0f); 
        }
        transform.rotation = Quaternion.Euler(0, 90, 0);
        
        // Get the finish position
        currentCol -= 1;
        Vector3 finishPosition = new Vector3(currentCol * GlobalData.unit_xSize, 0, currentRow * GlobalData.unit_zSize) + new Vector3(3.8f, 0, 2f);
        
        StartCoroutine(MoveTruckCoroutine(finishPosition));
    }

    void MoveDown()
    {
        // Rotate the truck before moving
        float previousYRotation = transform.eulerAngles.y;
        if (previousYRotation == 180) // if was looking up
        {
            transform.position += new Vector3(0f, 0f, 0f); 
        }
        
        transform.rotation = Quaternion.Euler(0, 0, 0);
        
        // Get the finish position
        currentRow -= 1;
        Vector3 finishPosition = new Vector3(currentCol * GlobalData.unit_xSize, 0, currentRow * GlobalData.unit_zSize) + new Vector3(3f, 0, 3.8f);
        
        StartCoroutine(MoveTruckCoroutine(finishPosition));
        
    }
    
    IEnumerator MoveTruckCoroutine(Vector3 finishPosition)
    {  
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
    }
}
