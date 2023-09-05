using System;
using UnityEngine;

public class FieldController : MonoBehaviour
{
    public GameObject unitPrefab;  // Corn section prefab
    public GameObject harvesterPrefab; // Harvester prefab
    public GameObject truckPrefab; // Truck prefab

    private WS_Client wsClient; 

    void Awake()
    {
        CreateGlobalMatrix(); // Create the matrix of corn sections on GlobalData
    }
    void Start()
    {
        wsClient = FindObjectOfType<WS_Client>(); 
        
        CreateField();
        UpdateParentPosition();
        InstantiateHarvestersAndTrucks();

    }
    

    int[] GetLeftRightPos(int[,] startingPoints)
    {
        // Get an initial col and row
        System.Random random = new System.Random();
        int row = random.Next(0, GlobalData.fieldRows);
        int[] possibleCols =  new int[] {-1, GlobalData.fieldCols};
        int col = possibleCols[random.Next(0, possibleCols.Length)];
        
        // Verify that the position is not occupied
        bool isValidPos = false;
        while (!isValidPos)
        {
            isValidPos = true;
            for(int i = 0; i < startingPoints.GetLength(0); i++)
            {
                if (startingPoints[i, 0] == row && startingPoints[i, 1] == col) // Si se encontro un repetido
                {
                    isValidPos = false; 
                    row = random.Next(0, GlobalData.fieldRows);
                    col = possibleCols[random.Next(0, possibleCols.Length)];
                    break;
                }
            }
        }
        
        // return the position
        int[] pos = new int[] {row, col};
        return pos;
    }
    
    int[] GetTopBottomPos(int[,] startingPoints)
    {
        // Get an initial col and row
        System.Random random = new System.Random();
        int col = random.Next(0, GlobalData.fieldCols);
        int[] possibleRows =  new int[] {-1, GlobalData.fieldRows};
        int row = possibleRows[random.Next(0, possibleRows.Length)];
        
        // Verify that the position is not occupied
        bool isValidPos = false;
        while (!isValidPos)
        {
            isValidPos = true;
            for(int i = 0; i < startingPoints.GetLength(0); i++)
            {
                if (startingPoints[i, 0] == row && startingPoints[i, 1] == col) // Si se encontro un repetido
                {
                    isValidPos = false; 
                    col = random.Next(0, GlobalData.fieldCols);
                    row = possibleRows[random.Next(0, possibleRows.Length)];
                    break;
                }
            }
        }
        
        // return the position
        int[] pos = new int[] {row, col};
        return pos;
    }
    void InstantiateHarvestersAndTrucks()
    {
        Harvester[] harvesters = new Harvester[GlobalData.numHarvesters];
        Truck[] trucks = new Truck[GlobalData.numTrucks];
        int[,] startingPoints = new int[GlobalData.numHarvesters, 2];
        
        int trucksInstantiated = 0;
        
        for (int i = 0; i < GlobalData.numHarvesters; i++)
        {
            // Define if it is going to be spawned on the sides or on the top/bottom
            System.Random random = new System.Random();
            int side = random.Next(0, 2); // 0 = left/right, 1 = top/bottom
            int[] pos = new int[2];; 
            if (side == 0)
                pos = GetLeftRightPos(startingPoints);
            else if (side == 1)
                pos = GetTopBottomPos(startingPoints); 
            
            startingPoints[i, 0] = pos[0];
            startingPoints[i, 1] = pos[1];
            
            // Instantiate the harvester
            GameObject harvester = Instantiate(harvesterPrefab);
            Harvester harvesterScript = harvester.GetComponent<Harvester>(); 
            harvesterScript.id = i;
            harvesterScript.currentRow = pos[0];
            harvesterScript.currentCol = pos[1];
            harvesterScript.PutOnPosition(pos[0], pos[1]); 
            harvesters[i] = harvesterScript;
            
            // Instantiate the truck if possible
            if (trucksInstantiated < GlobalData.numTrucks)
            {
                GameObject truck = Instantiate(truckPrefab);
                Truck truckScript = truck.GetComponent<Truck>();
                truckScript.currentRow = pos[0];
                truckScript.currentCol = pos[1];
                truckScript.PutOnPosition(pos[0], pos[1]);
                trucks[i] = truckScript;
                trucksInstantiated++;
            }
        }
        
        // Set the array of harvesters on GlobalData
        GlobalData.harvesters = harvesters;
        GlobalData.harvestersStartingPoints = startingPoints;
        
        // Set the array of trucks on GlobalData
        GlobalData.trucks = trucks;
    }

    void AssignRouteToHarvesters()
    {
        // TODO: Asignar la ruta a cada harvester
    }

    void CreateGlobalMatrix()
    {
        // Create the matrix
        int[,] matrix = new int[GlobalData.fieldRows, GlobalData.fieldCols];

        // Fill the matrix with 1s (not harvested)
        for(int i = 0; i < GlobalData.fieldRows; i++)
        {
            for(int j = 0; j < GlobalData.fieldCols; j++)
            {
                matrix[i, j] = 1;
            }
        }
        
        // Set the matrix on GlobalData
        GlobalData.fieldMatrix = matrix;

        // wsClient.SendCampo(matrix);
    }
    
    void CreateField()
    {
        for (int row = 0; row < GlobalData.fieldRows; row++) // Start generating from top to bottom
        {
            for (int col = 0; col < GlobalData.fieldCols; col++) // Generate from left to right
            {
                GameObject newUnit = Instantiate(unitPrefab, transform);
                
                // Set the position of the new corn section
                newUnit.transform.position = new Vector3(col * GlobalData.unit_xSize, 0, row * GlobalData.unit_zSize);
                newUnit.name = $"Unit({row}, {col})";
            }
        }
    }

    void UpdateParentPosition()
    {
        transform.position += new Vector3(GlobalData.unit_xSize / 2, 0, GlobalData.unit_zSize / 2); 
    }

    
}
