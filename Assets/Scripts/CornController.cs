using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornController : MonoBehaviour
{
    void Start()
    {
        // Posicion actual del maiz
        Vector3 currentPosition = transform.position;

        // Ajustar la posición para que la parte inferior toque el suelo (y = 0)
        Vector3 newPosition = transform.position;
        newPosition.y = 0.8f; // Ajustar la posición vertical a la mitad de la altura
        
        
        transform.position = newPosition;
    }
    
}
