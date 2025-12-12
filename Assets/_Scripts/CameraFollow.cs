using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("Drag your Batman/Batmobile here")]
    public Transform target;

    [Header("Camera Settings")]
    [Tooltip("How smoothly the camera follows (lower is smoother)")]
    public float smoothSpeed = 0.125f;
    
    // The distance and height offset from the player
    private Vector3 offset;

    void Start()
    {
        // AUTOMATIC SETUP:
        // Instead of typing numbers, we just calculate the distance 
        // based on where you placed the camera in the Scene view relative to the player.
        // We use InverseTransformPoint to get the offset in "Local Space" (behind the player).
        if (target != null)
        {
            offset = target.InverseTransformPoint(transform.position);
        }
        else
        {
            Debug.LogError("Camera needs a Target! Drag Batman into the Target slot.");
        }
    }

    void LateUpdate()
    {
        // LateUpdate is called AFTER the player has moved for the frame.
        // This prevents the camera from "jittering".
        
        if (target == null) return;

        // 1. Calculate where the camera WANTS to be (Local Space -> World Space)
        // We take the offset and rotate it to match the player's current rotation.
        Vector3 desiredPosition = target.TransformPoint(offset);

        // 2. Smoothly move from current position to desired position
        // This adds that "professional game" feel where the camera lags slightly behind.
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // 3. Apply the position
        transform.position = smoothedPosition;

        // 4. Ensure the camera always looks at the target's back/head
        transform.LookAt(target);
    }
}