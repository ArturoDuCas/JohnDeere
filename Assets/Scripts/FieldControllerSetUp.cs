using System;
using UnityEngine;

public class FieldControllerSetUp : MonoBehaviour
{
    public Transform fieldTransform;
    public GameObject unitPrefab;  

    private WS_Client wsClient;

    void Start()
    {
        CreateField();
        UpdateParentPosition();
        // wsClient = FindObjectOfType<WS_Client>();
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            AddColumn();
        }
    }

    void CreateGlobalMatrix()
    {
        // Create the matrix
        int[,] matrix = new int[GlobalData.fieldRows, GlobalData.fieldCols];

        // Fill the matrix with 1s (not harvested)
        for (int i = 0; i < GlobalData.fieldRows; i++)
        {
            for (int j = 0; j < GlobalData.fieldCols; j++)
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


void AddColumn()
{
    int newCol = GlobalData.fieldCols; 
    GlobalData.fieldCols++; 

    // pasa por cada row y pone uno 
    for (int row = 0; row < GlobalData.fieldRows; row++)
    {
        GameObject newUnit = Instantiate(unitPrefab, transform);
        
        Vector3 newPosition = new Vector3(newCol  * GlobalData.unit_xSize + 3, 0, row * GlobalData.unit_zSize + 3);


        newUnit.transform.position = newPosition;
        newUnit.name = $"Unit({row}, {newCol})";

        Debug.Log($"Created Unit({row}, {newCol})");
    }
}

}