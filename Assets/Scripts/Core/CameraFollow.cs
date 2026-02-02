using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;           // The player
    public float smoothTime = 0.25f;   // The "Cinematic" lag amount
    public Vector3 offset = new Vector3(0, 2, -10);

    [Header("Map Boundaries")]
    public bool enableBounds = true;   // Check this box to turn on limits
    public Vector2 minPosition;        // The bottom-left limit
    public Vector2 maxPosition;        // The top-right limit

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target != null)
        {
            // 1. Calculate where the camera WANTS to go
            Vector3 targetPosition = target.position + offset;

            // 2. Clamp that position inside the box (Keep camera inside the level)
            if (enableBounds)
            {
                targetPosition.x = Mathf.Clamp(targetPosition.x, minPosition.x, maxPosition.x);
                targetPosition.y = Mathf.Clamp(targetPosition.y, minPosition.y, maxPosition.y);
            }

            // 3. Smoothly move there
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
    }
}