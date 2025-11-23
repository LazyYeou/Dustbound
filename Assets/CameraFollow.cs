using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;       // Drag the Player object here
    public float smoothSpeed = 5f; // How "lazy" the camera is (Higher = tighter follow)
    public Vector3 offset;         // To keep the camera away from the game plane (Z-axis)

    void LateUpdate()
    {
        // Check if player exists (in case they died)
        if (target == null) return;

        // 1. Calculate target X and Y (we keep Z as the camera's current Z so distance to scene is preserved)
        Vector3 desiredPosition = new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y,
            transform.position.z
        );

        // 2. Smoothly move only X and Y using Lerp on individual components
        float t = smoothSpeed * Time.deltaTime;
        float smoothedX = Mathf.Lerp(transform.position.x, desiredPosition.x, t);
        float smoothedY = Mathf.Lerp(transform.position.y, desiredPosition.y, t);

        // 3. Apply position (preserve Z)
        transform.position = new Vector3(smoothedX, smoothedY, transform.position.z);
    }
}