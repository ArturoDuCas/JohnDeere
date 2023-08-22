using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornController : MonoBehaviour
{
    public float alturaMaxima = 2.5f;
    public float anchoMaximo = 0.1f; 
    void Start()
    {
        // Posicion actual del maiz
        Vector3 currentPosition = transform.position;
        
        // Generar dimensiones aleatorias (entre 80% y 100% de los valores máximos)
        float altura = Random.Range(alturaMaxima * 0.8f, alturaMaxima);
        float ancho = Random.Range(anchoMaximo * 0.8f, anchoMaximo);
        Debug.Log(anchoMaximo * 0.8f); 
        
        
        // Ajustar la escala
        transform.localScale = new Vector3(ancho, altura, ancho);
        
        
        // Ajustar la posición para que la parte inferior toque el suelo (y = 0)
        Vector3 newPosition = transform.position;
        newPosition.y = altura / 2.0f; // Ajustar la posición vertical a la mitad de la altura
        
        // // Ajustar la posición x y z en función del ancho del maíz
        newPosition.x = ancho / 2.0f + currentPosition.x;
        newPosition.z = ancho / 2.0f + currentPosition.z;
        
        transform.position = newPosition;
    }
    
}
