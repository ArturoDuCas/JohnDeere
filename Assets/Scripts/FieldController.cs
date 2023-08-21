using UnityEngine;
using UnityEngine.ProBuilder;

public class FieldController : MonoBehaviour
{
    public GameObject fieldGameObject;

    void Update()
    {
        // Vector3 newSize = new Vector3(GlobalData.field_xSize, GlobalData.field_ySize, GlobalData.field_zSize); 
        ProBuilderMesh pbMesh =fieldGameObject.GetComponent<ProBuilderMesh>();



    }
}
