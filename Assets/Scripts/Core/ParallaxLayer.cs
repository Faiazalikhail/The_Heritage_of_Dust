using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    public float parallaxFactor = 0.5f; // 0 = far, 1 = near

    private Transform cam;
    private Vector3 lastCamPos;

    void Start()
    {
        cam = Camera.main.transform;
        lastCamPos = cam.position;
    }

    void LateUpdate()
    {
        Vector3 delta = cam.position - lastCamPos;

        // Move background based on camera movement
        transform.position += new Vector3(delta.x * parallaxFactor, delta.y * parallaxFactor, 0);

        lastCamPos = cam.position;
    }
}