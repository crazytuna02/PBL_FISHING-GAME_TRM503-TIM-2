using UnityEngine;

public class RotateReel : MonoBehaviour
{
    public Transform pivotPoint;            // Titik pusat rotasi
    public float rotationSpeed = 360f;      // Kecepatan rotasi dalam derajat per detik
    private bool isRotating = false;        // Status apakah reel sedang berputar

    void Update()
    {
        // Periksa apakah tombol Space ditekan atau dilepaskan
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isRotating = true; // Aktifkan rotasi
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            isRotating = false; // Nonaktifkan rotasi
        }

        // Baca input dari joystick (Axis Joystick 2)
        float horizontalInput = Input.GetAxis("Joystick2Horizontal"); // Contoh nama axis Joystick 2 Horizontal
        float verticalInput = Input.GetAxis("Joystick2Vertical");     // Contoh nama axis Joystick 2 Vertical
        Vector3 rotationDirection = new Vector3(horizontalInput, verticalInput, 0).normalized;

        // Jika tombol Space ditekan atau ada input dari joystick
        if (isRotating)
        {
            if (rotationDirection != Vector3.zero)
            {
                // Reel berputar mengikuti arah joystick
                transform.RotateAround(pivotPoint.position, rotationDirection, rotationSpeed * Time.deltaTime);
            }
            else
            {
                // Reel berputar secara default saat joystick tidak digunakan
                transform.RotateAround(pivotPoint.position, transform.forward, rotationSpeed * Time.deltaTime);
            }
        }
    }
}
