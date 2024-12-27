using UnityEngine;

public class NotifikasiPasangUmpan : MonoBehaviour
{
    public GameObject panelNotifikasi; // Panel notifikasi yang diatur di Inspector
    public Transform umpan; // Objek umpan
    public Transform kail; // Objek kail
    public float jarakToleransi = 0.5f; // Toleransi jarak untuk mendeteksi keberhasilan pemasangan umpan
    public float durasiNotifikasi = 1f; // Durasi notifikasi muncul dalam detik
    public AudioClip audioClipNotifikasi; // File audio WAV untuk notifikasi
    private AudioSource audioSource; // Komponen AudioSource

    private bool umpanDipasan = false; // Menyimpan status apakah umpan sudah dipasang

    void Start()
    {
        if (panelNotifikasi != null)
        {
            panelNotifikasi.SetActive(false); // Panel notifikasi dimulai dalam keadaan tidak aktif
        }

        // Tambahkan komponen AudioSource secara otomatis jika belum ada
        audioSource = gameObject.AddComponent<AudioSource>();
        if (audioClipNotifikasi != null)
        {
            audioSource.clip = audioClipNotifikasi;
        }
    }

    void Update()
    {
        // Periksa apakah umpan baru saja dipasang
        if (!umpanDipasan && Vector3.Distance(umpan.position, kail.position) <= jarakToleransi)
        {
            umpanDipasan = true; // Tandai bahwa umpan telah dipasang
            TampilkanNotifikasi();
        }

        // Reset status jika umpan menjauh dari kail
        if (umpanDipasan && Vector3.Distance(umpan.position, kail.position) > jarakToleransi)
        {
            umpanDipasan = false;
        }
    }

    void TampilkanNotifikasi()
    {
        if (panelNotifikasi != null)
        {
            panelNotifikasi.SetActive(true); // Aktifkan panel notifikasi
        }

        // Mainkan suara notifikasi jika klip audio tersedia
        if (audioClipNotifikasi != null)
        {
            audioSource.Play();
        }

        // Jalankan fungsi untuk menutup notifikasi setelah durasi tertentu
        Invoke(nameof(TutupNotifikasi), durasiNotifikasi);
    }

    void TutupNotifikasi()
    {
        if (panelNotifikasi != null)
        {
            panelNotifikasi.SetActive(false); // Sembunyikan panel notifikasi
        }
    }
}
