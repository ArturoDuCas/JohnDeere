using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornController : MonoBehaviour
{
    void Start()
    {
        Transform childTransform = transform.GetChild(0); // Get the child transform
  
        Vector3 newPosition = transform.position;
        // Ajustar la posición para que la parte inferior toque el suelo (y = 0)
		newPosition.y = childTransform.localScale.y * 0.5f;
  
        transform.position = newPosition;
    }
    
}
