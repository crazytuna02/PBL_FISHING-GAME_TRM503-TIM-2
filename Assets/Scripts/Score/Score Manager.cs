using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI pullStrengthText;
    public TextMeshProUGUI finalPullStrengthText;
    public TextMeshProUGUI fishCountText;

    [Header("Game Settings")]
    public float initialGameDuration = 60f; // Durasi awal game
    public float durationIncrement = 30f;  // Penambahan durasi setiap level
    public int tunaPoints = 10;            // Poin untuk ikan tuna
    public int bawalPoints = 5;           // Poin untuk ikan bawal

    private int totalScore = 0;
    private int totalFishCount = 0;
    private int tunaCount = 0;
    private int bawalCount = 0;
    private float timeRemaining;
    private bool isGameActive = true;
    private float pullStrength = 0f;
    private float totalPullStrength = 0f;
    private float lastPullStrength = 0f;

    private int currentLevel = 1;         // Level permainan saat ini
    public float pullIncrement = 0.1f;   // Nilai tambahan saat menarik

    // Referensi ke FishingStatsDisplay untuk mengambil nilai pull strength dan jumlah ikan
    public FishingStatsDisplay fishingStatsDisplay;

    void Start()
    {
        SetGameDuration();  // Atur durasi permainan sesuai level
        UpdateUI();
    }

    void Update()
    {
        if (isGameActive)
        {
            timeRemaining -= Time.deltaTime;

            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                isGameActive = false;
                DisplayFinalScore();
            }

            // Deteksi tombol S dan W
            if (Input.GetKey(KeyCode.S))
            {
                IncreasePullStrength(pullIncrement, "up");
            }
            else if (Input.GetKey(KeyCode.W))
            {
                IncreasePullStrength(pullIncrement, "down");
            }
        }

        UpdateUI();
    }

    private void SetGameDuration()
    {
        // Hitung durasi berdasarkan level
        timeRemaining = initialGameDuration + (durationIncrement * (currentLevel - 1));
    }

    private void UpdateUI()
    {
        scoreText.text = "Score: " + totalScore.ToString();
        timerText.text = Mathf.CeilToInt(timeRemaining).ToString() + "s";
        pullStrengthText.text = "Pull Strength: " + pullStrength.ToString("F2");

        totalFishCount = fishingStatsDisplay != null ? fishingStatsDisplay.GetFishCount() : 0;

        if (isGameActive)
        {
            fishCountText.text = $"Total Fish: {totalFishCount}";
        }

        Debug.Log($"UI Updated - Total Score: {totalScore}, Total Fish: {totalFishCount}");
    }

    private void DisplayFinalScore()
    {
        if (fishingStatsDisplay != null)
        {
            lastPullStrength = fishingStatsDisplay.GetPullStrength();
        }

        finalPullStrengthText.text = $"Final Pull Strength: {lastPullStrength:F2}";
        fishCountText.text = $"Total Fish Caught: {totalFishCount}";

        Debug.Log($"Game Over - Final Score: {totalScore}, Total Fish Caught: {totalFishCount}, Final Pull Strength: {lastPullStrength}");
    }

    public void AddScore(int points)
    {
        if (isGameActive)
        {
            totalScore += points;
            Debug.Log("Score added: " + points + ", Total Score: " + totalScore);
            UpdateUI();
        }
    }

    public void UpdatePullStrength(float strength)
    {
        if (isGameActive && fishingStatsDisplay != null)
        {
            pullStrength = fishingStatsDisplay.GetPullStrength();
            totalPullStrength += strength;
            Debug.Log($"Pull Strength Synchronized: {pullStrength:F2}");
            UpdateUI();
        }
    }

    public void RecordLastPullStrength()
    {
        if (isGameActive)
        {
            lastPullStrength = pullStrength;
            Debug.Log($"Last Pull Strength Recorded: {lastPullStrength:F2}");
        }
    }

    private void IncreasePullStrength(float increment, string direction)
    {
        if (direction == "up")
        {
            pullStrength += increment; // Tambahkan pullStrength saat menarik ke atas
            Debug.Log($"Pull Strength Increased Upwards: {pullStrength:F2}");
        }
        else if (direction == "down")
        {
            pullStrength += increment; // Tambahkan pullStrength saat menarik ke bawah
            Debug.Log($"Pull Strength Increased Downwards: {pullStrength:F2}");
        }

        totalPullStrength += increment;
    }

    public void CatchFish(string fishType)
    {
        if (isGameActive)
        {
            int points = 0;

            // Tentukan jumlah poin berdasarkan jenis ikan yang ditangkap
            if (fishType == "Tuna")
            {
                tunaCount++;
                points = tunaPoints;
                Debug.Log($"Tuna Caught! Total Tuna: {tunaCount}, Points Earned: {points}");
            }
            else if (fishType == "Bawal")
            {
                bawalCount++;
                points = bawalPoints;
                Debug.Log($"Bawal Caught! Total Bawal: {bawalCount}, Points Earned: {points}");
            }

            if (points > 0)
            {
                totalScore += points;
                totalFishCount++;
                Debug.Log($"Fish Caught! Total Score: {totalScore}");
                UpdateUI();
            }
        }
    }

    public void NextLevel()
    {
        if (!isGameActive)
        {
            currentLevel++; // Naikkan level
            SetGameDuration(); // Perbarui durasi
            totalScore = 0;    // Reset skor
            pullStrength = 0;  // Reset pull strength
            isGameActive = true; // Aktifkan permainan kembali
            UpdateUI();
            Debug.Log($"Next Level Started: Level {currentLevel}");
        }
    }

    public void StarfishAttack()
    {
        if (isGameActive)
        {
            totalScore -= 2;

            // Pastikan skor tidak menjadi negatif
            if (totalScore < 0)
                totalScore = 0;

            Debug.Log("Starfish Attack! Score reduced by 2. Current Score: " + totalScore);
            UpdateUI();
        }
    }
}
