using UnityEngine;

public class PasangUmpan : MonoBehaviour
{
    [Header("References")]
    public Transform hook; // Drag objek hook di sini
    public string pushButtonInput = "Fire1"; // Input untuk push button di controller
    public Transform initialPosition; // Posisi awal umpan
    public AudioSource kecemplungAudio; // Referensi ke AudioSource untuk suara kecemplung

    [Header("Bait Settings")]
    public float resetDelay = 0f; // Waktu delay untuk mengembalikan umpan ke posisi awal (sekarang langsung)

    private bool isBaitAttached = false; // Mengecek apakah umpan terpasang
    private bool isBaitEaten = false; // Mengecek apakah umpan dimakan ikan

    void Start()
    {
        // Menyimpan posisi awal umpan
        if (initialPosition == null)
        {
            initialPosition = transform; // Gunakan posisi objek ini sebagai posisi awal jika belum diatur
        }
    }

    void Update()
    {
        // Input dari keyboard (tombol Z)
        if (Input.GetKeyDown(KeyCode.Z))
        {
            AttachBait();
        }

        // Input dari push button pada controller
        if (Input.GetButtonDown(pushButtonInput))
        {
            AttachBait();
        }
    }

    public void AttachBait()
    {
        if (!isBaitAttached)
        {
            // Pindahkan umpan ke hook
            transform.position = hook.position;

            // Buat umpan mengikuti hook
            transform.parent = hook;

            isBaitAttached = true;
            isBaitEaten = false; // Pastikan umpan tidak dianggap dimakan sebelum waktunya

            // Mainkan suara kecemplung jika tersedia
            if (kecemplungAudio != null)
            {
                kecemplungAudio.Play();
            }

            Debug.Log("Umpan berhasil dipasang.");
        }
    }

    // Panggil metode ini dari script lain (misalnya dari FishMovement saat ikan makan umpan)
    public void BaitEaten()
    {
        if (isBaitAttached && !isBaitEaten)
        {
            isBaitEaten = true; // Umpan dimakan ikan
            Debug.Log("Umpan dimakan ikan!");

            // Panggil ResetBaitPosition langsung tanpa delay
            ResetBaitPosition();
        }
    }

    public void ResetBaitPosition()
    {
        // Reset hanya jika umpan sedang di kail atau dimakan
        if (isBaitAttached || isBaitEaten)
        {
            // Reset umpan ke posisi awal
            transform.position = initialPosition.position;
            transform.parent = null; // Lepaskan umpan dari hook

            isBaitAttached = false;
            isBaitEaten = false;
            Debug.Log("Umpan dikembalikan ke posisi awal.");
        }
    }
}
