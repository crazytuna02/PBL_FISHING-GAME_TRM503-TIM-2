using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;

namespace YourNamespace // Sesuaikan dengan namespace Anda
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private float initialGameDuration = 60f; // Durasi awal game
        [SerializeField] private float durationIncrement = 30f; // Penambahan durasi di setiap level
        [SerializeField] private GameObject scorePanel; // Panel skor
        [SerializeField] private TextMeshProUGUI scoreText; // Menggunakan TextMeshProUGUI untuk teks skor
        [SerializeField] private TextMeshProUGUI fishingStatsText; // Panel fishing stats display
        [SerializeField] private Button playButton; // Tombol Play
        [SerializeField] private Button exitButton; // Tombol Exit
        private float timer;
        private int playerScore;
        private int tunaCount; // Jumlah ikan tuna yang ditangkap
        private int bawalCount; // Jumlah ikan bawal yang ditangkap
        private float pullStrength; // Nilai pull strength
        private bool isGameOver = false;
        private int currentLevel = 1; // Level permainan saat ini
        private GameObject currentButton; // Tombol yang dipilih saat ini
        private Color normalColor = Color.white; // Warna normal tombol
        private Color highlightedColor = new Color(1f, 1f, 0f, 1f); // Warna kuning terang

        // Referensi untuk pergerakan player
        [SerializeField] private Rigidbody playerRigidbody; // Jika menggunakan Rigidbody

        void Start()
        {
            timer = initialGameDuration; // Set timer ke durasi awal
            scorePanel.SetActive(false); // Sembunyikan panel di awal
            Time.timeScale = 1; // Pastikan game berjalan normal di awal

            // Atur tombol awal untuk navigasi
            if (playButton != null)
            {
                EventSystem.current.SetSelectedGameObject(playButton.gameObject);
                currentButton = playButton.gameObject;
                HighlightButton(playButton, true); // Tandai tombol awal
            }

            // Inisialisasi fishing stats display
            UpdateFishingStats();
        }

        void Update()
        {
            if (!isGameOver) // Jika game belum selesai
            {
                timer -= Time.deltaTime; // Kurangi timer setiap frame

                if (timer <= 0) // Jika waktu habis
                {
                    timer = 0; // Pastikan timer tidak kurang dari 0
                    EndGame(); // Panggil fungsi EndGame
                }

                UpdateFishingStats(); // Perbarui fishing stats setiap frame
            }
            else
            {
                HandleNavigation(); // Navigasi tombol saat game over
                // Nonaktifkan pergerakan atau kontrol game lainnya
                DisablePlayerMovement(); // Mematikan pergerakan pemain
            }
        }

        public void AddScore(int amount) // Fungsi menambah skor
        {
            if (!isGameOver) // Tambah skor hanya jika game belum selesai
            {
                playerScore += amount;
            }
        }

        public void IncrementTunaCount() // Fungsi untuk menambah jumlah ikan tuna
        {
            if (!isGameOver)
            {
                tunaCount++;
            }
        }

        public void IncrementBawalCount() // Fungsi untuk menambah jumlah ikan bawal
        {
            if (!isGameOver)
            {
                bawalCount++;
            }
        }

        public void UpdatePullStrength(float value) // Fungsi untuk memperbarui pull strength
        {
            pullStrength = value;
            UpdateFishingStats();
        }

        private void UpdateFishingStats() // Fungsi untuk memperbarui teks fishing stats display
        {
            if (fishingStatsText != null)
            {
                fishingStatsText.text = "Pull Strength: " + pullStrength.ToString("F2") +
                                        "\nTimer: " + timer.ToString("F1") + " detik" +
                                        "\nTuna: " + tunaCount +
                                        "\nBawal: " + bawalCount;
            }
        }

        void EndGame() // Fungsi akhir permainan
        {
            isGameOver = true;
            scorePanel.SetActive(true); // Tampilkan panel skor
            scoreText.text = "Skor Akhir: " + playerScore +
                             "\nPull Strength: " + pullStrength.ToString("F2") +
                             "\nLevel: " + currentLevel; // Tambahkan level
            Time.timeScale = 0; // Hentikan semua pergerakan game

            // Atur tombol awal untuk navigasi
            if (playButton != null)
            {
                EventSystem.current.SetSelectedGameObject(playButton.gameObject);
                currentButton = playButton.gameObject;
                HighlightButton(playButton, true); // Tandai tombol awal
            }
        }

        public void NextPlayGame() // Fungsi untuk melanjutkan ke level berikutnya
        {
            if (isGameOver) // Jika game selesai
            {
                currentLevel++; // Naikkan level permainan
                timer = initialGameDuration + (durationIncrement * (currentLevel - 1)); // Tambahkan durasi berdasarkan level
                pullStrength = 0; // Reset pull strength ke awal
                tunaCount = 0; // Reset jumlah ikan tuna
                bawalCount = 0; // Reset jumlah ikan bawal
                playerScore = 0; // Reset skor ke awal
                UpdateFishingStats(); // Perbarui fishing stats
                isGameOver = false;
                scorePanel.SetActive(false); // Sembunyikan panel skor
                Time.timeScale = 1; // Pastikan game berjalan normal
            }
        }

        public void ExitGame() // Fungsi untuk tombol Exit (kembali ke menu utama)
        {
            Time.timeScale = 1; // Pastikan waktu berjalan normal
            SceneManager.LoadScene("intro menu"); // Ganti dengan nama scene menu utama Anda
        }

        private void HandleNavigation()
        {
            // Navigasi kiri (tombol A atau joystick axis)
            if (Input.GetKeyDown(KeyCode.A) || Input.GetAxis("Horizontal") < -0.5f)
            {
                if (currentButton == exitButton.gameObject)
                {
                    HighlightButton(exitButton, false);
                    EventSystem.current.SetSelectedGameObject(playButton.gameObject);
                    currentButton = playButton.gameObject;
                    HighlightButton(playButton, true);
                }
            }

            // Navigasi kanan (tombol D atau joystick axis)
            if (Input.GetKeyDown(KeyCode.D) || Input.GetAxis("Horizontal") > 0.5f)
            {
                if (currentButton == playButton.gameObject)
                {
                    HighlightButton(playButton, false);
                    EventSystem.current.SetSelectedGameObject(exitButton.gameObject);
                    currentButton = exitButton.gameObject;
                    HighlightButton(exitButton, true);
                }
            }

            // Konfirmasi pilihan (tombol Space atau joystick button)
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Submit"))
            {
                Button selectedButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
                selectedButton?.onClick.Invoke();
            }
        }

        private void HighlightButton(Button button, bool isHighlighted)
        {
            ColorBlock colorBlock = button.colors; // Ambil pengaturan warna tombol

            // Ubah warna berdasarkan apakah tombol sedang dipilih atau tidak
            if (isHighlighted)
            {
                colorBlock.highlightedColor = highlightedColor; // Warna saat dipilih
                colorBlock.normalColor = highlightedColor;     // Pastikan warna normal sama saat dipilih
            }
            else
            {
                colorBlock.highlightedColor = normalColor;     // Kembalikan ke warna default
                colorBlock.normalColor = normalColor;          // Kembalikan ke warna default
            }

            button.colors = colorBlock; // Terapkan kembali pengaturan warna tombol
        }

        // Fungsi untuk menonaktifkan pergerakan player saat game selesai
        void DisablePlayerMovement()
        {
            // Menonaktifkan input pergerakan atau kontrol
            if (playerRigidbody != null)
            {
                playerRigidbody.velocity = Vector3.zero; // Hentikan pergerakan player jika menggunakan Rigidbody
            }
        }
    }
}
