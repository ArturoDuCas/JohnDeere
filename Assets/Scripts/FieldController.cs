using System;
using UnityEngine;
using UnityEngine.ProBuilder;

public class FieldController : MonoBehaviour
{
    public Vector3 newDimensions = new Vector3(1.0f, 0.0f, 1.0f);
    public GameObject cornPrefab; 
    public float rowSpacing = 0.7f;
    public float plantSpacingMin = 0.3f; 
    public float plantSpacingMax = 0.4f;
    

    void Start()
    {
        Resize();
        GenerateCorn(); 
    }

    void Resize()
    {
        Transform planeTransform = transform; 
        
        float scaleFactorX = newDimensions.x / 10.0f;
        float scaleFactorZ = newDimensions.z / 10.0f;
        
        planeTransform.localScale = new Vector3(scaleFactorX, 1.0f, scaleFactorZ);
        // Mover el plano de manera que la esquina inferior izquierda siga en (0, 0, 0)
        planeTransform.position = new Vector3(newDimensions.x * 0.5f, 0.0f, newDimensions.z * 0.5f);
    }

    void GenerateCorn()
    {
        Transform planeTransform = transform; 
        int numRows = Mathf.FloorToInt(newDimensions.z / rowSpacing);
        float plantXOffset = newDimensions.x / 2.0f;
        

        for (int row = 0; row < numRows; row++)
        {
            float zPosition = (row * rowSpacing) - planeTransform.position.z;

            int numPlants = Mathf.FloorToInt(newDimensions.x / plantSpacingMin);
            for (int plant = 0; plant < numPlants; plant++)
            {
                float xPosition = plant * plantSpacingMin;
                Vector3 position = new Vector3(xPosition - plantXOffset, 0.0f, zPosition);
                position += transform.position; // Ajustar la posición global

                Instantiate(cornPrefab, position, Quaternion.identity, transform);
            }
        }
    }
}
