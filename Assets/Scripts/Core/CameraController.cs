using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Follow Settings")]
    public float smoothTime = 0.2f;
    private Vector3 velocity = Vector3.zero;

    [Header("Look Ahead")]
    public float lookAheadDistance = 2f;
    public float lookAheadSpeed = 3f;
    private float currentLookAhead = 0f;

    [Header("Vertical Control")]
    public float verticalOffset = 1.5f;

    [Header("Bounds")]
    public Vector2 minBounds;
    public Vector2 maxBounds;

    private float targetLookAhead;

    void LateUpdate()
    {
        if (target == null) return;

        // 🔹 Detect movement direction
        float moveInput = Input.GetAxisRaw("Horizontal");

        targetLookAhead = moveInput * lookAheadDistance;

        currentLookAhead = Mathf.Lerp(
            currentLookAhead,
            targetLookAhead,
            Time.deltaTime * lookAheadSpeed
        );

        // 🔹 Target position
        Vector3 targetPosition = new Vector3(
            target.position.x + currentLookAhead,
            target.position.y + verticalOffset,
            transform.position.z
        );

        // 🔹 Smooth movement
        Vector3 smoothPosition = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );

        // 🔹 Clamp inside level
        smoothPosition.x = Mathf.Clamp(smoothPosition.x, minBounds.x, maxBounds.x);
        smoothPosition.y = Mathf.Clamp(smoothPosition.y, minBounds.y, maxBounds.y);

        transform.position = smoothPosition;
    }
}