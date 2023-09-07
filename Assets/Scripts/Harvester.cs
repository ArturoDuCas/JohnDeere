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

    public int grainCapacity = 20; 
    public int grainLoad = 0;

    public bool isWaitingForTruck = false; 

    public bool ayudando_harvester = false; 
    


    private WS_Client wsClient; 

    private Vector3 posCorrectionRight = new Vector3(-5.5f, 0f, 2.6f); 
    private Vector3 posCorrectionUp  = new Vector3(-3.2f, 0f, -5.4f);
    private Vector3 posCorrectionLeft = new Vector3(5.2f, 0f, 2.8f);
    private Vector3 posCorrectionDown = new Vector3(-3.2f, 0f, 5.1f); 
    
    public ParticleSystem unloadParticlesPrefab;
    private ParticleSystem unloadParticles;
    public ParticleSystem harvestParticlesPrefab;
    private ParticleSystem harvestParticles;
    
    public bool isBeingHelped = false;
    


    void Start()
    {
        harvestParticles = Instantiate(harvestParticlesPrefab, transform); 
        isWaitingForTruck = false;
        

        wsClient = FindObjectOfType<WS_Client>(); // Find the WebSocket client script
    }

    bool verifyTruckAviability()
    {
        // returns true if at least one truck is aviable
        for (int i = 0; i < GlobalData.numTrucks; i++)
        {
            if (GlobalData.trucks[i].isAviable)
            {
                return true; 
            }
        }
        
        return false;
    }


    void Update()
    {

        if (fuel <= 0) // if fuel is over, stop moving
            return;
        

        
        if(isWaitingForTruck)
        {
            if (verifyTruckAviability())
            {
                isWaitingForTruck = false; 
                wsClient.SendHarvesterUnloadRequest((int)lastPos.x, (int)lastPos.y, id);
            }
            
            return; 
        }
        
        
        if (path.Count == 0)
        {
            // ayudando_harvester = true; 

            List<int> num_harv = new List<int>();

            Debug.Log("Entra al otro elsee ------------- ");

            for(int i = 0; i < GlobalData.harvesters.Length; i++){
                Debug.Log("ENTRA AL FOR");

                if( !GlobalData.harvesters[i].finishedPath ){
                    num_harv.Add(GlobalData.harvesters[i].id);
                }
            }

            if(num_harv.Count >= 1){
            
                // finishedPath = false; 
                // HelpHarvester(GlobalData.harvesters[num_harv[0]].path);
                
                List<Vector2> otra_path = GlobalData.harvesters[num_harv[0]].path;
                otra_path.Reverse(); 
                path = otra_path;
            
            }
            return;
        }
        
        
        if(grainCapacity <= grainLoad) // If the harvester  is full
        {
            return; 
        }
        
        
        if (!finishedPath) // If the path is not finished
        {
            if (!isMoving) // If the harvester is not moving
            {
                // if (Common.isGoingToCrash(path[0]))
                // {
                //     AddRightUpLeftPath(); 
                // }
                // else
                // {
                    GetMovement(); 
                    
                // }
            }
        }
        // else{

        //     List<int> num_harv = new List<int>();

        //     Debug.Log("Entra al otro elsee ------------- ");

        //     for(int i = 0; i < GlobalData.harvesters.Length; i++){
        //         Debug.Log("ENTRA AL FOR");

        //         if( !GlobalData.harvesters[i].finishedPath ){
        //             num_harv.Add(GlobalData.harvesters[i].id);
        //         }
        //     }

        //     if(num_harv.Count >= 1){
        //         if(!isMoving){

        //             HelpHarvester(GlobalData.harvesters[num_harv[0]].path);
        //         }
        //     }

        //     // if(id == 0){

        //     //     HelpHarvester(GlobalData.harvesters[1].path); 
        //     // }else{
        //     //     HelpHarvester(GlobalData.harvesters[0].path); 

        //     // }
        // }

        
    }

    // void HelpHarvester(List<Vector2> new_path){
    //     new_path.Reverse(); 

    //     if(new_path.Count == 0)
    //     {
    //         // finishedPath = true;
    //         // ayudando_harvester = false; 
    //         return; 
    //     }
        
    //     lastPos = new Vector2(currentRow, currentCol); 
        
    //     isMoving = true;
    //     if(GlobalData.fieldMatrix[(int) new_path[0].x, (int) new_path[0].y] == 1) // if the unit has corn
    //     {
    //         harvestParticles.Play();
    //     }
        
    //     if (currentCol < new_path[0].y) // Se mueve a la derecha
    //     {
    //         HarvestRight(); 
    //     } else if (currentCol > new_path[0].y) // Se mueve a la izquierda
    //     {
    //         HarvestLeft(); 
    //     } else if (currentRow < new_path[0].x) // Se mueve hacia arriba
    //     {
    //         HarvestUp(); 
    //     } else if (currentRow > new_path[0].x) // Se mueve hacia abajo
    //     {
    //         HarvestDown(); 
    //     }
    //     else
    //     {
    //         isMoving = false; 
    //     }
        
        
    //     // Remove the first element of the array
    //     path.RemoveAt(0);
    // }
    

    void AddRightUpLeftPath()
    {
        // delete the first element of the path
        path.RemoveAt(0);
        Vector2 right = new Vector2(); ;
        Vector2 up = new Vector2(); ; 
        Vector2 left = new Vector2(); ;
        
        if (transform.rotation.y == 180f) // looking down
        {
            right = new Vector2(currentRow, currentCol -1);
            up = right + new Vector2(-1,0f); 
            left = up + new Vector2(0f, +1f);
        } else if (transform.rotation.y == 270f || transform.rotation.y == -90f) // looking left
        {
            right = new Vector2(currentRow + 1, currentCol);
            up = right + new Vector2(-1,0f); 
            left = up + new Vector2(0f, -1f);
        } else if(transform.rotation.y == 0f) // looking up
        {
            right = new Vector2(currentRow, currentCol + 1);
            up = right + new Vector2(1,0f); 
            left = up + new Vector2(0f, -1f);
        } else if(transform.rotation.y == 90f) // looking right
        {
            right = new Vector2(currentRow - 1, currentCol);
            up = right + new Vector2(1,0f); 
            left = up + new Vector2(0f, +1f);
        }
        
        path.Insert(0, left);
        path.Insert(0, up);
        path.Insert(0, right);

    }

    void GetMovement()
    { 
        if(path.Count == 0)
        {
            finishedPath = true;
            ayudando_harvester = true; 
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
        else
        {
            isMoving = false; 
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

    public void UnloadGrain(int truckId)
    {
        StartCoroutine(UnloadProcessCoroutine(truckId)); 
    }

    IEnumerator UnloadProcessCoroutine(int truckId)
    {
        yield return new WaitForSeconds(.5f); 
        
        unloadParticles = Instantiate(unloadParticlesPrefab, transform); 

        // Espera 5 segundos
        yield return new WaitForSeconds(5f);
        
        Destroy(unloadParticles);
        GlobalData.trucks[truckId].grainLoad += grainLoad; 
        
        grainLoad = 0;
        isBeingHelped = false; 
        
        
        // Si se lleno el truck
        if (GlobalData.trucks[truckId].grainLoad >= GlobalData.trucks[id].grainCapacity)
        {
            GlobalData.trucks[truckId].isAviable = false;
            GlobalData.trucks[truckId].GoToSilos();
        }
        else
        {
            GlobalData.trucks[truckId].isAviable = true;
            // TODO: LLamar a la funcion de brindar soporte
        }
        
    }
    
    
    
    
    IEnumerator HarvestCoroutine(Vector3 finishPosition)
    {
        bool hadCorn; // Verify if it is a unit that had corn 
        if(GlobalData.fieldMatrix[currentRow, currentCol] == 1)
            hadCorn = true; 
        else
            hadCorn = false; 
        
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

        if (hadCorn) // if it is a unit that had corn
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














