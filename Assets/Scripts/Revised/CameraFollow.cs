using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;
    public Vector3 offset;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y,
            transform.position.z
        );

        float t = smoothSpeed * Time.deltaTime;
        float smoothedX = Mathf.Lerp(transform.position.x, desiredPosition.x, t);
        float smoothedY = Mathf.Lerp(transform.position.y, desiredPosition.y, t);

        transform.position = new Vector3(smoothedX, smoothedY, transform.position.z);
    }
}