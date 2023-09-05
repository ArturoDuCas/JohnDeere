using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvesterPlacement : MonoBehaviour
{
    public GameObject harvesterPrefab; // The harvester prefab to instantiate
    public int numHarvestersToPlace = 2; // Number of harvesters to place
    public float harvesterSpacing = 2.0f; // Spacing between harvesters

    private void Start()
    {
        PlaceHarvesters();
    }

    void PlaceHarvesters()
    {
        // Calculate the total width required for all harvesters
        float totalWidth = (numHarvestersToPlace - 1) * harvesterSpacing;

        // Calculate the starting x position to place the first harvester in the middle of the field
        float startX = -totalWidth / 2 + harvesterSpacing * (numHarvestersToPlace - 1) / 2;

        for (int i = 0; i < numHarvestersToPlace; i++)
        {
            GameObject newHarvester = Instantiate(harvesterPrefab);

            // Calculate the x position with an offset
            float xPosition = startX + i * harvesterSpacing;

            // Adjust the z position if needed
            float zPosition = -GlobalData.unit_zSize * 0.99f;

            newHarvester.transform.position = new Vector3(xPosition, 0, zPosition);
        }
    }
}
