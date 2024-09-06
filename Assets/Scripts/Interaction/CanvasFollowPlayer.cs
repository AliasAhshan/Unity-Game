using UnityEngine;

public class CanvasFollowPlayer : MonoBehaviour
{
    public Transform cameraTransform;  // Drag the camera here in the inspector
    public Vector3 offset;  // Set the offset for where you want the buttons to be relative to the camera

    void LateUpdate()
    {
        if (Time.timeScale == 0) return;  // Skip updating when the game is paused

        // Update button positions relative to the camera
        transform.position = cameraTransform.position + offset;
    }

}
