using System.Collections;
using UnityEngine;
using System.Collections.Generic;



public class Truck : MonoBehaviour
{
    public int id; 
    
    public int currentRow; 
    public int currentCol; 
    
    private float movementSpeed = 6.0f;
    private float actualSpeed;

    private float fuel = 1200; 
    float fuelConsumption = 20;

    public List<Vector2> path; 

    
    public bool isMoving = false;
    public bool isGoingToFinish = false; 
        
        
    public  int grainCapacity = 20; 
    public int grainLoad = 0;
    
    public bool isAviable = true;
    public int targetHarvester;

    public bool isReturning = false; // To know if it is returning to the storage or not
    
    private WS_Client wsClient; 
    void Start()
    {
        grainCapacity = 20; // TODO: Eliminar esto
        wsClient = FindObjectOfType<WS_Client>(); // Find the WebSocket client script
    }

    void Update()
    {
        if(fuel <= 0) // if fuel is over, stop moving
            return;
        
        
        
        if (isReturning) // Si esta llendo al almacen
        {
            if(path.Count == 0) // Si ya llego al almacen
            {
                isReturning = false;
                isAviable = true;
                grainLoad = 0;
                return;
            }
            
            if (!isMoving) // if the truck is not moving
            {
                GetFixedMovement(); 
            }
            
            
        }
        else
        {
            if (path.Count == 0 ) // if there is no path, return
            {
                if (isGoingToFinish) {
                    UnloadGrain();
                    isGoingToFinish = false; // para llamar una vez a la funcion de descargar maiz
                }
                return;
            }
            
        
            if (!isMoving) // if the truck is not moving
            {
                GetMovement(); 
            }

        }
        
    }

    void UnloadGrain()
    {
        GlobalData.harvesters[targetHarvester].UnloadGrain(id);
    }
    
    void GetFixedMovement()
    {
        isMoving = true;

        int fixedCol = currentCol + 1; 
        int fixedRow = currentRow + 1;
        
        
        if (fixedCol < path[0].y) // Se mueve a la derecha
        {
            MoveRight(); 
        } else if (fixedCol > path[0].y) // Se mueve a la izquierda
        {
            MoveLeft(); 
        } else if (fixedRow < path[0].x) // Se mueve hacia arriba
        {
            MoveUp(); 
        } else if (fixedRow > path[0].x) // Se mueve hacia abajo
        {
            MoveDown(); 
        }
        else
        {
            isMoving = false;
        }
        
        // Remove the first element of the array
        path.RemoveAt(0);
    }
    
    void GetMovement()
    {
        if (path.Count == 1)
        {
            isGoingToFinish = true;
        }
        
        isMoving = true;
        
        if (currentCol < path[0].y) // Se mueve a la derecha
        {
            MoveRight(); 
        } else if (currentCol > path[0].y) // Se mueve a la izquierda
        {
            MoveLeft(); 
        } else if (currentRow < path[0].x) // Se mueve hacia arriba
        {
            MoveUp(); 
        } else if (currentRow > path[0].x) // Se mueve hacia abajo
        {
            MoveDown(); 
        }
        else
        {
            isMoving = false;
        }
        
        // Remove the first element of the array
        path.RemoveAt(0);
    }

    public void GoToSilos()
    {
        wsClient.SendTruckToSilos(currentRow, currentCol, id);    
    }
    
    public void PutOnPosition(int row, int col)
    {
        if (col == -1) // starting on the left side
        {
            transform.position = new Vector3(0, 0, row * GlobalData.unit_zSize) + new Vector3(-12f, 0, 2f); 
            transform.rotation = Quaternion.Euler(0, 270, 0);
        } else if(col == GlobalData.fieldCols) // starting on the right side
        {
            transform.position = new Vector3(col * GlobalData.unit_xSize + GlobalData.unit_xSize, 0, row * GlobalData.unit_zSize) + new Vector3(6.5f, 0, 2f);
            transform.rotation = Quaternion.Euler(0, 90, 0);
        } else if(row == -1) // starting on the bottom side
        {
            transform.position = new Vector3(col * GlobalData.unit_xSize, 0, 0) + new Vector3(2.5f, 0, -12f);
            transform.rotation = Quaternion.Euler(0, 180, 0);
        } else if(row == GlobalData.fieldRows) // starting on the top side
        {
            transform.position = new Vector3(col * GlobalData.unit_xSize, 0, row * GlobalData.unit_zSize + GlobalData.unit_zSize) + new Vector3(3f, 0, 5f);
            transform.rotation = Quaternion.Euler(0, 0, 0);
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
        } else if (previousYRotation == 0) // if was looking down 
        {
            transform.position += new Vector3(-0.8f, 0f, -1.4f); 
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
        if(previousYRotation == -90 || previousYRotation == 270f) // if was looking right
        {
            transform.position += new Vector3(0.8f, 0f, 0.2f); 
        } else if (previousYRotation == 90f) // if was looking left
        {
            transform.position += new Vector3(-1.1f, 0f, 0f); 
        } else if (previousYRotation == 0) // if was looking down
        {
            transform.position += new Vector3(0f, 0f, 0f); 
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
        } else if (previousYRotation == 0) // if was looking down
        {
            transform.position += new Vector3(1.8f, 0f, -1.8f);
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
        } else if (previousYRotation == 270 || previousYRotation == -90) // if was looking right
        {
            transform.position += new Vector3(1.2f, 0f, 1f); 
        } else if (previousYRotation == previousYRotation) // if was looking left
        {
            transform.position += new Vector3(-1f, 0f, 1f); 
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
        isMoving = false;
    }
}
