using UnityEngine;
using TMPro;

public class FishingStatsDisplay : MonoBehaviour
{
    [Header("Elemen UI")]
    public TextMeshProUGUI pullStrengthText; // Teks untuk menampilkan nilai pullStrength
    public TextMeshProUGUI fishCountText;    // Teks untuk menampilkan jumlah ikan
    public TextMeshProUGUI finalPullStrengthText; // Teks untuk menampilkan nilai pullStrength akhir
    public TextMeshProUGUI timerText;       // Teks untuk menampilkan timer permainan

    [Header("Pengaturan Permainan")]
    public float initialGameDuration = 60f; // Durasi awal permainan dalam detik
    public float durationIncrement = 30f;   // Penambahan durasi di setiap level
    public Transform rodTransform;          // Referensi ke transform joran
    public float rodMoveThreshold = 1f;     // Ambang batas pergerakan joran untuk meningkatkan pullStrength

    public float pullIncrement = 0.1f;      // Tambahan nilai pullStrength setiap deteksi
    public float reelPullIncrement = 0.1f;  // Tambahan nilai pullStrength saat reel diputar

    public ScoreManager scoreManager;       // Referensi ke ScoreManager untuk memperbarui pullStrength

    private float timeRemaining;            // Waktu yang tersisa dalam permainan
    private bool isGameActive = true;       // Status permainan, aktif atau tidak
    private float pullStrength = 0f;        // Nilai pullStrength saat ini
    private int fishCount = 0;              // Jumlah ikan yang tertangkap
    private int currentLevel = 1;           // Level permainan saat ini

    private Vector3 lastRodPosition;        // Posisi terakhir joran untuk mendeteksi pergerakan

    void Start()
    {
        InitializeGame(); // Inisialisasi permainan saat pertama kali mulai
    }

    void Update()
    {
        if (isGameActive)
        {
            // Mengurangi waktu yang tersisa setiap frame
            timeRemaining -= Time.deltaTime;

            // Jika waktu habis, berhentikan permainan dan tampilkan hasil
            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                isGameActive = false; // Matikan permainan
                DisplayFinalPullStrength(); // Tampilkan pullStrength akhir
                RestartGameWithAdditionalTime(); // Restart permainan dengan waktu tambahan
            }

            // Jika tombol W ditekan dan joran bergerak ke atas, tingkatkan pullStrength
            if (Input.GetKey(KeyCode.W) && RodMovedUpwards())
            {
                IncreasePullStrength(pullIncrement); // Tingkatkan pullStrength
            }

            // Jika tombol Space ditekan, tingkatkan pullStrength saat reel diputar
            if (Input.GetKey(KeyCode.Space))
            {
                IncreasePullStrength(reelPullIncrement); // Tingkatkan pullStrength
            }

            if (Input.GetKey(KeyCode.S))
            {
                IncreasePullStrength(pullIncrement);
            }

            // Perbarui nilai pullStrength di ScoreManager jika ada
            if (scoreManager != null)
            {
                scoreManager.UpdatePullStrength(pullStrength);
            }
        }

        // Perbarui UI setiap frame
        UpdateUI();
    }

    void IncreasePullStrength(float amount)
    {
        pullStrength += amount; // Menambah nilai pullStrength
        Debug.Log($"Pull Strength Increased: {pullStrength:F2}"); // Debug log untuk melihat perubahan nilai pullStrength
    }

    bool RodMovedUpwards()
    {
        // Menghitung pergerakan joran ke atas
        float movement = rodTransform.position.y - lastRodPosition.y;
        lastRodPosition = rodTransform.position; // Simpan posisi terbaru
        return movement > rodMoveThreshold; // Kembalikan true jika joran bergerak lebih dari ambang batas
    }

    public void FishCaught()
    {
        if (isGameActive)
        {
            fishCount++; // Tambahkan jumlah ikan tertangkap
            Debug.Log($"Ikan Tertangkap! Total Ikan: {fishCount}"); // Log untuk melihat jumlah ikan
            UpdateUI(); // Perbarui UI setelah ikan tertangkap
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ketika joran mengenai objek dengan tag "Fish", ikan dianggap tertangkap
        if (other.CompareTag("Fish"))
        {
            FishCaught(); // Panggil method untuk menangkap ikan
            Destroy(other.gameObject); // Hancurkan objek ikan
        }
    }

    void UpdateUI()
    {
        // Perbarui teks UI dengan nilai saat ini
        pullStrengthText.text = $"{pullStrength:F2}"; // Tampilkan nilai pullStrength dengan dua angka desimal
        fishCountText.text = $"{fishCount}"; // Tampilkan jumlah ikan

        // Menampilkan waktu yang tersisa
        if (isGameActive)
        {
            timerText.text = $"{timeRemaining:F1}s"; // Update timer setiap frame
        }
    }

    public float GetPullStrength()
    {
        return pullStrength; // Mengembalikan nilai pullStrength
    }

    public int GetFishCount()
    {
        return fishCount; // Mengembalikan jumlah ikan tertangkap
    }

    void DisplayFinalPullStrength()
    {
        if (finalPullStrengthText != null)
        {
            // Tampilkan nilai pullStrength akhir setelah permainan selesai
            finalPullStrengthText.text = $"Pull Strength Akhir: {pullStrength:F2}";
        }
    }

    public void NextPlayGame()
    {
        // Naikkan level permainan dan sesuaikan durasi permainan
        currentLevel++; 
        float newDuration = initialGameDuration + (durationIncrement * (currentLevel - 1));  // Durasi baru berdasarkan level
        Debug.Log($"Level: {currentLevel}, Durasi Baru: {newDuration}s");
        ResetGame(newDuration); // Reset permainan dengan durasi dan level baru
    }

    void InitializeGame()
    {
        // Inisialisasi permainan pertama kali dimulai
        currentLevel = 1;  // Set level awal
        ResetGame(initialGameDuration); // Mulai permainan dengan durasi awal
    }

    void ResetGame(float duration)
    {
        // Reset semua elemen permainan (waktu, jumlah ikan, pull strength, dll.)
        timeRemaining = duration;  // Setel durasi permainan yang baru
        pullStrength = 0f;          // Reset nilai pull strength
        fishCount = 0;              // Reset jumlah ikan
        isGameActive = true;        // Pastikan permainan aktif
        lastRodPosition = rodTransform.position; // Reset posisi joran
        UpdateUI();  // Perbarui UI dengan nilai baru
        Debug.Log($"Game Reset: TimeRemaining={timeRemaining}s, PullStrength={pullStrength}, FishCount={fishCount}");
    }

    void RestartGameWithAdditionalTime()
    {
        // Tambahkan durasi tambahan setelah waktu habis
        timeRemaining = initialGameDuration + 30f; // Durasi tambahan 30 detik
        isGameActive = true; // Aktifkan kembali permainan
        
        // Reset nilai pullStrength dan jumlah ikan
        fishCount = 0; // Reset jumlah ikan
        pullStrength = 0f; // Reset pull strength

        // Reset posisi joran
        lastRodPosition = rodTransform.position;

        // Update UI untuk mencerminkan perubahan
        UpdateUI(); // Perbarui UI dengan nilai baru
        
        Debug.Log("Game Restarted with 30 Seconds Additional Time.");
    }
}