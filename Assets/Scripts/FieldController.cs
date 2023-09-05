using System;
using UnityEngine;

public class FieldController : MonoBehaviour
{
    public GameObject unitPrefab;  // Corn section prefab
    public GameObject harvesterPrefab; // Harvester prefab

    private WS_Client wsClient; 

    void Awake()
    {
        CreateGlobalMatrix(); // Create the matrix of corn sections on GlobalData
    }
    void Start()
    {
        CreateField();
        UpdateParentPosition();
        // wsClient = FindObjectOfType<WS_Client>(); 

        InstantiateHarvesters(); 
    }
    
    
    void InstantiateHarvesters()
    {
        Harvester[] harvesters = new Harvester[GlobalData.numHarvesters];
        int[,] startingPoints = new int[GlobalData.numHarvesters, 2];
        
        for (int i = 0; i < GlobalData.numHarvesters; i++)
        {
            // Get the initial col and row of the harvester
            System.Random random = new System.Random();
            int row = random.Next(0, GlobalData.fieldRows);
            int[] possibleCols =  new int[] {0, GlobalData.fieldCols - 1};
            int col = possibleCols[random.Next(0, possibleCols.Length)];
            if(col == 0)
                col += -1;
            else
                col += 1;
            
            // Verify that the position is not occupied
            for(int j = 0; j < harvesters.Length; j++)
            {
                if (harvesters[j] == null)
                    break; 
                
                if(harvesters[j].currentRow == row && harvesters[j].currentCol == col)
                {
                    row = random.Next(0, GlobalData.fieldRows);
                    col = possibleCols[random.Next(0, possibleCols.Length)];
                    if(col == 0)
                        col += -1;
                    else 
                        col += 1;
                }
            }
            
            
            // Instantiate the harvester
            GameObject harvester = Instantiate(harvesterPrefab);
            Harvester harvesterScript = harvester.GetComponent<Harvester>(); 
            harvesterScript.currentRow = row;
            harvesterScript.currentCol = col;
            harvesterScript.PutOnPosition(row, col); 
            harvesters[i] = harvesterScript;
            
            // Add the starting point to the array
            startingPoints[i, 0] = row;
            if(col == -1)
                startingPoints[i, 1] = 0;
            else
                startingPoints[i, 1] = GlobalData.fieldCols - 1;
        }
        
        // Set the array of harvesters on GlobalData
        GlobalData.harvesters = harvesters;
        
        
        wsClient.SendStartingPoints(startingPoints); 
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

        Common.printMatrix(matrix);
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
