using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harvest : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6) // Layer 6 = Corn
        {
            Destroy(other.gameObject);
        }
    }
}
