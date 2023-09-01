using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckController : MonoBehaviour
{

    public float truckSpeed = 0.1f; 
    public float truckBackSpeed = 0.05f; 

    Transform truckTransform; 
    Vector3 truckPosition; 

    Quaternion targetTruckRotation; 
    float rotationTruckAmount = 0.0f; 
    public float rotationTruckSpeed = 70.0f; 

    private WS_Client wsClient; 


    // Start is called before the first frame update
    void Start()
    {

        truckTransform = transform; 
        truckPosition = new Vector3(7, 0, 0); 
        truckTransform.position = truckPosition;
        targetTruckRotation = truckTransform.rotation; 

        wsClient = FindObjectOfType<WS_Client>(); 
        
    }


    // Update is called once per frame
    void Update()
    {

        float horizontalInput = Input.GetAxis("Horizontal"); 
        float verticalInput = Input.GetAxis("Vertical"); 

        if(verticalInput > 0){
            MoveBack();
        }
        else if(verticalInput < 0)
        {
            MoveFront(); 
        }

        if(horizontalInput != 0)
        {
            rotationTruckAmount += horizontalInput * rotationTruckSpeed * Time.deltaTime;        
            targetTruckRotation = Quaternion.Euler(0, rotationTruckAmount - 180, 0);
        }

        truckTransform.rotation = Quaternion.Lerp(truckTransform.rotation, targetTruckRotation, Time.deltaTime * rotationTruckSpeed);

    }

    public void MoveFront()
    {
        Vector3 forwardDirection = truckTransform.forward;

        truckPosition += forwardDirection * truckBackSpeed;
        truckTransform.position = truckPosition;
    }

    public void MoveBack()
    {
        Vector3 forwardDirection = truckTransform.forward;

        truckPosition -= forwardDirection * truckSpeed; 
        truckTransform.position = truckPosition;
    }
}
