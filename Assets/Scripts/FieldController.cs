using System;
using UnityEngine;

public class FieldController : MonoBehaviour
{
    public GameObject unitPrefab;  // Corn section prefab
    public float largo = 100f;  // x axis, multiplies of 50 
	public float ancho = 60f; // z axis, multiplies of 6
    
    private int numCols;
    private int numRows;
    
    void Start()
    {
        // Number of cols and rows 
        numCols = (int) (largo / 50);
        numRows = (int) (ancho / 6);
        
        CreateField();
        UpdateParentPosition();
    }

    void CreateField()
    {
        for (int row = 0; row < numRows; row++) // Start generating from top to bottom
        {
            for (int col = 0; col < numCols; col++) // Generate from left to right
            {
                GameObject newUnit = Instantiate(unitPrefab, transform);
                
                // Set the position of the new corn section
                newUnit.transform.position = new Vector3(col * 50, 0, row * 6);
                newUnit.name = $"Unit({row}, {col})";
            }
        }
    }

    void UpdateParentPosition()
    {
        transform.position += new Vector3(5f, 0, 3f);
    }

    
}
