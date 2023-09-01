using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldFloorGenerator : MonoBehaviour
{
    public GameObject cornPrefab;
    
    private int width = 6; // x axis
    private int height = 6; // z axis
    
    void Start()
    {
        ResizeFloor();
        SetCorn();
    }


    void ResizeFloor()
    {
        transform.localScale = new Vector3(width * .1f, 1, height * .1f);
    }

    void SetCorn()
    {
        for (int i = 0; i < height; i++)
        {
            for (float j = 0; j < width; j += 0.5f)
            {
                GameObject newCorn = Instantiate(cornPrefab, transform);
                newCorn.transform.position = new Vector3(j - (width / 2), 0, i - (height / 2));
            }
        }
    }
}
