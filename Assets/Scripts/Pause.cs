using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel; // Panel untuk menu pause
    private bool isPaused = false;

    private AudioSource[] audioSources; // Array untuk menyimpan semua AudioSource

    void Start()
    {
        pausePanel.SetActive(false); // Pastikan panel pause tidak aktif di awal
        Time.timeScale = 1; // Pastikan game berjalan normal di awal

        // Mengambil semua komponen AudioSource yang ada di dalam scene
        audioSources = FindObjectsOfType<AudioSource>();
    }

    void Update()
    {
        // Memeriksa input tombol "Escape" atau "P" untuk pause
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            TogglePauseGame();
        }

        // Melanjutkan game dengan spasi
        if (isPaused && Input.GetKeyDown(KeyCode.Space))
        {
            ResumeGame();
        }
    }

    public void TogglePauseGame()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        pausePanel.SetActive(true); // Menampilkan panel pause
        Time.timeScale = 0; // Menghentikan waktu di dalam game

        // Menjeda semua suara yang sedang diputar
        foreach (var audioSource in audioSources)
        {
            audioSource.Pause(); // Menghentikan audio yang sedang diputar
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        pausePanel.SetActive(false); // Menyembunyikan panel pause
        Time.timeScale = 1; // Mengembalikan waktu ke normal

        // Melanjutkan semua suara yang dijeda sebelumnya
        foreach (var audioSource in audioSources)
        {
            audioSource.UnPause(); // Melanjutkan audio yang dijeda
        }
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1; // Mengembalikan waktu ke normal sebelum berpindah scene
        SceneManager.LoadScene("intro menu"); // Ganti dengan nama scene menu utama
    }
}
