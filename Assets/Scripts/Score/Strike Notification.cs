using UnityEngine;

public class StrikeNotification : MonoBehaviour
{
    [Header("Notification Panels")]
    public GameObject tunaStrikePanel; // Panel notifikasi untuk ikan tuna
    public GameObject bawalStrikePanel; // Panel notifikasi untuk ikan bawal
    public float displayTime = 1f; // Waktu tampil untuk panel notifikasi

    [Header("Audio Clips")]
    public AudioClip tunaAudioClip; // Klip audio untuk tuna
    public AudioClip bawalAudioClip; // Klip audio untuk bawal
    private AudioSource audioSource; // Komponen AudioSource

    private bool hasTunaBeenNotified = false; // Cek apakah tuna sudah diberi notifikasi
    private bool hasBawalBeenNotified = false; // Cek apakah bawal sudah diberi notifikasi

    private void Start()
    {
        // Sembunyikan semua panel di awal
        if (tunaStrikePanel != null)
            tunaStrikePanel.SetActive(false);
        else
            Debug.LogError("Tuna strike panel GameObject not assigned!");

        if (bawalStrikePanel != null)
            bawalStrikePanel.SetActive(false);
        else
            Debug.LogError("Bawal strike panel GameObject not assigned!");

        // Tambahkan AudioSource ke gameObject ini jika belum ada
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tuna") && !hasTunaBeenNotified)
        {
            hasTunaBeenNotified = true; // Tandai bahwa notifikasi telah ditampilkan
            ShowTunaStrikePanel();
        }
        else if (other.CompareTag("Bawal") && !hasBawalBeenNotified)
        {
            hasBawalBeenNotified = true; // Tandai bahwa notifikasi telah ditampilkan
            ShowBawalStrikePanel();
        }
    }

    private void ShowTunaStrikePanel()
    {
        if (tunaStrikePanel != null)
        {
            tunaStrikePanel.SetActive(true); // Tampilkan panel tuna
            if (tunaAudioClip != null)
            {
                audioSource.PlayOneShot(tunaAudioClip); // Mainkan audio tuna
            }
            Invoke("HideTunaStrikePanel", displayTime); // Sembunyikan setelah 1 detik
        }
    }

    private void ShowBawalStrikePanel()
    {
        if (bawalStrikePanel != null)
        {
            bawalStrikePanel.SetActive(true); // Tampilkan panel bawal
            if (bawalAudioClip != null)
            {
                audioSource.PlayOneShot(bawalAudioClip); // Mainkan audio bawal
            }
            Invoke("HideBawalStrikePanel", displayTime); // Sembunyikan setelah 1 detik
        }
    }

    private void HideTunaStrikePanel()
    {
        if (tunaStrikePanel != null)
        {
            tunaStrikePanel.SetActive(false); // Sembunyikan panel tuna
            hasTunaBeenNotified = false; // Reset flag agar bisa memunculkan notifikasi lagi
        }
    }

    private void HideBawalStrikePanel()
    {
        if (bawalStrikePanel != null)
        {
            bawalStrikePanel.SetActive(false); // Sembunyikan panel bawal
            hasBawalBeenNotified = false; // Reset flag agar bisa memunculkan notifikasi lagi
        }
    }
}
