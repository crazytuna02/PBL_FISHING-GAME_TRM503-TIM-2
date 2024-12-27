using UnityEngine;
using System.Collections.Generic;

public class FishingRodController : MonoBehaviour
{
    public Transform rod;                     // Referensi objek joran
    public List<Transform> fishes;            // Referensi daftar objek ikan
    public float liftSpeed = 1f;              // Kecepatan mengangkat joran
    public float maxLiftHeight = 2f;          // Ketinggian maksimum angkatan joran
    public float bendAmount = 20f;            // Jumlah kelengkungan saat joran diangkat
    public float lowerSpeed = 0.5f;           // Kecepatan menurunkan joran secara otomatis
    public float fishPullStrength = 1f;       // Kekuatan tarikan ikan
    public float playerPullStrength = 1f;     // Kekuatan tarikan pemain
    public float pullIncrement = 0.2f;        // Inkrement kekuatan setiap tarikan
    public float pullCooldown = 1f;           // Cooldown untuk menambah kekuatan
    public float horizontalLimit = 2f;        // Batas horizontal joran (kiri-kanan)

    public ScoreManager scoreManager;         // Referensi ke ScoreManager
    public FishingStatsDisplay fishingStatsDisplay; // Referensi ke FishingStatsDisplay

    private float currentLift = 0f;           // Ketinggian angkatan joran saat ini
    private Vector3 initialPosition;          // Posisi awal joran
    private Quaternion initialRotation;       // Rotasi awal joran
    private bool isFishHooked = false;        // Flag untuk mengecek apakah ada ikan yang terjerat
    private float lastPullTime = 0f;          // Waktu terakhir tarikan pemain

    void Start()
    {
        // Menyimpan posisi dan rotasi awal joran
        initialPosition = rod.localPosition;
        initialRotation = rod.localRotation;
    }

    void Update()
    {
        if (isFishHooked)
        {
            HandleRodPulling(); // Pergerakan joran saat ikan terjerat
        }
        else
        {
            HandleRodMovement(); // Pergerakan joran biasa
        }

        // Menambah kekuatan tarikan pemain
        if (Input.GetKey(KeyCode.Space) && Time.time - lastPullTime >= pullCooldown)
        {
            lastPullTime = Time.time; // Memperbarui waktu terakhir tarikan
            IncreasePlayerPullStrength(); // Meningkatkan kekuatan tarikan
        }

        // Perbarui posisi dan rotasi joran
        UpdateRodPositionAndRotation();
    }

    private void HandleRodMovement()
    {
        // Menggabungkan input keyboard (W/S) dan joystick (Vertical Axis)
        float verticalInput = Input.GetAxis("Vertical"); // Input dari joystick
        
        // Menambahkan input dari keyboard
        if (Input.GetKey(KeyCode.S)) verticalInput = 1f;  // Tekan S untuk mengangkat
        if (Input.GetKey(KeyCode.W)) verticalInput = -1f; // Tekan W untuk menurunkan

        else
        {
            currentLift -= lowerSpeed * Time.deltaTime; // Turun perlahan jika tidak ada input
        }

        // Mengangkat atau menurunkan joran
        currentLift += verticalInput * liftSpeed * Time.deltaTime;

        // Membatasi ketinggian joran
        currentLift = Mathf.Clamp(currentLift, 0, maxLiftHeight);

        // Menggerakkan joran ke kiri dan kanan (A/D atau Horizontal Axis)
        float horizontalInput = Input.GetAxis("Horizontal"); // Input dari joystick
        if (Input.GetKey(KeyCode.A)) horizontalInput = -1f;  // Tekan A untuk ke kiri
        if (Input.GetKey(KeyCode.D)) horizontalInput = 1f;   // Tekan D untuk ke kanan

        // Update posisi horizontal
        rod.localPosition += new Vector3(horizontalInput * liftSpeed * Time.deltaTime, 0, 0);

        // Membatasi posisi horizontal joran
        float clampedX = Mathf.Clamp(rod.localPosition.x, -horizontalLimit, horizontalLimit);
        rod.localPosition = new Vector3(clampedX, rod.localPosition.y, rod.localPosition.z);
    }

    private void HandleRodPulling()
    {
        foreach (var fish in fishes)
        {
            if (fish != null)
            {
                float distanceToFish = Vector3.Distance(rod.position, fish.position);

                // Jika ikan cukup dekat dan ada tarikan yang cukup kuat
                if (distanceToFish < 5f && playerPullStrength >= fishPullStrength)
                {
                    if (scoreManager != null)
                    {
                        scoreManager.AddScore(10); // Tambah skor saat ikan ditangkap
                        scoreManager.RecordLastPullStrength(); // Simpan nilai pull strength terakhir
                        UnhookFish(); // Lepas ikan setelah ditangkap
                    }
                    return; // Keluar dari loop setelah ikan ditangkap
                }
            }
        }
    }

    private void UpdateRodPositionAndRotation()
    {
        rod.localPosition = initialPosition + new Vector3(0, currentLift, 0);
        rod.localRotation = initialRotation * Quaternion.Euler(0, 0, -currentLift * bendAmount);
    }

    private void IncreasePlayerPullStrength()
    {
        playerPullStrength += pullIncrement; // Tambah kekuatan tarikan
        if (scoreManager != null)
        {
            scoreManager.UpdatePullStrength(playerPullStrength); // Update ke ScoreManager
        }
        Debug.Log("Player Pull Strength: " + playerPullStrength);
    }

    public void HookFish()
    {
        isFishHooked = true;
    }

    public void UnhookFish()
    {
        isFishHooked = false;
    }

    public void AddFish(Transform newFish)
    {
        if (!fishes.Contains(newFish))
        {
            fishes.Add(newFish);
        }
    }

    public void RemoveFish(Transform fishToRemove)
    {
        if (fishes.Contains(fishToRemove))
        {
            fishes.Remove(fishToRemove);
        }
    }
}
