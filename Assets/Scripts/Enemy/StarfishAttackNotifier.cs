using UnityEngine;
using UnityEngine.UI;

public class StarfishAttack : MonoBehaviour
{
    [Header("Settings")]
    public Transform attackTarget;            // Titik tujuan serangan
    public float attackRange = 5f;            // Jarak di mana bintang laut akan menyerang
    public float moveSpeed = 2f;              // Kecepatan gerakan bintang laut

    [Header("UI Elements")]
    public Image attackNotificationImage;    // Referensi ke komponen Image untuk notifikasi serangan
    public Image avoidanceNotificationImage; // Referensi ke komponen Image untuk notifikasi menghindar

    [Header("Audio Settings")]
    public AudioClip notificationSound;      // Suara notifikasi serangan
    private AudioSource audioSource;         // Komponen AudioSource untuk memainkan suara

    [Header("Player & Fishing Rod Settings")]
    public GameObject fishingRod;            // Referensi ke objek joran
    private Vector3 lastRodPosition;         // Posisi terakhir joran untuk mendeteksi pergerakan
    private float rodMovementThreshold = 0.1f; // Batas pergerakan kecil untuk mendeteksi apakah joran bergerak

    private bool isAttacking = false;        // Status serangan bintang laut
    private bool isAvoiding = false;         // Status apakah bintang laut sedang menghindar

    void Start()
    {
        // Pastikan notifikasi tidak aktif saat awal permainan
        if (attackNotificationImage != null)
        {
            attackNotificationImage.enabled = false; // Menyembunyikan notifikasi serangan
        }

        if (avoidanceNotificationImage != null)
        {
            avoidanceNotificationImage.enabled = false; // Menyembunyikan notifikasi menghindar
        }

        // Inisialisasi AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Menyimpan posisi awal joran
        lastRodPosition = fishingRod.transform.position;
    }

    void Update()
    {
        // Periksa jarak antara bintang laut dan titik serangan
        if (attackTarget != null && Vector3.Distance(transform.position, attackTarget.position) < attackRange)
        {
            isAttacking = true;
            MoveTowardsTarget();
        }
        else
        {
            isAttacking = false;
        }

        // Cek pergerakan joran
        CheckForRodMovement();
    }

    void MoveTowardsTarget()
    {
        if (isAttacking)
        {
            // Gerakkan bintang laut menuju titik serangan
            Vector3 direction = (attackTarget.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }

    void CheckForRodMovement()
    {
        // Cek pergerakan joran (berdasarkan posisi joran sebelumnya dan sekarang)
        float rodMovement = Vector3.Distance(fishingRod.transform.position, lastRodPosition);

        // Cek apakah pemain menekan tombol 'S' untuk menggerakkan joran
        bool isRodMoving = Input.GetKey(KeyCode.S); // Cek apakah tombol 'S' ditekan

        if (isAttacking)
        {
            if (isRodMoving)
            {
                // Jika joran bergerak (pemain menekan 'S'), tampilkan notifikasi menghindar
                ShowAvoidanceNotification();
            }
            else
            {
                // Jika joran tidak bergerak, tampilkan notifikasi serangan
                ShowAttackNotification();
            }
        }
        else
        {
            // Menyembunyikan kedua notifikasi jika bintang laut tidak menyerang
            HideNotification(attackNotificationImage);
            HideNotification(avoidanceNotificationImage);
        }

        // Update posisi terakhir joran
        lastRodPosition = fishingRod.transform.position;
    }

    void ShowAttackNotification()
    {
        if (attackNotificationImage != null && !attackNotificationImage.enabled)
        {
            // Menampilkan gambar notifikasi serangan
            attackNotificationImage.enabled = true;

            // Mainkan suara notifikasi jika tersedia
            if (notificationSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(notificationSound);
            }

            // Panggil coroutine untuk menghilangkan notifikasi setelah 1 detik
            StartCoroutine(HideNotification(attackNotificationImage));
        }
    }

    void ShowAvoidanceNotification()
    {
        if (avoidanceNotificationImage != null && !avoidanceNotificationImage.enabled)
        {
            // Menampilkan gambar notifikasi menghindar
            avoidanceNotificationImage.enabled = true;

            // Mainkan suara notifikasi jika tersedia
            if (notificationSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(notificationSound);
            }

            // Panggil coroutine untuk menghilangkan notifikasi setelah 1 detik
            StartCoroutine(HideNotification(avoidanceNotificationImage));
        }
    }

    // Coroutine untuk menyembunyikan notifikasi setelah 1 detik
    System.Collections.IEnumerator HideNotification(Image notificationImage)
    {
        // Tunggu selama 1 detik
        yield return new WaitForSeconds(1f);

        // Nonaktifkan notifikasi
        if (notificationImage != null)
        {
            notificationImage.enabled = false;
        }
    }
}
