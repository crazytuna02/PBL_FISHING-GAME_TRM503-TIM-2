using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedCam : MonoBehaviour
{
    public float moveSpeed = 5f; // Kecepatan gerak
    public float jumpForce = 5f; // Kekuatan lompatan
    public float sensitivity = 100f; // Sensitivitas rotasi kamera
    public float rotateSpeed = 50f; // Kecepatan rotasi kamera dengan panah

    public Transform cameraTransform; // Transform kamera untuk mengontrol rotasi

    private Rigidbody rb; // Referensi ke Rigidbody untuk lompatan
    private bool cursorLocked = true; // Status kursor terkunci

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Mengambil komponen Rigidbody
        LockCursor(); // Mengunci kursor di awal permainan
    }

    // Update is called once per frame
    void Update()
    {
        RotateCameraWithArrowKeys(); // Rotasi dengan tombol panah

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            Jump();
        }

        // Toggle cursor lock/unlock when left mouse button is clicked
        if (Input.GetMouseButtonDown(0))
        {
            cursorLocked = !cursorLocked;
            if (cursorLocked)
            {
                LockCursor();
            }
            else
            {
                UnlockCursor();
            }
        }
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }

    void RotateCameraWithArrowKeys()
    {
        // Ambil input horizontal dari tombol panah kiri/kanan
        float horizontalInput = Input.GetAxis("Horizontal"); // Defaultnya untuk panah kanan dan kiri

        // Hitung rotasi berdasarkan input dan kecepatan rotasi
        float yRotationChange = horizontalInput * rotateSpeed * Time.deltaTime;

        // Tambahkan perubahan rotasi pada rotasi kamera
        cameraTransform.Rotate(0, yRotationChange, 0, Space.World);
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}