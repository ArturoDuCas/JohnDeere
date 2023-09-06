using UnityEngine;
using UnityEngine.UI;

public class IDPrinter : MonoBehaviour
{
    public Text idText; 

    private void Start()
    {

        idText.text = "ID: " + GlobalData.selfID;
    }
}
