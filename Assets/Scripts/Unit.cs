using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public void DeleteCorn()
    {
        int numChilds = transform.childCount;
        
        for (int i = 0; i < numChilds; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            Destroy(child);
        }
    }   
}

