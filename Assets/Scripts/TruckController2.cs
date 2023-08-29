using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckController2 : MonoBehaviour
{
    public HarvesterController harvester;
    private Vector3 newPosition;

    Transform truckTransform; 

    public float rotationSpeed = 70.0f;
    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal"); 
        float verticalInput = Input.GetAxis("Vertical"); 

        newPosition = new Vector3(harvester.transform.position.x, 0, harvester.transform.position.z);
        transform.position = newPosition;

        truckTransform.rotation = Quaternion.Lerp(harvester.transform.position.x, harvester, Time.deltaTime *  rotationSpeed);

    }
}
