using System;
using UnityEngine;
using UnityEngine.ProBuilder;

public class FieldController : MonoBehaviour
{
    public GameObject cornPrefab; 
   

	public float largo = 10.0f;  // x axis
	public float ancho = 6.0f; // z axis
	private float plantSpacing = 0.5f;
    

    void Start()
    {
        Resize();
        GenerateCorn(); 
    }

    void Resize()
    {   
		// Ajustar el tamaño del plano
        float scaleFactorX = largo / 10.0f; // 50m de largo 
        float scaleFactorZ = ancho / 10.0f; // 6m de ancho
        transform.localScale = new Vector3(scaleFactorX, 1.0f, scaleFactorZ);

        // Mover el plano de manera que la esquina inferior izquierda siga en (0, 0, 0)
        transform.position = new Vector3(largo * 0.5f, 0.0f, ancho * 0.5f);
    }

    void GenerateCorn()
    {
		float plantXOffset = largo / 2.0f;
        for (int row = 0; row < 6; row++) // 6 rows = harvester width
        {
            float zPosition = row - transform.position.z;

            int numPlants = Mathf.FloorToInt(largo / plantSpacing);
            for (int plant = 0; plant < numPlants; plant++)
            {
                float xPosition = plant * plantSpacing;
                Vector3 position = new Vector3(xPosition - plantXOffset, 0.0f, zPosition);
                position += transform.position; // Ajustar la posición global

                Instantiate(cornPrefab, position, Quaternion.identity, transform);
            }
        }
    }
}
