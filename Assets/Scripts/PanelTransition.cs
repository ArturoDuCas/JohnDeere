using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelTransition : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject idPanel;

    public void ShowIDPanel()
    {
        mainMenuPanel.SetActive(false);
        idPanel.SetActive(true);
    }
}
