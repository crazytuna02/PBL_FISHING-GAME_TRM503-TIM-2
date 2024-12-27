using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; // Singleton instance
    public AudioSource backgroundMusic; // AudioSource untuk musik latar
    public string introMenuSceneName = "IntroMenu"; // Nama scene untuk panel intro menu

    private void Awake()
    {
        // Singleton pattern untuk memastikan hanya satu AudioManager
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Jangan hancurkan saat berpindah scene
            SceneManager.sceneLoaded += OnSceneLoaded; // Daftarkan event saat scene berganti
        }
        else
        {
            Destroy(gameObject); // Hancurkan duplikasi AudioManager
        }
    }

    private void OnDestroy()
    {
        // Pastikan untuk melepaskan event listener saat objek dihancurkan
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Fungsi dipanggil saat scene baru dimuat
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Periksa apakah scene yang aktif adalah Intro Menu
        if (scene.name == introMenuSceneName)
        {
            PlayMusic(); // Mainkan musik jika di scene Intro Menu
        }
        else
        {
            StopMusic(); // Hentikan musik di scene lain
        }
    }

    // Fungsi untuk mulai memainkan musik
    public void PlayMusic()
    {
        if (!backgroundMusic.isPlaying)
        {
            backgroundMusic.Play();
        }
    }

    // Fungsi untuk menghentikan musik
    public void StopMusic()
    {
        if (backgroundMusic.isPlaying)
        {
            backgroundMusic.Stop();
        }
    }
}
