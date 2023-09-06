using UnityEngine;

public class SetUpCamera : MonoBehaviour
{
    public Transform fieldTransform; // Reference to the FieldController's transform
    


    public void PositionCamera(){
        if (fieldTransform == null)
        {
            Debug.LogError("FieldTransform is not assigned to the camera controller!");
            return;
        }

        // Calculate the size of the field based on its dimensions and unit size
        float fieldWidth = GlobalData.fieldCols * GlobalData.unit_xSize;
        float fieldHeight = GlobalData.fieldRows * GlobalData.unit_zSize;

        // Calculate the camera's position to look down at the center of the field
        Vector3 cameraPosition = new Vector3(fieldWidth / 2, fieldHeight * 1.5f, fieldHeight / 2);

        // Set the camera's position
        transform.position = cameraPosition;

        // Calculate the orthographic size based on the field's dimensions
        float orthographicSize = Mathf.Max(fieldWidth, fieldHeight) * 0.5f;

        // Set the camera's orthographic size
        Camera mainCamera = GetComponent<Camera>();
        if (mainCamera != null)
        {
            mainCamera.orthographicSize = orthographicSize;
        }
        else
        {
            Debug.LogError("No Camera component found on the camera controller!");
        }

        // Rotate the camera to look down at the field
        transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
    }
    
    private void Start()
    {
        PositionCamera();
    }
}
