using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvesterController : MonoBehaviour
{
    public float harvesterSpeed = 0.1f;
    public float harvesterBackSpeed = 0.05f;
    float rotationAmount = 0.0f;  // Cantidad de rotación acumulada
    public float rotationSpeed = 70.0f;
    Quaternion targetRotation;

    public string cornLayer; 
    public float radioDetection = 0.5f;
    
    
    Transform harvesterTransform;
    Vector3 harvesterPosition;


    void Start()
    {
        harvesterTransform = transform;
        harvesterPosition = new Vector3(-2f, harvesterTransform.localScale.y * 0.5f, harvesterTransform.localScale.z * 0.5f);  // Ajusta el punto de pivote hacia atrás
        harvesterTransform.position = harvesterPosition;
        targetRotation = harvesterTransform.rotation;  // Inicializar la rotación objetivo
    }

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Movimiento frontal y trasero
        if (verticalInput > 0)
        {
            MoveFront();
        }
        else if (verticalInput < 0)
        {
            MoveBack();
        }
        

        // Rotación
        if (horizontalInput != 0)
        {
            rotationAmount += horizontalInput * rotationSpeed * Time.deltaTime;
            targetRotation = Quaternion.Euler(0, rotationAmount, 0);
        }

        // Aplicar la rotación suave utilizando Lerp
        harvesterTransform.rotation = Quaternion.Lerp(harvesterTransform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    public void MoveFront()
    {
        // Obtener la dirección hacia adelante basada en la rotación
        Vector3 forwardDirection = harvesterTransform.forward;
        
        // Mover el vehículo en la dirección hacia adelante
        harvesterPosition += forwardDirection * harvesterSpeed;
        harvesterTransform.position = harvesterPosition;
    }

    public void MoveBack()
    {
        // Obtener la dirección hacia adelante basada en la rotación
        Vector3 forwardDirection = harvesterTransform.forward;
        
        // Mover el vehículo en la dirección opuesta a la adelante
        harvesterPosition -= forwardDirection * harvesterBackSpeed;
        harvesterTransform.position = harvesterPosition;
    }
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(cornLayer))
        {
            // Destruir el objeto de maíz
            Destroy(other.gameObject);
            GlobalData.cornHarvested++;
            Debug.Log("Maíz recolectado: " + GlobalData.cornHarvested);
        }
    }
}