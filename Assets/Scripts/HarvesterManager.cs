using System.Collections.Generic;
using UnityEngine;

public class HarvesterManager : MonoBehaviour
{
    public GameObject harvesterVisualPrefab; 

    private List<GameObject> harvesterVisuals = new List<GameObject>();

    public static HarvesterManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateHarvesterCount(int newCount)
    {
        // i
        int currentCount = harvesterVisuals.Count;

        if (newCount > currentCount)
        {
            // add later animation
            for (int i = currentCount; i < newCount; i++)
            {
                GameObject newHarvesterVisual = Instantiate(harvesterVisualPrefab);
                harvesterVisuals.Add(newHarvesterVisual);

                // next to last
                if (i > 0)
                {
                    Vector3 lastHarvesterPosition = harvesterVisuals[i - 1].transform.position;
                    Vector3 offset = Vector3.right * 2.0f; // Adjust the offset as needed
                    newHarvesterVisual.transform.position = lastHarvesterPosition + offset;
                }
            }
        }
        else if (newCount < currentCount)
        {
            // Remove excess harvester visuals
            for (int i = currentCount - 1; i >= newCount; i--)
            {
                Destroy(harvesterVisuals[i]);
                harvesterVisuals.RemoveAt(i);
            }
        }
    }
}
