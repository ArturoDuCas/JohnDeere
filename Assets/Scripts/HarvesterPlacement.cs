using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvesterPlacement : MonoBehaviour
{
    public GameObject harvesterPrefab; // The harvester prefab to instantiate
    public float harvesterSpacing = 2.0f; // Spacing between harvesters

    private List<GameObject> harvesters = new List<GameObject>(); // Keep track of the spawned harvesters

    private void Start()
    {
        PlaceHarvesters();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddHarvester();
        }
    }

    void PlaceHarvesters()
    {
        int numHarvestersToPlace = GlobalData.numTrucks; // Get the number of harvesters from GlobalData

        // Calculate the total width required for all harvesters
        float totalWidth = (numHarvestersToPlace - 1) * harvesterSpacing;

        // Calculate the starting x position to place the first harvester in the middle of the field
        float startX = -totalWidth / 2 + harvesterSpacing * (numHarvestersToPlace - 1) / 2;

        for (int i = 0; i < numHarvestersToPlace; i++)
        {
            GameObject newHarvester = Instantiate(harvesterPrefab);

            // x position with an offset
            float xPosition = startX + i * harvesterSpacing;

            // Adjust the z position if needed
            float zPosition = -GlobalData.unit_zSize * 0.99f;

            newHarvester.transform.position = new Vector3(xPosition, 0, zPosition);

            harvesters.Add(newHarvester); // Add the harvester to the list
        }
    }

    void AddHarvester()
    {
        GameObject lastHarvester = harvesters[harvesters.Count - 1];
        float xPosition = lastHarvester.transform.position.x + harvesterSpacing;

        // z position
        float zPosition = -GlobalData.unit_zSize * 0.99f;

        GameObject newHarvester = Instantiate(harvesterPrefab, new Vector3(xPosition, 0, zPosition), Quaternion.identity);
        harvesters.Add(newHarvester);
    }
}
