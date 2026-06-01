using UnityEngine;

public class ParallaxScroll : MonoBehaviour
{
    public Transform gameCamera;
    public float scrollSpeed = 0.3f;

    private Vector3 lastCameraPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lastCameraPosition = gameCamera.position;
    }

    void LateUpdate()
    {
        Vector3 cameraDelta = gameCamera.position - lastCameraPosition;

        transform.position += new Vector3(
            cameraDelta.x * scrollSpeed,
            -cameraDelta.y * scrollSpeed / 10,
            0
        );

        lastCameraPosition = gameCamera.position;
    }
}

        
