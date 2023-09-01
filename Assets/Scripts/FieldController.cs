using System;
using UnityEngine;

public class FieldController : MonoBehaviour
{
    public GameObject unitPrefab;  // Corn section prefab
    public int numCols; // x axis, 6m
    public int numRows; // z axis, 6m

    void Awake()
    {
        CreateGlobalMatrix(); // Create the matrix of corn sections on GlobalData
    }
    void Start()
    {
        CreateField();
        UpdateParentPosition();
    }

    void CreateGlobalMatrix()
    {
        // Create the matrix
        int[,] matrix = new int[numRows, numCols];

        // Fill the matrix with 1s (not harvested)
        for(int i = 0; i < numRows; i++)
        {
            for(int j = 0; j < numCols; j++)
            {
                matrix[i, j] = 1;
            }
        }

        Common.printMatrix(matrix);
        // Set the matrix on GlobalData
        GlobalData.fieldMatrix = matrix;
    }
    
    void CreateField()
    {
        for (int row = 0; row < numRows; row++) // Start generating from top to bottom
        {
            for (int col = 0; col < numCols; col++) // Generate from left to right
            {
                GameObject newUnit = Instantiate(unitPrefab, transform);
                
                // Set the position of the new corn section
                newUnit.transform.position = new Vector3(col * 20, 0, row * 6);
                newUnit.name = $"Unit({row}, {col})";
            }
        }
    }

    void UpdateParentPosition()
    {
        transform.position += new Vector3(5f, 0, 3f);
    }

    
}
