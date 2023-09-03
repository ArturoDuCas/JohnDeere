using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlHarvester : MonoBehaviour
{
    public LayerMask cornLayer;
    
    private float movementSpeed = 2.0f;

    public float fuel = 100; 
    public float fuelConsumption = 20;

    public int currentRow; 
    public int currentCol;

    // Definir GlobalData o importar si es una clase externa

    void Start()
    {
        currentRow = 0;
        currentCol = 0; // Cambiado a 0
        GoToUnit(0, 0);
        // HarvestUnit();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            HarvestUp();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            HarvestRight();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            HarvestLeft();
        }
    }

    void GoToUnit(int row, int col)
    {
        // Implementa la lógica para moverse a una unidad específica
    }

    void HarvestUp()
    {
        // Obtener la posición de finalización
        Vector3 finishPosition = transform.position + Vector3.forward * GlobalData.unit_zSize;
        currentRow += 1;

        // Rotar el harvester para que mire hacia arriba
        transform.rotation = Quaternion.Euler(0, 270, 0);

        StartCoroutine(HarvestCoroutine(finishPosition));
    }

    IEnumerator GoUpCoroutine(float finishZPosition)
    {
        Vector3 finishPosition = new Vector3(transform.position.x, transform.position.y, finishZPosition);
        float distance = Vector3.Distance(transform.position, finishPosition);
        
        while (distance >= 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, finishPosition, movementSpeed * Time.deltaTime);
            distance = Vector3.Distance(transform.position, finishPosition);
            yield return null;
        }
        
        fuel -= fuelConsumption / 3;
    }

    void HarvestRight()
    {
        // Obtener la posición de finalización
        Vector3 finishPosition = transform.position + Vector3.right * GlobalData.unit_xSize;
        currentCol += 1;

        // Rotar el harvester para que mire hacia la derecha
        transform.rotation = Quaternion.Euler(0, 0, 0);

        StartCoroutine(HarvestCoroutine(finishPosition));
    }

    void HarvestLeft()
    {
        // Obtener la posición de finalización
        Vector3 finishPosition = transform.position - Vector3.right * GlobalData.unit_xSize;
        currentCol -= 1;

        // Rotar el harvester para que mire hacia la izquierda
        transform.rotation = Quaternion.Euler(0, 180, 0);

        StartCoroutine(HarvestCoroutine(finishPosition));
    }

    // Precondición: el recolector debe estar en el lado derecho o izquierdo de la unidad
    void HarvestUnit()
    {
        // Implementa la lógica para cosechar una unidad
    }

    IEnumerator HarvestCoroutine(Vector3 finishPosition)
    {
        GlobalData.fieldMatrix[currentRow, currentCol] = 2;
        
        float distance = Vector3.Distance(transform.position, finishPosition);

        while (distance >= 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, finishPosition, movementSpeed * Time.deltaTime);
            distance = Vector3.Distance(transform.position, finishPosition);
            yield return null;
        }
        
        fuel -= fuelConsumption;
        GlobalData.fieldMatrix[currentRow, currentCol] = 0; // Mueve esta línea después de una cosecha exitosa
    }
}
