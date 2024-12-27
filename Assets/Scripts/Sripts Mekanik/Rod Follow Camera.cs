using UnityEngine;

public class RodFollowCamera : MonoBehaviour
{
    [Header("Camera Reference")]
    public Transform cameraTransform; // Referensi ke kamera utama

    [Header("Offset Settings")]
    public Vector3 positionOffset = new Vector3(0.5f, -0.5f, 0.5f); // Posisi offset joran relatif ke kamera
    public Vector3 rotationOffset = new Vector3(0f, 0f, 0f); // Rotasi offset joran relatif ke kamera

    void LateUpdate()
    {
        if (cameraTransform == null) return;

        // Sinkronkan posisi joran dengan kamera ditambah offset
        transform.position = cameraTransform.position + cameraTransform.TransformDirection(positionOffset);

        // Sinkronkan rotasi joran dengan kamera ditambah offset
        transform.rotation = cameraTransform.rotation * Quaternion.Euler(rotationOffset);
    }
}
