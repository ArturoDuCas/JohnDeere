using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour
{
    public GameObject buildingPrefab;

    private GameObject building; 
    // Start is called before the first frame update
    void Start()
    {
    }

    public void SpawnBuilding()
    {
        string name = gameObject.name;
        name = name.Replace("Road", "");
        name = name.Replace("(", "");
        name = name.Replace(")", "");
        string[] coords = name.Split(',');
        
        
        GlobalData.storageRow = int.Parse(coords[0]) + 1;
        GlobalData.storageCol = int.Parse(coords[1]) + 1;
        
        // instantiate the building 
        building = Instantiate(buildingPrefab, transform.position, Quaternion.identity);
        
        
        // set the rotation 
        if (int.Parse(coords[1]) == -1 || int.Parse(coords[1]) == GlobalData.fieldCols)
        {
            building.transform.rotation = Quaternion.Euler(0, 0, 0);
        } else
        {
            building.transform.rotation = Quaternion.Euler(0, 90, 0);
        }


        if (int.Parse(coords[1]) == -1) // si esta del lado izquierdo
        {
            building.transform.position += new Vector3(-10f, 0f, -3f); 
        } else if (int.Parse(coords[1]) == GlobalData.fieldCols) // si esta del lado derecho 
        { 
            building.transform.position += new Vector3(10.5f, 0f, -3f); // 5 6 z 33
        }
        else if (int.Parse(coords[0]) == -1) // si esta abajo
        {
            building.transform.position += new Vector3(-2f, 0f, -10.5f);  
        } else if (int.Parse(coords[0]) == GlobalData.fieldRows) // si esta arroba
        {
            building.transform.position += new Vector3(-2.5f, 0f, 10f);  
        }
        
        
        {
            
        }
        
        
        
        
        
    }
}
